using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackExchange.Redis.Provider.Common
{
    public class ConfigSectionHelper
    {
        public static Configuration GetConfig(string configPath)
        {
            ExeConfigurationFileMap file = new ExeConfigurationFileMap();
            file.ExeConfigFilename = configPath;
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None);
            return config;
        }
        public static T GetConfig<T>(string configPath, string sectionName) where T:ConfigurationSection,new ()
        {
            var config = GetConfig(configPath);
            var data = config.Sections[sectionName] as T;

            return data;
        }


    }
}
