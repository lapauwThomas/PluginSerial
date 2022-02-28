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
                                if (newPort!= null && newPort.valid)
                                {
                                    FoundPorts.Add(newPort.Port, newPort);
                                    logger.Debug($"Registered serialport {newPort.Port}");
                                }

                            

                        }
                    }
                    catch(Exception ex)
                    {
                        logger.Error(ex, "Error on queryObj");
                    }


                }

            }
            catch (ManagementException e)
            {

                //     MessageBox.Show("An error occurred while querying for WMI data: " + e.Message);
            }

            return FoundPorts;

        }

        private static SerialPortInst TryparseQueryObjectToSerialPort(ManagementObject managementObject)
        {

            string friendlyName = (string)managementObject["Name"];
            string port;
            string hardwareID = (string)managementObject["PNPDeviceID"];
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
            switch (managementObject["Service"].ToString())
            {
                case "Serial":
                    return new SerialPortInst(port, friendlyName, hardwareID, SerialPortInst.PortType.Native);
                case "usbser":
                    return new UsbSerialPortInst(port, friendlyName, hardwareID);
                default:
                    return new SerialPortInst(port, friendlyName, hardwareID, SerialPortInst.PortType.Unknown);

            }

        }


    }
}
