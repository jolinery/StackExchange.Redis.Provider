using Microsoft.Practices.Unity;
using StackExchange.Redis.Provider.Common.Serializers;
using StackExchange.Redis.Provider.Core.Configuration;

namespace StackExchange.Redis.Provider.Core
{
    public class FactoryProvider
    {
        public class RedisProviderFactory
        {
            private static ICacheClient _instance;

            private static readonly object LockObject = new object();

            public static ICacheClient GetRedisClientProvider()
            {
                var container = UnityConfigurator.CreateContainer();

                IRedisCachingConfiguration config = RedisCachingSectionHandler.GetConfig();
                var sectionSerializerType = "newtonsoft";
                if (config.SerializerType != null)
                {
                    sectionSerializerType = config.SerializerType;
                }
                var serializer = container.Resolve<ISerializers>(sectionSerializerType);
                if (_instance == null)
                {
                    lock (LockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new RedisCacheClient(serializer, config);
                        }
                    }
                }
                return _instance;
            }

        }
    }
}
