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

        /// <summary>
        /// Create a display string by checking if the string contains the COMxx designator. if not, append
        /// </summary>
        public string DisplayString
        {
            get
            {
                string normalized1 = Regex.Replace(FriendlyName, @"\s", "");

                if (normalized1.Contains(Port))
                {
                    return FriendlyName;
                }
                else
                {
                    return $"{FriendlyName} - ({Port})";
                }

            }
        }



        public bool valid { get; protected set; } = true;

        public bool IsDisconnected { get; protected set; } = false;

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

        public Action OnDisconnect;

        /// <summary>
        /// To be called to mark the port as disconnected
        /// </summary>
        public void PortDisconnected()
        {
            OnDisconnect?.Invoke();
            IsDisconnected = true;
        }
    }

    public class UsbSerialPortInst : SerialPortInst
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public readonly string VID;
        public readonly string PID;

        public UsbSerialPortInst(string port, string friendlyName, string hardwareID, string instancePath) : base(port, friendlyName, hardwareID, instancePath, PortType.USB)
        {
            if (!usbserParseDeviceId(hardwareID, out VID, out PID)) valid = false;
            valid = true;

        }

        private static bool usbserParseDeviceId(string deviceID, out string VID, out string PID)
        {

            VID = null;
            PID = null;

            foreach (string pattern in ConfigManager.USBVIDPIDRegexList)
            {
                Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);
                var match = _rx.Match(deviceID.ToUpperInvariant());
                if (match.Success)
                {
                    try
                    {
                        string VIDcapture = match.Groups[1].Value;
                        string PIDcapture = match.Groups[2].Value;

                        //check if valid VID_PID combo by parsing. Should be nice hex numbers
                        if (!int.TryParse(VIDcapture, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _)) continue;

                        if (!int.TryParse(PIDcapture, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _)) continue;

                        VID = VIDcapture;
                        PID = PIDcapture;

                        logger.Debug($"Parsed HWID for VID [{VID}] PID [{PID}]");
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
