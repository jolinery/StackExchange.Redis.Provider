using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Joying.Framework.JRedis.Core;
using Joying.Framework.JRedis.Core.Configuration;
using Joying.Framework.Serializers;
using Joying.Mall.ThoseQiuService.Common;
using StackExchange.Redis;

namespace ConsoleJRedis
{
    class Program
    {
        static void Main(string[] args)
        {
            ISerializers serializer = new Newtonsofter();
            IRedisCachingConfiguration config = ConfigSectionHelper.GetConfig<RedisCachingSectionHandler>(
                ConfigurationManager.AppSettings["JRedisConfigPath"], "redisCacheClient");
            ICacheClient client = new RedisCacheClient(serializer, config);
            IDatabase database = client.Database;
            client.Set("testJRedis", "testJredisvalue");
            var getString = client.Get<string>("testJRedis");
            Console.WriteLine(getString);
           
        }
    }
}
