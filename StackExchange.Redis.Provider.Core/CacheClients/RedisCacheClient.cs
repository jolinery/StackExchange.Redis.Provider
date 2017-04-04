using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis.Provider.Core.Configuration;
using StackExchange.Redis.Provider.Core.Server;
using StackExchange.Redis.KeyspaceIsolation;
using StackExchange.Redis.Provider.Common.Extensions;
using StackExchange.Redis.Provider.Common.Serializers;

namespace StackExchange.Redis.Provider.Core
{
    public class RedisCacheClient : ICacheClient
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly ServerEnumerationStrategy _serverEnumerationStrategy = new ServerEnumerationStrategy();
        /// <summary>
        ///     Return the instance of <see cref="StackExchange.Redis.IDatabase" /> used be ICacheClient implementation
        /// </summary>
        public IDatabase Database { get; private set; }
        /// <summary>
        ///     Initializes a new instance of the <see cref="RedisCacheClient" /> class.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="configuration">The configuration.</param>
        public RedisCacheClient(ISerializers serializer, IRedisCachingConfiguration configuration = null)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }

            if (configuration == null)
            {
                configuration = RedisCachingSectionHandler.GetConfig();
            }

            if (configuration == null)
            {
                throw new ConfigurationErrorsException(
                    "Unable to locate <redisCacheClient> section into your configuration file. Take a look https://github.com/jolinery/StackExchange.Redis.Provider");
            }

            var options = new ConfigurationOptions
            {
                Ssl = configuration.Ssl,
                AllowAdmin = configuration.AllowAdmin,
                Password = configuration.Password,
                AbortOnConnectFail = configuration.AbortOnConnectFail,
                ConnectTimeout = configuration.ConnectTimeout,
            };
            _serverEnumerationStrategy = configuration.ServerEnumerationStrategy;

            foreach (RedisHost redisHost in configuration.RedisHosts)
            {
                options.EndPoints.Add(redisHost.Host, redisHost.CachePort);
            }

            _connectionMultiplexer = ConnectionMultiplexer.Connect(options);
            Database = _connectionMultiplexer.GetDatabase(configuration.Database);

            if (!string.IsNullOrWhiteSpace(configuration.KeyPrefix))
                Database = Database.WithKeyPrefix(configuration.KeyPrefix);

            Serializer = serializer;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RedisCacheClient" /> class.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="keyPrefix">Specifies the key separation prefix to be used for all keys</param>
        /// <exception cref="System.ArgumentNullException">serializer</exception>
        public RedisCacheClient(ISerializers serializer, string connectionString, string keyPrefix)
            : this(serializer, connectionString, 0, keyPrefix)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RedisCacheClient" /> class.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="database">The database.</param>
        /// <param name="keyPrefix">Specifies the key separation prefix to be used for all keys</param>
        /// <exception cref="System.ArgumentNullException">serializer</exception>
        public RedisCacheClient(ISerializers serializer, string connectionString, int database = 0,
            string keyPrefix = null)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }

            Serializer = serializer;
            _connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
            Database = _connectionMultiplexer.GetDatabase(database);

            if (!string.IsNullOrWhiteSpace(keyPrefix))
                Database = Database.WithKeyPrefix(keyPrefix);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RedisCacheClient" /> class.
        /// </summary>
        /// <param name="connectionMultiplexer">The connection multiplexer.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="keyPrefix">Specifies the key separation prefix to be used for all keys</param>
        /// <exception cref="System.ArgumentNullException">
        ///     connectionMultiplexer
        ///     or
        ///     serializer
        /// </exception>
        public RedisCacheClient(IConnectionMultiplexer connectionMultiplexer, ISerializers serializer, string keyPrefix)
            : this(connectionMultiplexer, serializer, 0, keyPrefix)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RedisCacheClient" /> class.
        /// </summary>
        /// <param name="connectionMultiplexer">The connection multiplexer.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="database">The database.</param>
        /// <param name="keyPrefix">Specifies the key separation prefix to be used for all keys</param>
        /// <exception cref="System.ArgumentNullException">
        ///     connectionMultiplexer
        ///     or
        ///     serializer
        /// </exception>
        public RedisCacheClient(IConnectionMultiplexer connectionMultiplexer, ISerializers serializer,
            int database = 0, string keyPrefix = null)
        {
            if (connectionMultiplexer == null)
            {
                throw new ArgumentNullException("connectionMultiplexer");
            }

            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }

            Serializer = serializer;
            this._connectionMultiplexer = connectionMultiplexer;

            Database = connectionMultiplexer.GetDatabase(database);

            if (!string.IsNullOrWhiteSpace(keyPrefix))
                Database = Database.WithKeyPrefix(keyPrefix);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _connectionMultiplexer.Dispose();
        }

        /// <summary>
        ///     Gets the serializer.
        /// </summary>
        /// <value>
        ///     The serializer.
        /// </value>
        public ISerializers Serializer { get; private set;
        }

        /// <summary>
        ///     Verify that the specified cache key exists
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>
        ///     True if the key is present into Redis. Othwerwise False
        /// </returns>
        public bool Exists(string key)
        {
            return Database.KeyExists(key);
        }

        /// <summary>
        ///     Verify that the specified cache key exists
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>
        ///     True if the key is present into Redis. Othwerwise False
        /// </returns>
        public Task<bool> ExistsAsync(string key)
        {
            return Database.KeyExistsAsync(key);
        }

        /// <summary>
        ///     Removes the specified key from Redis Database
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///     True if the key has removed. Othwerwise False
        /// </returns>
        public bool Remove(string key)
        {
            return Database.KeyDelete(key);
        }

        /// <summary>
        ///     Removes the specified key from Redis Database
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///     True if the key has removed. Othwerwise False
        /// </returns>
        public Task<bool> RemoveAsync(string key)
        {
            return Database.KeyDeleteAsync(key);
        }

        /// <summary>
        ///     Removes all specified keys from Redis Database
        /// </summary>
        /// <param name="keys">The key.</param>
        public void RemoveAll(IEnumerable<string> keys)
        {
            keys.ForEach(x => Remove(x));
        }

        /// <summary>
        ///     Removes all specified keys from Redis Database
        /// </summary>
        /// <param name="keys">The key.</param>
        /// <returns></returns>
        public Task RemoveAllAsync(IEnumerable<string> keys)
        {
            return keys.ForEachAsync(RemoveAsync);
        }

        /// <summary>
        ///     Get the object with the specified key from Redis database
        /// </summary>
        /// <typeparam name="T">The type of the expected object</typeparam>
        /// <param name="key">The cache key.</param>
        /// <returns>
        ///     Null if not present, otherwise the instance of T.
        /// </returns>
        public T Get<T>(string key)
        {
            var valueBytes = Database.StringGet(key);

            if (!valueBytes.HasValue)
            {
                return default(T);
            }

            return Serializer.Deserialize<T>(valueBytes);
        }

        /// <summary>
        ///     Get the object with the specified key from Redis database
        /// </summary>
        /// <typeparam name="T">The type of the expected object</typeparam>
        /// <param name="key">The cache key.</param>
        /// <returns>
        ///     Null if not present, otherwise the instance of T.
        /// </returns>
        public async Task<T> GetAsync<T>(string key)
        {
            var valueBytes = await Database.StringGetAsync(key);

            if (!valueBytes.HasValue)
            {
                return default(T);
            }

            return await Serializer.DeserializeAsync<T>(valueBytes);
        }

        /// <summary>
        ///     Sets the specified instance to the Redis database.
        /// </summary>
        /// <typeparam name="T">The type of the class to add to Redis</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The instance of T.</param>
        /// <returns>
        ///     True if the object has been added. Otherwise false
        /// </returns>
        public bool Set<T>(string key, T value)
        {
            var entryBytes = Serializer.Serialize(value);

            return Database.StringSet(key, entryBytes);
        }

        /// <summary>
        ///     Sets the specified instance to the Redis database.
        /// </summary>
        /// <typeparam name="T">The type of the class to add to Redis</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The instance of T.</param>
        /// <returns>
        ///     True if the object has been Seted. Otherwise false
        /// </returns>
        public async Task<bool> SetAsync<T>(string key, T value)
        {
            var entryBytes = await Serializer.SerializeAsync(value);

            return await Database.StringSetAsync(key, entryBytes);
        }

        /// <summary>
        ///     Replaces the object with specified key into Redis database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The instance of T</param>
        /// <returns>
        ///     True if the object has been added. Otherwise false
        /// </returns>
        public bool Replace<T>(string key, T value)
        {
            return Set(key, value);
        }

        /// <summary>
        ///     Replaces the object with specified key into Redis database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The instance of T</param>
        /// <returns>
        ///     True if the object has been added. Otherwise false
        /// </returns>
        public Task<bool> ReplaceAsync<T>(string key, T value)
        {
            return SetAsync(key, value);
        }

        /// <summary>
        ///     Sets the specified instance to the Redis database.
        /// </summary>
        /// <typeparam name="T">The type of the class to Set to Redis</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The instance of T.</param>
        /// <param name="expiresAt">Expiration time.</param>
        /// <returns>
        ///     True if the object has been Seted. Otherwise false
        /// </returns>
        public bool Set<T>(string key, T value, DateTimeOffset expiresAt)
        {
            var entryBytes = Serializer.Serialize(value);
            var expiration = expiresAt.Subtract(DateTimeOffset.Now);

            return Database.StringSet(key, entryBytes, expiration);
        }

        public async Task<bool> SetAsync<T>(string key, T value, DateTimeOffset expiresAt)
        {
            var entryBytes = await Serializer.SerializeAsync(value);
            var expiration = expiresAt.Subtract(DateTimeOffset.Now);

            return await Database.StringSetAsync(key, entryBytes, expiration);
        }

        public bool Replace<T>(string key, T value, DateTimeOffset expiresAt)
        {
            return Set(key, value, expiresAt);
        }

        public Task<bool> ReplaceAsync<T>(string key, T value, DateTimeOffset expiresAt)
        {
            return SetAsync(key, value, expiresAt);
        }

        public bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            var entryBytes = Serializer.Serialize(value);

            return Database.StringSet(key, entryBytes, expiresIn);
        }

        public async Task<bool> SetAsync<T>(string key, T value, TimeSpan expiresIn)
        {
            var entryBytes = await Serializer.SerializeAsync(value);

            return await Database.StringSetAsync(key, entryBytes, expiresIn);
        }

        public bool Replace<T>(string key, T value, TimeSpan expiresIn)
        {
            return Set(key, value, expiresIn);
        }

        public Task<bool> ReplaceAsync<T>(string key, T value, TimeSpan expiresIn)
        {
            return SetAsync(key, value, expiresIn);
        }

        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            var result = Database.StringGet(redisKeys);

            var dict = new Dictionary<string, T>(StringComparer.Ordinal);
            for (var index = 0; index < redisKeys.Length; index++)
            {
                var value = result[index];
                dict.Add(redisKeys[index], value == RedisValue.Null ? default(T) : Serializer.Deserialize<T>(value));
            }

            return dict;
        }

        public async Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys)
        {
            var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            var result = await Database.StringGetAsync(redisKeys);
            var dict = new Dictionary<string, T>(StringComparer.Ordinal);
            for (var index = 0; index < redisKeys.Length; index++)
            {
                var value = result[index];
                dict.Add(redisKeys[index], value == RedisValue.Null ? default(T) : Serializer.Deserialize<T>(value));
            }
            return dict;
        }


        public bool SetAll<T>(IDictionary<string, T> items)
        {
            var values = items
                .Select(item => new KeyValuePair<RedisKey, RedisValue>(item.Key, Serializer.Serialize(item.Value)))
                .ToArray();

            return Database.StringSet(values);
        }

        public async Task<bool> SetAllAsync<T>(IDictionary<string, T> items)
        {
            var values = items
                .Select(item => new KeyValuePair<RedisKey, RedisValue>(item.Key, Serializer.Serialize(item.Value)))
                .ToArray();

            return await Database.StringSetAsync(values);
        }

        public bool SetAdd<T>(string key, T item) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key cannot be empty.", "key");
            }

            if (item == null)
            {
                throw new ArgumentNullException("item", "item cannot be null.");
            }

            var serializedObject = Serializer.Serialize(item);

            return Database.SetAdd(key, serializedObject);
        }

        public async Task<bool> SetAddAsync<T>(string key, T item) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key cannot be empty.", "key");
            }

            if (item == null)
            {
                throw new ArgumentNullException("item", "item cannot be null.");
            }

            var serializedObject = await Serializer.SerializeAsync(item);

            return await Database.SetAddAsync(key, serializedObject);
        }

        public long SetAddAll<T>(string key, params T[] items) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key cannot be empty.", "key");
            }

            if (items == null)
            {
                throw new ArgumentNullException("items", "items cannot be null.");
            }

            if (items.Any((item) => item == null))
            {
                throw new ArgumentException("items cannot contains any null item.", "items");
            }

            return Database.SetAdd(key, items.Select(item => Serializer.Serialize(item)).Select((x) => (RedisValue)x).ToArray());
        }

        public async Task<long> SetAddAllAsync<T>(string key, params T[] items) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(@"key cannot be empty.", "key");
            }

            if (items == null)
            {
                throw new ArgumentNullException("items", @"items cannot be null.");
            }

            if (items.Any((item) => item == null))
            {
                throw new ArgumentException(@"items cannot contains any null item.", "items");
            }

            return await Database.SetAddAsync(key, items.Select(item => Serializer.Serialize(item)).Select((x) => (RedisValue)x).ToArray());
        }

        public bool SetRemove<T>(string key, T item) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(@"key cannot be empty.", "key");
            }

            if (item == null)
            {
                throw new ArgumentNullException("item", @"item cannot be null.");
            }

            var serializedObject = Serializer.Serialize(item);

            return Database.SetRemove(key, serializedObject);
        }

        public async Task<bool> SetRemoveAsync<T>(string key, T item) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(@"key cannot be empty.", "key");
            }

            if (item == null)
            {
                throw new ArgumentNullException("item", @"item cannot be null.");
            }

            var serializedObject = await Serializer.SerializeAsync(item);

            return await Database.SetRemoveAsync(key, serializedObject);
        }

        public long SetRemoveAll<T>(string key, params T[] items) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(@"key cannot be empty.", "key");
            }

            if (items == null)
            {
                throw new ArgumentNullException("items", @"items cannot be null.");
            }

            if (items.Any((item) => item == null))
            {
                throw new ArgumentException(@"items cannot contains any null item.", "items");
            }

            return Database.SetRemove(key, items.Select(item => Serializer.Serialize(item)).Select((x) => (RedisValue)x).ToArray());
        }

        /// <summary>
        ///     Run SREM command http://redis.io/commands/srem
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public async Task<long> SetRemoveAllAsync<T>(string key, params T[] items) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key cannot be empty.", "key");
            }

            if (items == null)
            {
                throw new ArgumentNullException("items", "items cannot be null.");
            }

            if (items.Any((item) => item == null))
            {
                throw new ArgumentException("items cannot contains any null item.", "items");
            }

            return await Database.SetRemoveAsync(key, items.Select(item => Serializer.Serialize(item)).Select((x) => (RedisValue)x).ToArray());
        }

        /// <summary>
        ///     Run SMEMBERS command http://redis.io/commands/SMEMBERS
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns></returns>
        public string[] SetMember(string memberName)
        {
            return Database.SetMembers(memberName).Select(x => x.ToString()).ToArray();
        }

        /// <summary>
        ///     Run SMEMBERS command see http://redis.io/commands/SMEMBERS
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns></returns>
        public async Task<string[]> SetMemberAsync(string memberName)
        {
            return (await Database.SetMembersAsync(memberName)).Select(x => x.ToString()).ToArray();
        }

        /// <summary>
        ///     Run SMEMBERS command see http://redis.io/commands/SMEMBERS
        ///     Deserializes the results to T
        /// </summary>
        /// <typeparam name="T">The type of the expected objects in the set</typeparam>
        /// <param name="key">The key</param>
        /// <returns>An array of objects in the set</returns>
        public IEnumerable<T> SetMembers<T>(string key)
        {
            var members = Database.SetMembers(key);
            return members.Select(m => m == RedisValue.Null ? default(T) : Serializer.Deserialize<T>(m));
        }

        /// <summary>
        ///     Run SMEMBERS command see http://redis.io/commands/SMEMBERS
        ///     Deserializes the results to T
        /// </summary>
        /// <typeparam name="T">The type of the expected objects</typeparam>
        /// <param name="key">The key</param>
        /// <returns>An array of objects in the set</returns>
        public async Task<IEnumerable<T>> SetMembersAsync<T>(string key)
        {
            var members = await Database.SetMembersAsync(key);

            return members.Select(m => m == RedisValue.Null ? default(T) : Serializer.Deserialize<T>(m));
        }

        public IEnumerable<string> SearchKeys(string pattern)
        {
            var keys = new HashSet<RedisKey>();

            var multiplexer = Database.Multiplexer;
            var servers = ServerIteratorFactory.GetServers(multiplexer, _serverEnumerationStrategy).ToArray();
            if (!servers.Any())
            {
                throw new Exception("No server found to serve the KEYS command.");
            }

            foreach (var server in servers)
            {
                var dbKeys = server.Keys(Database.Database, pattern);
                foreach (var dbKey in dbKeys)
                {
                    if (!keys.Contains(dbKey))
                    {
                        keys.Add(dbKey);
                    }
                }
            }

            return keys.Select(x => (string)x);
        }

        public Task<IEnumerable<string>> SearchKeysAsync(string pattern)
        {
            return Task.Factory.StartNew(() => SearchKeys(pattern));
        }

        /// <summary>
        ///     Flushes the database.
        /// </summary>
        public void FlushDb()
        {
            var endPoints = Database.Multiplexer.GetEndPoints();

            foreach (var endpoint in endPoints)
            {
                Database.Multiplexer.GetServer(endpoint).FlushDatabase(Database.Database);
            }
        }

        /// <summary>
        ///     Flushes the database asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task FlushDbAsync()
        {
            var endPoints = Database.Multiplexer.GetEndPoints();

            foreach (var endpoint in endPoints)
            {
                await Database.Multiplexer.GetServer(endpoint).FlushDatabaseAsync(Database.Database);
            }
        }

        /// <summary>
        ///     Save the DB in background.
        /// </summary>
        /// <param name="saveType"></param>
        public void Save(SaveType saveType)
        {
            var endPoints = Database.Multiplexer.GetEndPoints();

            foreach (var endpoint in endPoints)
            {
                Database.Multiplexer.GetServer(endpoint).Save(saveType);
            }
        }

        /// <summary>
        ///     Save the DB in background asynchronous.
        /// </summary>
        /// <param name="saveType"></param>
        public async void SaveAsync(SaveType saveType)
        {
            var endPoints = Database.Multiplexer.GetEndPoints();

            foreach (var endpoint in endPoints)
            {
                await Database.Multiplexer.GetServer(endpoint).SaveAsync(saveType);
            }
        }

        /// <summary>
        ///     Gets the information about redis.
        ///     More info see http://redis.io/commands/INFO
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetInfo()
        {
            var info = Database.ScriptEvaluate("return redis.call('INFO')").ToString();

            return ParseInfo(info);
        }

        /// <summary>
        ///     Gets the information about redis.
        ///     More info see http://redis.io/commands/INFO
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetInfoAsync()
        {
            var info = (await Database.ScriptEvaluateAsync("return redis.call('INFO')")).ToString();

            return ParseInfo(info);
        }

        /// <summary>
        ///     Publishes a message to a channel.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public long Publish<T>(RedisChannel channel, T message, CommandFlags flags = CommandFlags.None)
        {
            var sub = _connectionMultiplexer.GetSubscriber();
            return sub.Publish(channel, Serializer.Serialize(message), flags);
        }

        /// <summary>
        ///     Publishes a message to a channel.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<long> PublishAsync<T>(RedisChannel channel, T message, CommandFlags flags = CommandFlags.None)
        {
            var sub = _connectionMultiplexer.GetSubscriber();
            return await sub.PublishAsync(channel, await Serializer.SerializeAsync(message), flags);
        }

        /// <summary>
        ///     Registers a callback handler to process messages published to a channel.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="handler"></param>
        /// <param name="flags"></param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void Subscribe<T>(RedisChannel channel, Action<T> handler, CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            var sub = _connectionMultiplexer.GetSubscriber();
            sub.Subscribe(channel, (redisChannel, value) => handler(Serializer.Deserialize<T>(value)), flags);
        }

        /// <summary>
        ///     Registers a callback handler to process messages published to a channel.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="handler"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async Task SubscribeAsync<T>(RedisChannel channel, Func<T, Task> handler,
            CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            var sub = _connectionMultiplexer.GetSubscriber();
            await
                sub.SubscribeAsync(channel, async (redisChannel, value) => await handler(Serializer.Deserialize<T>(value)), flags);
        }

        /// <summary>
        ///     Unregisters a callback handler to process messages published to a channel.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="handler"></param>
        /// <param name="flags"></param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void Unsubscribe<T>(RedisChannel channel, Action<T> handler, CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            var sub = _connectionMultiplexer.GetSubscriber();
            sub.Unsubscribe(channel, (redisChannel, value) => handler(Serializer.Deserialize<T>(value)), flags);
        }

        /// <summary>
        ///     Unregisters a callback handler to process messages published to a channel.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="handler"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async Task UnsubscribeAsync<T>(RedisChannel channel, Func<T, Task> handler,
            CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            var sub = _connectionMultiplexer.GetSubscriber();
            await sub.UnsubscribeAsync(channel, (redisChannel, value) => handler(Serializer.Deserialize<T>(value)), flags);
        }

        /// <summary>
        ///     Unregisters all callback handlers on a channel.
        /// </summary>
        /// <param name="flags"></param>
        public void UnsubscribeAll(CommandFlags flags = CommandFlags.None)
        {
            var sub = _connectionMultiplexer.GetSubscriber();
            sub.UnsubscribeAll(flags);
        }

        /// <summary>
        ///     Unregisters all callback handlers on a channel.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None)
        {
            var sub = _connectionMultiplexer.GetSubscriber();
            await sub.UnsubscribeAllAsync(flags);
        }

        /// <summary>
        ///     Insert the specified value at the head of the list stored at key. If key does not exist, it is created as empty
        ///     list before performing the push operations.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <returns>
        ///     the length of the list after the push operations.
        /// </returns>
        /// <exception cref="System.ArgumentException">key cannot be empty.;key</exception>
        /// <exception cref="System.ArgumentNullException">item;item cannot be null.</exception>
        /// <remarks>
        ///     http://redis.io/commands/lpush
        /// </remarks>
        public long ListAddToLeft<T>(string key, T item) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key cannot be empty.", "key");
            }

            if (item == null)
            {
                throw new ArgumentNullException("item", "item cannot be null.");
            }

            var serializedItem = Serializer.Serialize(item);

            return Database.ListLeftPush(key, serializedItem);
        }

        /// <summary>
        ///     Lists the add to left asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">key cannot be empty.;key</exception>
        /// <exception cref="System.ArgumentNullException">item;item cannot be null.</exception>
        public async Task<long> ListAddToLeftAsync<T>(string key, T item) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key cannot be empty.", "key");
            }

            if (item == null)
            {
                throw new ArgumentNullException("item", "item cannot be null.");
            }

            var serializedItem = await Serializer.SerializeAsync(item);

            return await Database.ListLeftPushAsync(key, serializedItem);
        }

        /// <summary>
        ///     Removes and returns the last element of the list stored at key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">key cannot be empty.;key</exception>
        /// <remarks>
        ///     http://redis.io/commands/rpop
        /// </remarks>
        public T ListGetFromRight<T>(string key) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key cannot be empty.", "key");
            }

            var item = Database.ListRightPop(key);

            return item == RedisValue.Null ? null : Serializer.Deserialize<T>(item);
        }

        /// <summary>
        ///     Removes and returns the last element of the list stored at key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">key cannot be empty.;key</exception>
        /// <remarks>
        ///     http://redis.io/commands/rpop
        /// </remarks>
        public async Task<T> ListGetFromRightAsync<T>(string key) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key cannot be empty.", "key");
            }

            var item = await Database.ListRightPopAsync(key);

            if (item == RedisValue.Null) return null;

            return item == RedisValue.Null ? null : await Serializer.DeserializeAsync<T>(item);
        }

        private Dictionary<string, string> ParseInfo(string info)
        {
            var lines = info.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var data = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line) || line[0] == '#')
                {
                    // 2.6+ can have empty lines, and comment lines
                    continue;
                }

                var idx = line.IndexOf(':');
                if (idx > 0) // double check this line looks about right
                {
                    var key = line.Substring(0, idx);
                    var infoValue = line.Substring(idx + 1).Trim();

                    data.Add(key, infoValue);
                }
            }

            return data;
        }

        public bool HashDelete(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashDelete(hashKey, key, commandFlags);
        }

        public long HashDelete(string hashKey, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashDelete(hashKey, keys.Select(x => (RedisValue)x).ToArray(), commandFlags);
        }

        public bool HashExists(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashExists(hashKey, key, commandFlags);
        }

        public T HashGet<T>(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisValue = Database.HashGet(hashKey, key, commandFlags);
            return redisValue.HasValue ? Serializer.Deserialize<T>(redisValue) : default(T);
        }

        public Dictionary<string, T> HashGet<T>(string hashKey, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None)
        {
            return keys.Select(x => new { key = x, value = HashGet<T>(hashKey, x, commandFlags) })
                        .ToDictionary(kv => kv.key, kv => kv.value, StringComparer.Ordinal);
        }

        public Dictionary<string, T> HashGetAll<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database
                        .HashGetAll(hashKey, commandFlags)
                        .ToDictionary(
                            x => x.Name.ToString(),
                            x => Serializer.Deserialize<T>(x.Value),
                            StringComparer.Ordinal);
        }

        public long HashIncerement(string hashKey, string key, long value, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashIncrement(hashKey, key, value, commandFlags);
        }

        public double HashIncerement(string hashKey, string key, double value, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashIncrement(hashKey, key, value, commandFlags);
        }

        public IEnumerable<string> HashKeys(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashKeys(hashKey, commandFlags).Select(x => x.ToString());
        }

        public long HashLength(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashLength(hashKey, commandFlags);
        }

        public bool HashSet<T>(string hashKey, string key, T value, bool nx = false, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashSet(hashKey, key, Serializer.Serialize(value), nx ? When.NotExists : When.Always, commandFlags);
        }

        public void HashSet<T>(string hashKey, Dictionary<string, T> values, CommandFlags commandFlags = CommandFlags.None)
        {
            var entries = values.Select(kv => new HashEntry(kv.Key, Serializer.Serialize(kv.Value)));
            Database.HashSet(hashKey, entries.ToArray(), commandFlags);
        }

        public IEnumerable<T> HashValues<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashValues(hashKey, commandFlags).Select(x => Serializer.Deserialize<T>(x));
        }

        public Dictionary<string, T> HashScan<T>(string hashKey, string pattern, int pageSize = 10, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database
                         .HashScan(hashKey, pattern, pageSize, commandFlags)
                         .ToDictionary(x => x.Name.ToString(),
                                      x => Serializer.Deserialize<T>(x.Value),
                                      StringComparer.Ordinal);
        }

        public async Task<bool> HashDeleteAsync(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
        {
            return await Database.HashDeleteAsync(hashKey, key, commandFlags);
        }

        public async Task<long> HashDeleteAsync(string hashKey, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None)
        {
            return await Database.HashDeleteAsync(hashKey, keys.Select(x => (RedisValue)x).ToArray(), commandFlags);
        }

        public async Task<bool> HashExistsAsync(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
        {
            return await Database.HashExistsAsync(hashKey, key, commandFlags);
        }

        public async Task<T> HashGetAsync<T>(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisValue = await Database.HashGetAsync(hashKey, key, commandFlags);
            return redisValue.HasValue ? Serializer.Deserialize<T>(redisValue) : default(T);
        }

        public async Task<Dictionary<string, T>> HashGetAsync<T>(string hashKey, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None)
        {
            var result = new Dictionary<string, T>();
            foreach (var key in keys)
            {
                var value = await HashGetAsync<T>(hashKey, key, commandFlags);

                result.Add(key, value);
            }

            return result;
        }


        public async Task<Dictionary<string, T>> HashGetAllAsync<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return (await Database
                        .HashGetAllAsync(hashKey, commandFlags))
                        .ToDictionary(
                            x => x.Name.ToString(),
                            x => Serializer.Deserialize<T>(x.Value),
                            StringComparer.Ordinal);
        }


        public async Task<long> HashIncerementByAsync(string hashKey, string key, long value, CommandFlags commandFlags = CommandFlags.None)
        {
            return await Database.HashIncrementAsync(hashKey, key, value, commandFlags);
        }

        public async Task<double> HashIncerementByAsync(string hashKey, string key, double value, CommandFlags commandFlags = CommandFlags.None)
        {
            return await Database.HashIncrementAsync(hashKey, key, value, commandFlags);
        }


        public async Task<IEnumerable<string>> HashKeysAsync(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return (await Database.HashKeysAsync(hashKey, commandFlags)).Select(x => x.ToString());
        }

        public async Task<long> HashLengthAsync(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return await Database.HashLengthAsync(hashKey, commandFlags);
        }

        public async Task<bool> HashSetAsync<T>(string hashKey, string key, T value, bool nx = false, CommandFlags commandFlags = CommandFlags.None)
        {
            return await Database.HashSetAsync(hashKey, key, Serializer.Serialize(value), nx ? When.NotExists : When.Always, commandFlags);
        }

        public async Task HashSetAsync<T>(string hashKey, IDictionary<string, T> values, CommandFlags commandFlags = CommandFlags.None)
        {
            var entries = values.Select(kv => new HashEntry(kv.Key, Serializer.Serialize(kv.Value)));
            await Database.HashSetAsync(hashKey, entries.ToArray(), commandFlags);
        }

        public async Task<IEnumerable<T>> HashValuesAsync<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return (await Database.HashValuesAsync(hashKey, commandFlags)).Select(x => Serializer.Deserialize<T>(x));
        }

        public async Task<Dictionary<string, T>> HashScanAsync<T>(string hashKey, string pattern, int pageSize = 10, CommandFlags commandFlags = CommandFlags.None)
        {
            return (await Task.Run(() => Database.HashScan(hashKey, pattern, pageSize, commandFlags)))
                .ToDictionary(x => x.Name.ToString(), x => Serializer.Deserialize<T>(x.Value), StringComparer.Ordinal);
        }

    }
}
