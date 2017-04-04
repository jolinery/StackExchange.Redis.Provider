using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Joying.Framework.Serializers;
using StackExchange.Redis;

namespace Joying.Framework.JRedis.Core
{
    /// <summary>
    /// Redis接口
    /// </summary>
    public interface ICacheClient : IDisposable
    {
        /// <summary>
        /// Return the instance of <see cref="StackExchange.Redis.IDatabase"/> used be ICacheClient implementation
        /// </summary>
        IDatabase Database { get; }
        /// <summary>
        /// Serializer
        /// </summary>
        ISerializers Serializer { get; }
        /// <summary>
        /// 校验Cache key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Exists(string key);
        /// <summary>
        /// 校验Cache key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(string key);
        /// <summary>
        /// 移除Redis数据库的key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Remove(string key);
        /// <summary>
        /// 移除Redis数据库的key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> RemoveAsync(string key);

        /// <summary>
        /// 移除多个redis数据库的key
        /// </summary>
        /// <param name="key"></param>
        void RemoveAll(IEnumerable<string> key);
        /// <summary>
        /// 移除多个redis数据库的key
        /// </summary>
        /// <param name="key"></param>
        Task RemoveAllAsync(IEnumerable<string> key);

        /// <summary>
        /// 获取value值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key);
        /// <summary>
        /// 获取一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string key);
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Set<T>(string key, T value);

        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresTime">过期到点时间</param>
        /// <returns></returns>
        bool Set<T>(string key, T value, DateTimeOffset expiresTime);
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresTime">过期到点时间</param>
        /// <returns></returns>
        Task<bool> SetAsync<T>(string key, T value, DateTimeOffset expiresTime);
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresSpan">过期间隔时间</param>
        /// <returns></returns>
        bool Set<T>(string key, T value, TimeSpan expiresSpan);
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresSpan">过期间隔时间</param>
        /// <returns></returns>
        Task<bool> SetAsync<T>(string key, T value, TimeSpan expiresSpan);
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresSpan">过期间隔时间</param>
        /// <returns></returns>
        Task<bool> SetAsync<T>(string key, T value);
        /// <summary>
        /// 替换一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Replace<T>(string key, T value);

        /// <summary>
        /// 替换一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> ReplaceAsync<T>(string key, T value);
        /// <summary>
        /// 替换一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresTime">过期到点时间</param>
        /// <returns></returns>
        bool Replace<T>(string key, T value, DateTimeOffset expiresTime);
        /// <summary>
        /// 替换一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresTime">过期到点时间</param>
        /// <returns></returns>
        Task<bool> ReplaceAsync<T>(string key, T value, DateTimeOffset expiresTime);
        /// <summary>
        /// 替换一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresSpan">过期间隔时间</param>
        /// <returns></returns>
        bool Replace<T>(string key, T value, TimeSpan expiresSpan);
        /// <summary>
        /// 替换一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresSpan">过期间隔时间</param>
        /// <returns></returns>
        Task<bool> ReplaceAsync<T>(string key, T value, TimeSpan expiresSpan);
        /// <summary>
        /// 批量获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        IDictionary<string, T> GetAll<T>(IEnumerable<string> keys);
        /// <summary>
        /// 批量获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys);
        /// <summary>
        /// 批量添加或更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        bool SetAll<T>(IDictionary<string, T> items);
        /// <summary>
        /// 批量添加或更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        Task<bool> SetAllAsync<T>(IDictionary<string, T> items);
        /// <summary>
        /// 将一个或多个 member 元素加入到集合 key 当中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        bool SetAdd<T>(string key, T item) where T : class;
        /// <summary>
        /// 将一个member 元素加入到集合 key 当中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<bool> SetAddAsync<T>(string key, T item) where T : class;

        /// <summary>
        /// 将多个 member 元素加入到集合 key 当中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        long SetAddAll<T>(string key, params T[] items) where T : class;


        /// <summary>
        /// 将多个 member 元素加入到集合 key 当中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        Task<long> SetAddAllAsync<T>(string key, params T[] items) where T : class;
        /// <summary>
        /// 移除Set集合 key 中的一个ember 元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        bool SetRemove<T>(string key, T item) where T : class;
        /// <summary>
        /// 移除Set集合 key 中的一个ember 元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<bool> SetRemoveAsync<T>(string key, T item) where T : class;

        /// <summary>
        /// 移除Set集合 key 中的多个 member 元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        long SetRemoveAll<T>(string key, params T[] items) where T : class;
        /// <summary>
        /// 移除Set集合 key 中的多个 member 元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        Task<long> SetRemoveAllAsync<T>(string key, params T[] items) where T : class;
        /// <summary>
        /// 获取Set集合key中的member元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string[] SetMember(string key);
        /// <summary>
        /// 获取Set集合key中的member元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<string[]> SetMemberAsync(string key);
        /// <summary>
        ///  在redis中搜索key
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        IEnumerable<string> SearchKeys(string pattern);

