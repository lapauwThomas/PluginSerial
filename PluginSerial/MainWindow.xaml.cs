using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NLog;
using PluginSerial;

namespace PluginSerialFW
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public MainWindow()
        {

            logger.Info("Starting PluginSerial");
            InitializeComponent();

            var portnames = System.IO.Ports.SerialPort.GetPortNames();
            foreach (var port in portnames)
            {
                logger.Trace($"Port found {port}");
            }
            Thread thr = new Thread(f=> { SerialPortSearcher.FindSerialPortsWMI(); });
            thr.Start();
            //SerialPortSearcher.FindSerialPortsWMI();
        }
    }
}
