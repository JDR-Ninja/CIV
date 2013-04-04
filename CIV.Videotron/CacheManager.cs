using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Videotron.Wired;
using Videotron.Enums;

namespace Videotron
{
    internal class CacheManager
    {
        #region Singleton
        private static CacheManager _instance;
        private static object _instanceLocker = new object();

        // Singleton
        public static CacheManager Instance
        {
            get
            {
                lock (_instanceLocker)
                {
                    if (_instance == null)
                        _instance = new CacheManager();
                    return _instance;
                }
            }
        }
        #endregion

        private Dictionary<string, WiredAccountCache> _cache = new Dictionary<string, WiredAccountCache>();

        private string GetKey(string token, int period)
        {
            return String.Format("{0}-{1}", token, period);
        }

        /// <summary>
        /// Vérifie si un objet est dans le cache
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool IsCached(string token, int period)
        {
            return _cache.ContainsKey(GetKey(token, period));
        }

        /// <summary>
        /// Crée un cache pour un objet
        /// </summary>
        /// <param name="token"></param>
        public void CreateCache(string token, int period)
        {
            _cache.Add(GetKey(token, period), new WiredAccountCache() { Status = CacheStatusTypes.Waiting });
        }

        /// <summary>
        /// Vérifie si un objet est prêt à être lue
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool IsReady(string token, int period)
        {
            return _cache[GetKey(token, period)].Status == CacheStatusTypes.Ready;
        }

        /// <summary>
        /// Obtient un objet à partir du cache
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public WiredAccountCache GetCache(string token, int period)
        {
            //WiredAccountCache cachedItem;

            return _cache[GetKey(token, period)];
        }

        /// <summary>
        /// Met à jours un objet dans le cache
        /// </summary>
        /// <param name="token"></param>
        /// <param name="wiredAccount"></param>
        public void SetCache(string token, int period, WiredAccount wiredAccount)
        {
            _cache[GetKey(token, period)].WiredAccount = wiredAccount;
        }
    }
}