        /// <summary>
        ///  在redis中搜索key
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> SearchKeysAsync(string pattern);
        /// <summary>
        /// 刷新DB
        /// </summary>
        void FlushDb();
        /// <summary>
        /// 刷新DB
        /// </summary>
        /// <returns></returns>
        Task FlushDbAsync();
        /// <summary>
        ///  执行一个将当前 Redis 实例的所有数据快照保存操作
        /// </summary>
        /// <param name="saveType"></param>
        void Save(SaveType saveType);
        /// <summary>
        /// 执行一个将当前 Redis 实例的所有数据快照保存操作
        /// </summary>
        /// <param name="saveType"></param>
        void SaveAsync(SaveType saveType);
        /// <summary>
        /// 获取Redis 服务器的各种信息和统计数值
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetInfo();
        /// <summary>
        /// 获取Redis 服务器的各种信息和统计数值
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<string, string>> GetInfoAsync();
        /// <summary>
        ///添加一条数据到list中,顺序从左到右
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long ListAddToLeft<T>(string key, T item) where T : class;
        /// <summary>
        /// 添加一条数据到list中,顺序从左到右
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<long> ListAddToLeftAsync<T>(string key, T item) where T : class;
        /// <summary>
        /// 添加一条数据到list中,顺序从右到左
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T ListGetFromRight<T>(string key) where T : class;
        /// <summary>
        /// 添加一条数据到list中,顺序从右到左
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> ListGetFromRightAsync<T>(string key) where T : class;
        /// <summary>
        /// 删除hash表中hashkey下的一条记录
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        bool HashDelete(string key, string hashField, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 删除hash表中hashkey下的多条记录
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashFields"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        long HashDelete(string key, IEnumerable<string> hashFields, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 是否在hash表存在hashkey的数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        bool HashExists(string key, string hashField, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 获取hash表中一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        T HashGet<T>(string key, string hashField, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 获取hash表中一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="keys"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        Dictionary<string, T> HashGet<T>(string key, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 获取hash表中的多条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        Dictionary<string, T> HashGetAll<T>(string key, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 增加hash表中的一条记录
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <param name="value"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        long HashIncerement(string key, string hashField, long value, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 增加hash表中的一条记录
        /// <remarks>redis version>= 2.6.0</remarks>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <param name="value"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        double HashIncerement(string key, string hashField, double value, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 返回hash中key的域名
        /// </summary>
        /// <param name="key"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        IEnumerable<string> HashKeys(string key, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 获取hash中key的域条数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        long HashLength(string key, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 根据key和hashField更新或添加域的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <param name="value"></param>
        /// <param name="nx"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        bool HashSet<T>(string key, string hashField, T value, bool nx = false, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 根据key更新或添加所有域的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <param name="commandFlags"></param>
        void HashSet<T>(string key, Dictionary<string, T> values, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 获取key的所有值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        IEnumerable<T> HashValues<T>(string key, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 遍历hash类型及其关联的值的字段。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashKey"></param>
        /// <param name="pattern"></param>
        /// <param name="pageSize"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        Dictionary<string, T> HashScan<T>(string hashKey, string pattern, int pageSize = 10, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 删除指定的key & hashField记录
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        Task<bool> HashDeleteAsync(string key, string hashField, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 删除指定的hashField记录
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashFields"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        Task<long> HashDeleteAsync(string key, IEnumerable<string> hashFields, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 是否在hash表存在key的数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        Task<bool> HashExistsAsync(string key, string hashField, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 获取hash表中一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        Task<T> HashGetAsync<T>(string key, string hashField, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 获取hash表中一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="hashFields"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        Task<Dictionary<string, T>> HashGetAsync<T>(string key, IEnumerable<string> hashFields, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 获取hash表中的多条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        Task<Dictionary<string, T>> HashGetAllAsync<T>(string key, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 增加hash表中的一条记录
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <param name="value"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        Task<long> HashIncerementByAsync(string key, string hashField, long value, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 增加hash表中的一条记录
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <param name="value"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        Task<double> HashIncerementByAsync(string key, string hashField, double value, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 返回hash中key的域名
        /// </summary>
        /// <param name="key"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> HashKeysAsync(string key, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 获取hash中key的域条数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        Task<long> HashLengthAsync(string key, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 根据key和hashField更新或添加域的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <param name="value"></param>
        /// <param name="nx"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        Task<bool> HashSetAsync<T>(string key, string hashField, T value, bool nx = false, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 根据key更新或添加所有域的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        Task HashSetAsync<T>(string key, IDictionary<string, T> values, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 获取key的所有值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> HashValuesAsync<T>(string key, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// 遍历hash类型及其关联的值的字段。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="pattern"></param>
        /// <param name="pageSize"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        Task<Dictionary<string, T>> HashScanAsync<T>(string key, string pattern, int pageSize = 10, CommandFlags commandFlags = CommandFlags.None);

    }
}
