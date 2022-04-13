using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using PluginSerialLib;

namespace PluginSerialLib
{
    public class ShellInvoker
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();



        public static Process CreateAndInvokeProcess(string ProcessPath, IList<string> ProcessArguments, string serialport)
        {

            if (ProcessPath == null || string.IsNullOrEmpty(ProcessPath))
                return null;

            string processPathFull = "";
            if (Path.IsPathRooted(ProcessPath))
            {
                processPathFull = Path.GetFullPath(ProcessPath);
            }
            else
            {
                if (ProcessPath[0] == '.') //check if relative path starts with null
                {

                }
                else
                {
                    processPathFull = ProcessPath; //TRIAL to see if on path exes are found
                    //if (!PathExeHelper.FindExeEnvironmentPath(ProcessPath, out processPathFull)) //scan the environment to check if exe on path
                    //{

                    //} 

                }
            }


            StringBuilder sb = new StringBuilder();
            foreach (var arg in ProcessArguments)
            {
                if (arg.Contains(SerialPortRecipe.serialPlaceholderString))
                {
                    sb.Append(" " + arg.Replace(SerialPortRecipe.serialPlaceholderString, serialport));
                }
                else
                {
                    sb.Append(" " + arg);
                }


            }

            string processArguments = sb.ToString();


            logger.Debug($"Starting process on path: \n{ProcessPath} \nArguments: {processArguments}");


            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            startInfo.FileName = processPathFull;
            startInfo.Arguments = processArguments;
            process.StartInfo = startInfo;

            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception on starting process");
            }

            return process;
        }


    }
}
