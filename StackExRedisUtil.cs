using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DealApi.Lib;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace DealApi.Utility
{
    public static class StackExRedisUtil
    {
        #region StackExchange Redis Utils
        /// <summary>
        /// Set all cache key by Module. This function using save all cache key by module. MP Backend list all cache key to manager
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cachekey"></param>
        /// <returns></returns>
        //public static bool UpdateModuleCacheKey(string module, string cachekey)
        //{
        //    List<string> CacheKeys = new List<string>();
        //    bool ret = false;

        //    if (!string.IsNullOrEmpty(module) && !string.IsNullOrEmpty(cachekey))
        //    {
        //        cachekey = cachekey + ":v2";
        //        var RedisConnection = RedisConnectionFactory.GetConnection();
        //        var db = RedisConnection.GetDatabase();
        //        if (string.IsNullOrEmpty(db.StringGet(module)))
        //        {
        //            CacheKeys.Add(cachekey);
        //            Set(db,module, CacheKeys,0);
        //            ret = true;
        //        }
        //        else
        //        {
        //            CacheKeys = Get<List<string>>(db,module);
        //            IEnumerable<string> matchingvalues = CacheKeys.Where(stringToCheck => stringToCheck.Contains(cachekey));
        //            if (matchingvalues == null || matchingvalues.LongCount() == 0)
        //            {
        //                CacheKeys.Add(cachekey);
        //                Set(db, module, CacheKeys,0);
        //                ret = true;
        //            }
        //        }

        //    }
        //    return ret;
        //}
        /// <summary>
        /// Get All Cache key by Modules
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public static List<string> GetAllCacheKey(string module, out string filesize)
        {
            long size = 0;
            filesize = string.Empty;
            List<RedisKey> CacheKeys = new List<RedisKey>();
            List<string> lstKeys = new List<string>();
            var RedisConnection = RedisConnectionFactory.GetConnection();
            var db = RedisConnection.GetDatabase();

            CacheKeys = RedisConnection.GetServer("cache1", 6379).Keys().ToList();//redisClient.Get<List<string>>(module);
            //size += redisClient.Get(module).Length;
            if (CacheKeys != null && CacheKeys.LongCount() > 0)
            {
                //foreach (string i in CacheKeys) {
                //    if (redisClient.Get(i) != null)
                //    {
                //        size += redisClient.Get(i).Length;
                //    }
                //}
                //filesize = SizeSuffix(size);
                foreach (RedisKey key in CacheKeys)
                {
                    lstKeys.Add(key.ToString());
                }
            }


            return lstKeys;
        }
        public static List<string> SearchCacheKey(string module)
        {
            List<RedisKey> CacheKeys = new List<RedisKey>();
            List<string> lstKeys = new List<string>();
            var RedisConnection = RedisConnectionFactory.GetConnection();
            var db = RedisConnection.GetDatabase();

            CacheKeys = RedisConnection.GetServer("cache1", 6379).Keys(pattern: "*" + module + "*").ToList();//redisClient.Get<List<string>>(module);
            if (CacheKeys != null && CacheKeys.LongCount() > 0)
            {
                foreach (RedisKey key in CacheKeys)
                {
                    lstKeys.Add(key.ToString());
                }
            }


            return lstKeys;
        }
        public static string GetCacheExpired(string key)
        {
            var RedisConnection = RedisConnectionFactory.GetConnection();
            var db = RedisConnection.GetDatabase();
            var timeToLive = db.KeyTimeToLive(key);
            return timeToLive.ToJson();
        }
        public static string GetCacheSizeByKey(string key)
        {
            long size = 0;
            string cacheSize = string.Empty;
            return cacheSize;
        }
        /// <summary>
        /// Remove all by List Key Cache from client
        /// </summary>
        /// <param name="allkeys"></param>
        public static void RemoveAllCacheByListKey(List<string> allkeys)
        {

            if (allkeys != null && allkeys.Count > 0)
            {
                var RedisConnection = RedisConnectionFactory.GetConnection();
                var db = RedisConnection.GetDatabase();
                foreach (string key in allkeys)
                {
                    db.KeyDelete(key);
                }

            }

        }

        /// <summary>
        /// Remove all Cache from client
        /// </summary>
        /// <param name="allkeys"></param>
        public static void RemoveAllCache()
        {
            var RedisConnection = RedisConnectionFactory.GetConnection();
            var server = RedisConnection.GetServer("cache1", 6379);
            server.FlushDatabase();
        }
        /// <summary>
        /// Auto set cache for 5 minute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="module"></param>
        /// <param name="value"></param>
        /// <param name="funtionname"></param>
        /// <param name="args"></param>
        /// 
        public static void SetCacheByKeyShortestTime<T>(string module, T value, string functionname, params object[] args)
        {
            if (IsNull<T>(value)) return;
            if (!string.IsNullOrEmpty(module) && value != null && !string.IsNullOrEmpty(functionname) && AppEnv.GetSetting("isdebugcachemode") == "0")
            {
                var RedisConnection = RedisConnectionFactory.GetConnection();
                var db = RedisConnection.GetDatabase();
                string cachekey = module + ":" + functionname + ":" + string.Join(":", args) + ":v2";

                Set(db, cachekey, value, (int)CacheDefinition.TimeIntervalCaching.shortestcache);
                //UpdateModuleCacheKey(module, cachekey);
            }
        }
        /// <summary>
        /// Auto set cache for 5 minute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="module"></param>
        /// <param name="value"></param>
        /// <param name="funtionname"></param>
        /// <param name="args"></param>
        /// 
        public static void SetCacheByKeyShortTime<T>(string module, T value, string functionname, params object[] args)
        {
            if (IsNull<T>(value)) return;
            if (!string.IsNullOrEmpty(module) && value != null && !string.IsNullOrEmpty(functionname) && AppEnv.GetSetting("isdebugcachemode") == "0")
            {
                var RedisConnection = RedisConnectionFactory.GetConnection();
                var db = RedisConnection.GetDatabase();
                string cachekey = module + ":" + functionname + ":" + string.Join(":", args) + ":v2";

                Set(db, cachekey, value, (int)CacheDefinition.TimeIntervalCaching.shortcache);
                //UpdateModuleCacheKey(module, cachekey);
            }
        }
        /// <summary>
        /// Auto set cache for 60 minute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="module"></param>
        /// <param name="value"></param>
        /// <param name="funtionname"></param>
        /// <param name="args"></param>
        public static void SetCacheByKeyDefault<T>(string module, T value, string functionname, params object[] args)
        {
            try
            {
                if (IsNull<T>(value)) return;
                if (!string.IsNullOrEmpty(module) && value != null && !string.IsNullOrEmpty(functionname) && AppEnv.GetSetting("isdebugcachemode") == "0")
                {
                    var RedisConnection = RedisConnectionFactory.GetConnection();
                    var db = RedisConnection.GetDatabase();
                    string cachekey = module + ":" + functionname + ":" + string.Join(":", args) + ":v2";
                    Set(db, cachekey, value, (int)CacheDefinition.TimeIntervalCaching.normalcache);
                    //UpdateModuleCacheKey(module, cachekey);
                }
            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// Set cache permanent
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="module"></param>
        /// <param name="value"></param>
        /// <param name="funtionname"></param>
        /// <param name="args"></param>
        public static void SetCacheByKeypermanent<T>(string module, T value, string functionname, params object[] args)
        {
            try
            {
                if (IsNull<T>(value)) return;
                if (!string.IsNullOrEmpty(module) && value != null && !string.IsNullOrEmpty(functionname) && AppEnv.GetSetting("isdebugcachemode") == "0")
                {
                    var RedisConnection = RedisConnectionFactory.GetConnection();
                    var db = RedisConnection.GetDatabase();
                    string cachekey = module + ":" + functionname + ":" + string.Join(":", args) + ":v2";
                    Set(db, cachekey, value, (int)CacheDefinition.TimeIntervalCaching.longcache);
                    //UpdateModuleCacheKey(module, cachekey);
                }
            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="module"></param>
        /// <param name="functionname"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T GetCacheByKey<T>(string module, string functionname, params object[] args)
        {
            try
            {
                if (!string.IsNullOrEmpty(module) && !string.IsNullOrEmpty(functionname) && AppEnv.GetSetting("isdebugcachemode") == "0")
                {
                    var RedisConnection = RedisConnectionFactory.GetConnection();
                    var db = RedisConnection.GetDatabase();
                    string cachekey = module + ":" + functionname + ":" + string.Join(":", args) + ":v2";
                    return Get<T>(db, cachekey);
                }
            }
            catch (Exception)
            {
            }
            return default(T);
        }
        //
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="module"></param>
        /// <param name="functionname"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T GetCacheByKeyStr<T>(string key)
        {
            if (!string.IsNullOrEmpty(key) && AppEnv.GetSetting("isdebugcachemode") == "0")
            {
                var RedisConnection = RedisConnectionFactory.GetConnection();
                var db = RedisConnection.GetDatabase();
                return Get<T>(db, key);
            }
            return default(T);
        }
        #endregion
        #region common Function Cache
        public static T Get<T>(this IDatabase cache, string key)
        {
            try
            {
                var value = cache.StringGet(key);
                if (!value.IsNull)
                    return JsonConvert.DeserializeObject<T>(value);
                else
                {
                    return default(T);
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static void Set(this IDatabase cache, string key, object value, int minutes)
        {
            if (minutes > 0)
            {
                TimeSpan experation = new TimeSpan(0, 0, minutes, 0);
                cache.StringSet(key, JsonConvert.SerializeObject(value), experation);
            }
            else
            {
                cache.StringSet(key, JsonConvert.SerializeObject(value));
            }
        }

        public static void Delete(this IDatabase cache, string key)
        {
            cache.Delete(key);
        }
        #endregion
        #region Extention
        public static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        public static string SizeSuffix(Int64 value)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0.0 bytes"; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }
        public static bool IsNull<T>(T subject)
        {
            return ReferenceEquals(subject, null);
        }
        #endregion
    }
}