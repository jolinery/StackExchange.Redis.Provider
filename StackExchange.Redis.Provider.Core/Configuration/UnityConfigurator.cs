using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;
using StackExchange.Redis.Provider.Common;

namespace StackExchange.Redis.Provider.Core.Configuration
{
    public class UnityConfigurator
    {
        /// <summary>
        /// Create Container
        /// </summary>
        /// <returns></returns>
        public static IUnityContainer CreateContainer()
        {
            var section = ConfigSectionHelper.GetConfig<UnityConfigurationSection>(ConfigurationManager.AppSettings["RedisConfigPath"], "unity");
            IUnityContainer container = new UnityContainer().LoadConfiguration(section);
            container.LoadConfiguration(section);
            return container;
        }
    }
}
