using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NLog;

namespace PluginSerialLib
{
    public class SerialPortInst
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public enum PortType
        {
            Unknown,
            Native,
            USB,
            Virtual
        }
        public readonly int PortNumber = -1;
        public readonly string Port;
        public readonly string FriendlyName;
        public readonly string HardwareID;
        public readonly string InstancePath;
        public readonly PortType Type;

       
        public bool valid { get; protected set; } = true;

        public SerialPortInst(string port, string friendlyName, string hardwareID, string instancePath, PortType type)
        {
            Port = port;
            FriendlyName = friendlyName;
            HardwareID = hardwareID;
            InstancePath = instancePath;
            Type = type;
            if (!int.TryParse(GetNumbers(port), out PortNumber)) valid = false;


            logger.Debug($"Created Serial port {Port} for of type {type.ToString()} at path [{instancePath}] { (valid? "" : "INVALID")}");
        }


        private static string GetNumbers(string input)
        {
            return new string(input.Where(c => char.IsDigit(c)).ToArray());
        }
    }

    public class UsbSerialPortInst : SerialPortInst
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public readonly int VID;
        public readonly int PID;
        public readonly string InstancePath;

        public UsbSerialPortInst(string port, string friendlyName, string hardwareID, string instancePath) : base(port, friendlyName, hardwareID, instancePath, PortType.USB)
        {
            if (!usbserParseDeviceId(hardwareID, out VID, out PID)) valid = false;
            valid = true;

        }

        private static bool usbserParseDeviceId(string deviceID, out int VID, out int PID)
        {

            VID = 0;
            PID = 0;

            foreach (string pattern in ConfigManager.USBVIDPIDRegexList)
            {
                Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);
                var match = _rx.Match(deviceID.ToUpperInvariant());
                if (match.Success)
                {
                    try
                    {
                        string VIDcapture = match.Groups[1].Value;

                        if (!int.TryParse(VIDcapture, NumberStyles.HexNumber,
                            CultureInfo.InvariantCulture, out VID)) continue;


                        string PIDcapture = match.Groups[2].Value;
                        if (!int.TryParse(PIDcapture, NumberStyles.HexNumber,
                            CultureInfo.InvariantCulture, out PID)) continue;

                        logger.Debug($"Parsed HWID for VID [{VID:X4}] PID [{PID:X4}]");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "Error parsing USB serial HardwareID");
                    }
                }

            }

            return false;
        }

    }
}
