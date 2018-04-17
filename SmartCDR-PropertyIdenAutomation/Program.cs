using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyIdentication.DAL.Entities;
using PropertyIdentification.BAL;
using PropertyIdentification.Common;

namespace SmartCDR_PropertyIdenAutomation
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var configMap = new ExeConfigurationFileMap();
            configMap.ExeConfigFilename = args[0];
            var config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            PropertyIdentification.BAL.PropertyAction action = new PropertyAction();
            var logger = new NLogger();
            logger.WriteLog("Test", LogLevel.Info);

            action.ProcessStart();
            Console.ReadLine();
        }
    }
}