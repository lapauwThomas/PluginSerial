using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;
using PluginSerialLib;

namespace PluginSerialLib
{
    public sealed class SerialPortManager
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static SerialPortManager instance;

        private Dictionary<string, SerialPortInst> currentPorts;
        private Mutex mutex = new Mutex();


        public event EventHandler<PortChangedEventArgs> OnPortAdded;
        public event EventHandler<PortChangedEventArgs> OnPortRemoved;

        public event EventHandler<AvailablePortsChangedEventArgs> OnPortsChanged;
        public List<SerialPortInst> AvailablePorts => currentPorts.Values.ToList();


        private SerialPortManager()
        {
            currentPorts = SerialPortSearcher.FindSerialPortsWMI(); //init currentPorts  


        }

        public void SubscribeWMI()
        {
            DeviceChangeNotifier.Start();
            DeviceChangeNotifier.DeviceNotify += OnDeviceChange;
        }

        private void OnDeviceChange(Message msg)
        {
            Thread scanThread = new Thread(
                delegate () { Scan(); });
            scanThread.Start();
        }

        public void UnsubscribeWMI()
        {
            DeviceChangeNotifier.DeviceNotify -= OnDeviceChange;
        }

        public void ReIndex()
        {
            currentPorts = SerialPortSearcher.FindSerialPortsWMI(); //init currentPorts  
        }

        public void Scan()
        {
            mutex.WaitOne();   // Wait until it is safe to enter. 

            Dictionary<string, SerialPortInst> newPorts = SerialPortSearcher.FindSerialPortsWMI(); //init currentPorts

            var removedPorts = currentPorts.Except(newPorts, new KeyComparer<string,SerialPortInst>());
            var addedPorts = newPorts.Except(currentPorts, new KeyComparer<string, SerialPortInst>());



            logger.Trace($"Removing ports:");
            foreach (var removedPort in removedPorts)
            {
                logger.Trace($"\t port {removedPort.Value.Port}");
            }
            logger.Trace($"Adding ports:");
            foreach (var addedPort in addedPorts)
            {
                logger.Trace($"\t port {addedPort.Value.Port}");
            }

            bool portsChanged = false;

            foreach (var removedPort in removedPorts)
            {
                removedPort.Value.PortDisconnected();
                OnPortRemoved?.Invoke(this, new PortChangedEventArgs(removedPort.Value));
                portsChanged = true;

            }

            foreach (var addedPort in addedPorts)
            {
                OnPortAdded?.Invoke(this, new PortChangedEventArgs(addedPort.Value));
                portsChanged = true;
            }


            currentPorts = newPorts;

            logger.Trace($"current ports:");
            foreach (var port in currentPorts)
            {
                logger.Trace($"\t port {port.Value.Port}");
            }

            if (portsChanged)
            {
                OnPortsChanged?.Invoke(this, new AvailablePortsChangedEventArgs(ToPortList(addedPorts),ToPortList(removedPorts),ToPortList(currentPorts)));
            }

            mutex.ReleaseMutex();

        }

        public static List<SerialPortInst> ToPortList(IEnumerable<KeyValuePair<string, SerialPortInst>> source)
        {
            List<SerialPortInst> instlist = new List<SerialPortInst>();
            foreach (var keyValuePair in source)
            {
                instlist.Add(keyValuePair.Value);
            }

            return instlist;
        }

        public static SerialPortManager GetSerialPortManager()
        {
            if (instance == null)
            {
                instance = new SerialPortManager();
            }

            return instance;
        }

        public class PortChangedEventArgs
        {
            public readonly SerialPortInst Port;
            public PortChangedEventArgs(SerialPortInst port)
            {
                Port = port;
            }
        }

        public class AvailablePortsChangedEventArgs
        {
            public List<SerialPortInst> AddedPorts;
            public List<SerialPortInst> RemovedPorts;
            public List<SerialPortInst> AvailablePorts;

            public AvailablePortsChangedEventArgs(List<SerialPortInst> addedPorts, List<SerialPortInst> removedPorts, List<SerialPortInst> availablePorts)
            {
                AddedPorts = addedPorts;
                RemovedPorts = removedPorts;
                AvailablePorts = availablePorts;
            }
        }

    }


    class KeyComparer<T1, T2> : IEqualityComparer<KeyValuePair<T1, T2>>
        where T1 : IComparable
    {
        public bool Equals(KeyValuePair<T1, T2> x, KeyValuePair<T1, T2> y)
        {
            //Check whether the keys are equal
            return x.Key.Equals(y.Key);
        }

        // GetHashCode() must return the same value for equal objects.
        public int GetHashCode(KeyValuePair<T1, T2> kVPair)
        {
            return kVPair.Key.GetHashCode();
        }
    }
}
