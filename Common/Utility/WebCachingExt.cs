using System;
using System.Web.Caching;

namespace Common.Utility
{
    /// <summary>
    /// Web.Caching typed Extension
    /// Modified: 2018-04-13
    /// </summary>
    public static class WebCachingExt
    {
        private static object sync = new object();
        private static DateTime lastSync = default(DateTime);
        public const int DefaultCacheExpMin = 10; //默认超时,min
        //public static event EventHandler<WebCachingEventArgs> CacheChanged;


        /// <summary>
        /// Allows Caching of typed data
        /// </summary>
        /// <example><![CDATA[
        /// var user = HttpRuntime
        ///   .Cache
        ///   .GetOrStore<User>(
        ///      string.Format("User{0}", _userId), 
        ///      () => Repository.GetUser(_userId));
        ///
        /// ]]></example>
        public static T GetOrStore<T>(this Cache cache, string key, Func<T> generator)
        {
            return cache.GetOrStore(key
                , (cache[key] == null && generator != null) ? generator() : default(T)
                , DefaultCacheExpMin);
        }


        /// <summary>
        /// Allows Caching of typed data
        /// </summary>
        public static T GetOrStore<T>(this Cache cache, string key, Func<T> generator, double expireInMinutes)
        {
            return cache.GetOrStore(key
                , (cache[key] == null && generator != null) ? generator() : default(T)
                , expireInMinutes);
        }


        /// <summary>
        /// Allows Caching of typed data
        /// </summary>
        public static T GetOrStore<T>(this Cache cache, string key, T obj)
        {
            return cache.GetOrStore(key, obj, DefaultCacheExpMin);
        }

        /// <summary>
        /// Allows Caching of typed data
        /// </summary>
        public static T GetOrStore<T>(this Cache cache, string key, T obj, double expireInMinutes)
        {
            var result = cache[key];
            if (result == null)
            {
                lock (sync)
                {
                    result = cache[key];
                    if (result == null)
                    {
                        var dtSync = DateTime.Now;
                        lastSync = dtSync;
                        result = obj != null ? obj : default(T);
                        cache.Insert(key, result, null, dtSync.AddMinutes(expireInMinutes), Cache.NoSlidingExpiration);
                        //OnCacheChanged(cache, new WebCachingEventArgs());
                    }
                }
            }
            return (T)result;
        }

        //private static void OnCacheChanged(Cache cache, WebCachingEventArgs e)
        //{
        //    if (CacheChanged != null)
        //    {
        //        CacheChanged(cache, e);
        //    }
        //}

        public static DateTime GetLastSyncTime<T>(this Cache cache)
        {
            return lastSync;
        }
    }

    public class WebCachingEventArgs : EventArgs
    {
    }

}