using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;

namespace Common.Utility
{

    /// <summary>
    /// վ�㻺�湤����
    /// ���ܼ̳и���
    /// </summary>
    public sealed class CacheFactory {

        /// <summary>
        /// ����һ������
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
        /// ����һ������
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
        /// ����һ������
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expriation"></param>
        /// <param name="priority"></param>
        public static void Add(string key, object value, DateTime expriation) {
            HttpRuntime.Cache.Add(key, value, null, expriation, TimeSpan.Zero, CacheItemPriority.Default, null);
        }

        /// <summary>
        /// ����һ�����󣬲����Ļ���ʧЧ�¼�
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expriation"></param>
        /// <param name="onRemoveCallback"></param>
        public static void Add(string key, object value, DateTime expriation, CacheItemRemovedCallback onRemoveCallback) {
            HttpRuntime.Cache.Add(key, value, null, expriation, TimeSpan.Zero, CacheItemPriority.Default, onRemoveCallback);
        }

        /// <summary>
        /// ����һ������
        /// Ĭ�ϻ���ʱ��Ϊ12h
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="dependencyFileName"></param>
        /// <param name="priority"></param>
        public static void Add(string key, object value, string dependencyFileName) {
            Add(key, value, DateTime.Now.AddHours(12), dependencyFileName);
        }

        /// <summary>
        /// ��ȡ����ָ����ֵ�Ļ������
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetCache(string key) {
            return HttpRuntime.Cache[key];
        }

        /// <summary>
        /// ɾ������ָ����ֵ�Ļ������
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key) {
            HttpRuntime.Cache.Remove(key);
        }

        /// <summary> 
        /// ������е�Cache 
        /// </summary> 
        public static void ClearCache() {
            IDictionaryEnumerator cacheEnum = HttpContext.Current.Cache.GetEnumerator();
            while(cacheEnum.MoveNext()) {
                HttpContext.Current.Cache.Remove(cacheEnum.Key.ToString());
            }
        }
    }
}
