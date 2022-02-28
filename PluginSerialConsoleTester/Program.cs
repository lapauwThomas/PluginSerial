using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using PluginSerialLib;

namespace PluginSerialConsoleTester
{
    class Program
    {


        private static Logger logger = LogManager.GetCurrentClassLogger();


        static void Main(string[] args)
        {

            logger.Info("Starting PluginSerial");


            var portnames = System.IO.Ports.SerialPort.GetPortNames();
            foreach (var port in portnames)
            {
                logger.Trace($"Port found {port}");
            }
            Thread thr = new Thread(f => { SerialPortSearcher.FindSerialPortsWMI(); });
            thr.Start();
            //SerialPortSearcher.FindSerialPortsWMI();




        }
    }
}
