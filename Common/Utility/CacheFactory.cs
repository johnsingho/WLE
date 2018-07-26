using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;

namespace Common.Utility
{

    /// <summary>
    /// 站点缓存工厂类
    /// 不能继承该类
    /// </summary>
    public sealed class CacheFactory {

        /// <summary>
        /// 缓存一个对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expriation"></param>
        /// <param name="dependencyFileName"></param>
        /// <param name="priority"></param>
        public static void Add(string key, object value, DateTime expriation, string dependencyFileName, CacheItemPriority priority) {
            HttpRuntime.Cache.Add(key, value, new CacheDependency(dependencyFileName), expriation, TimeSpan.Zero, priority, null);
        }

        /// <summary>
        /// 缓存一个对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expriation"></param>
        /// <param name="dependencyFileNames"></param>
        /// <param name="priority"></param>
        public static void Add(string key, object value, DateTime expriation, params string[] dependencyFileNames) {
            HttpRuntime.Cache.Add(key, value, new CacheDependency(dependencyFileNames), expriation, TimeSpan.Zero, CacheItemPriority.Default, null);
        }

        /// <summary>
        /// 缓存一个对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expriation"></param>
        /// <param name="priority"></param>
        public static void Add(string key, object value, DateTime expriation) {
            HttpRuntime.Cache.Add(key, value, null, expriation, TimeSpan.Zero, CacheItemPriority.Default, null);
        }

        /// <summary>
        /// 缓存一个对象，并订阅缓存失效事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expriation"></param>
        /// <param name="onRemoveCallback"></param>
        public static void Add(string key, object value, DateTime expriation, CacheItemRemovedCallback onRemoveCallback) {
            HttpRuntime.Cache.Add(key, value, null, expriation, TimeSpan.Zero, CacheItemPriority.Default, onRemoveCallback);
        }

        /// <summary>
        /// 缓存一个对象，
        /// 默认缓存时间为12h
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="dependencyFileName"></param>
        /// <param name="priority"></param>
        public static void Add(string key, object value, string dependencyFileName) {
            Add(key, value, DateTime.Now.AddHours(12), dependencyFileName);
        }

        /// <summary>
        /// 获取具有指定键值的缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetCache(string key) {
            return HttpRuntime.Cache[key];
        }

        /// <summary>
        /// 删除具有指定键值的缓存对象
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key) {
            HttpRuntime.Cache.Remove(key);
        }

        /// <summary> 
        /// 清空所有的Cache 
        /// </summary> 
        public static void ClearCache() {
            IDictionaryEnumerator cacheEnum = HttpContext.Current.Cache.GetEnumerator();
            while(cacheEnum.MoveNext()) {
                HttpContext.Current.Cache.Remove(cacheEnum.Key.ToString());
            }
        }
    }
}
