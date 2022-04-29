using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using NLog;

namespace PluginSerialLib
{
    public static class SerialPortSearcher
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();



        public static Dictionary<string, SerialPortInst> FindSerialPortsWMI()
        {

            Dictionary<string, SerialPortInst> FoundPorts = new Dictionary<string, SerialPortInst>();
            try
            {

                ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\CIMV2", "SELECT * FROM Win32_PnPEntity");
                ManagementObjectCollection colItems = searcher.Get();

                foreach (ManagementObject queryObj in colItems)
                {
                    try
                    {
                        //logger.Debug(queryObj["Caption"]);


                        if (queryObj["PNPClass"] != null && queryObj["PNPClass"].ToString().Contains("Ports"))
                        {
                            var caption = queryObj["Caption"].ToString();

                            logger.Trace($"Found Port with CAPTION: [{caption}]");
                            var newPort = TryparseQueryObjectToSerialPort(queryObj);
                            if (newPort != null && newPort.valid)
                            {
                                FoundPorts.Add(newPort.Port, newPort);
                                logger.Debug($"Registered serialport {newPort.Port}");
                            }



                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "Error on queryObj");
                    }


                }

            }
            catch (ManagementException)
            { }

            return FoundPorts;

        }

        private static SerialPortInst TryparseQueryObjectToSerialPort(ManagementObject managementObject)
        {

            string friendlyName = (string)managementObject["Name"];
            string port;
            string instancePath = (string)managementObject["PNPDeviceID"];
            string hwid = string.Empty;

            try
            {
                string[] arrHardwareID = (string[])(managementObject["HardwareID"]);

                if (arrHardwareID.Length > 0)
                {
                    hwid = arrHardwareID[0];
                }
            }
            catch (Exception ex)
            {
                logger.Warn($"Could not get HWID");
            }

            try
            {
                port = Regex.Match(friendlyName, @"\((COM[0-9]*)(.*)\)").Groups[1].Value;

                var portnames = System.IO.Ports.SerialPort.GetPortNames();
                if (string.IsNullOrEmpty(port) || !portnames.Contains(port))
                {
                    logger.Warn($"Port [{friendlyName}] not found in SerialPortInst");
                    return null;
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error parsing name to COMPORT");
                return null;
            }

            SerialPortInst.PortType type = SerialPortInst.PortType.Unknown;
            string service = managementObject["Service"].ToString();
            if (ConfigManager.ServiceDescriptions.NativeSerialDescriptionList.Contains(service))
            {
                var inst = new SerialPortInst(port, friendlyName, hwid, instancePath, SerialPortInst.PortType.Native);
                return inst;
            }
            if (ConfigManager.ServiceDescriptions.USBSerialDescriptionList.Contains(service))
            {
                var inst = new UsbSerialPortInst(port, friendlyName, hwid, instancePath);
                return inst;
            }

            return new SerialPortInst(port, friendlyName, hwid, instancePath, SerialPortInst.PortType.Unknown);


        }


    }
}
