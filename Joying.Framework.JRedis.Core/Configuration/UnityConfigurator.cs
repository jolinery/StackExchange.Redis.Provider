using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Joying.Framework.Serializers;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;

namespace Joying.Framework.JRedis.Core.Configuration
{
    public class UnityConfigurator
    {
        public static IUnityContainer CreateContainer()
        {
            var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            IUnityContainer container = new UnityContainer().LoadConfiguration(section);
                container.LoadConfiguration(section);
            return container;
        }
    }
}
