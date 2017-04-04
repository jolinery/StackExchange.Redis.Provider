using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis.Provider.Common;

namespace StackExchange.Redis.Provider.Core.Configuration
{
    [Serializable]
    public class RedisCachingSectionHandler : ConfigurationSection, IRedisCachingConfiguration
    {
        public RedisCachingSectionHandler() :base()
        {


        }

        /// <summary>
        /// The host of Redis Server
        /// </summary>
        /// <value>
        /// The ip or name
        /// </value>
        [ConfigurationProperty("hosts")]
        [ConfigurationCollection(typeof(RedisHostCollection), AddItemName = "add")]
        public RedisHostCollection RedisHosts
        {
            get
            {
                return this["hosts"] as RedisHostCollection;
            }
            
        }

        /// <summary>
        /// The strategy to use when executing server wide commands
        /// </summary>
        [ConfigurationProperty("serverEnumerationStrategy")]
        public ServerEnumerationStrategy ServerEnumerationStrategy
        {
            get
            {
                return this["serverEnumerationStrategy"] as ServerEnumerationStrategy;
            }
        }
		

		/// <summary>
		/// Specify if the connection can use Admin commands like flush database
		/// </summary>
		/// <value>
		///   <c>true</c> if can use admin commands; otherwise, <c>false</c>.
		/// </value>
		[ConfigurationProperty("allowAdmin")]
		public bool AllowAdmin
		{
			get
			{
			    string value = null;
			    if (this["allowAdmin"] != null)
			    {
			        value = this["allowAdmin"].ToString();
			    }
                bool result;
                return !string.IsNullOrEmpty(value) && bool.TryParse(value, out result) && result;
			}
		}

		/// <summary>
		/// Specify if the connection is a secure connection or not.
		/// </summary>
		/// <value>
		///   <c>true</c> if is secure; otherwise, <c>false</c>.
		/// </value>
		[ConfigurationProperty("ssl")]
		public bool Ssl
		{
			get
			{
                string value = null;
			    if (this["ssl"] != null)
			    {
			        value = this["ssl"].ToString();
			    }

                bool result;
                return !string.IsNullOrWhiteSpace(value) && bool.TryParse(value, out result) && result;
			}
		}

		/// <summary>
		/// The connection timeout
		/// </summary>
		[ConfigurationProperty("connectTimeout")]
		public int ConnectTimeout
		{
			get
			{
                string value = null;
			    if (this["connectTimeout"] != null)
			    {
			        value = this["connectTimeout"].ToString();
			    }
                int result;
			    return !string.IsNullOrWhiteSpace(value) && int.TryParse(value, out result) ? result : 5000;
			}
		}

		/// <summary>
		/// If true, Connect will not create a connection while no servers are available
		/// </summary>
		[ConfigurationProperty("abortOnConnectFail")]
		public bool AbortOnConnectFail
		{
			get
			{
                string value = null;
			    if (this["abortOnConnectFail"] != null)
			    {
			        value = this["abortOnConnectFail"].ToString();
			    }
                bool result;
			    return !string.IsNullOrWhiteSpace(value) && bool.TryParse(value, out result) && result;
			}
		}

		/// <summary>
		/// Database Id
		/// </summary>
		/// <value>
		/// The database id, the default value is 0
		/// </value>
		[ConfigurationProperty("database")]
		public int Database
		{
			get
			{
                string value = null;
			    if (this["database"] != null)
			    {
			        value = this["database"].ToString();
			    }
			    int result;
			    return !string.IsNullOrWhiteSpace(value) && int.TryParse(value, out result) ? result : 0;
			}
		}

        /// <summary>
        /// The password or access key
        /// </summary>
        [ConfigurationProperty("password", IsRequired = false)]
        public string Password
        {
            get
            {
                return this["password"] as string;       
            }
        }

        /// <summary>
        /// The key separation prefix used for all cache entries
        /// </summary>
        [ConfigurationProperty("keyprefix", IsRequired = false)]
        public string KeyPrefix
        {
            get
            {
                return this["keyprefix"] as string;
            }
        }
        /// <summary>
        /// The serializing structured data
        /// </summary>
        [ConfigurationProperty("serializerType", IsRequired = false)]
        public string SerializerType
        {
            get
            {
                return this["serializerType"] as string;
            }
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <returns></returns>
        public static RedisCachingSectionHandler GetConfig()
        {
            return ConfigSectionHelper.GetConfig<RedisCachingSectionHandler>(
                ConfigurationManager.AppSettings["RedisConfigPath"], "redisCacheClient");
        }
    }
}
