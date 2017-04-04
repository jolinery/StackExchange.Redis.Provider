using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis.Provider.Core;

namespace StackExchange.Redis.Provider.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            
            var redisProvider = FactoryProvider.RedisProviderFactory.GetRedisClientProvider();
            redisProvider.Set("testRedis", "testRedisvalue",new TimeSpan(0,0,0,30));
            redisProvider.Set("testRedis", "testRedisvalue");
            var getString = redisProvider.Get<string>("testRedis");
            Console.WriteLine(getString);
        }
    }
}
