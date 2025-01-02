namespace SingletonExample
{
    public sealed class Cache
    {
        /**
         * the singleton instance of the Cache class
         */
        private static readonly Lazy<Cache> instance = new Lazy<Cache>(()=>new Cache());

        /**
         * Dictionary to store cache data
         */
        private Dictionary<string, string> cacheData;

        /**
         * Private constructor to prevent instantiation
         */
        private Cache() {
            cacheData = new Dictionary<string, string>();
        }

        /**
         * Get the singleton instance of the Cache class
         * @return Cache instance
         */
        public static Cache Instance => instance.Value;

        /**
         * Add data to the cache
         * @param key Key to store the data
         * @param value Value to store
         */
        public void Add(string key, string value)
        {
            lock (cacheData) {
                cacheData[key] = value;
            }
        }

        /**
         * Get data from the cache
         * @param key Key to retrieve the data
         * @return Value stored in the cache
         */
        public string Get(string key)
        {
            lock(cacheData) {
                return cacheData.ContainsKey(key) ? cacheData[key] : null;
            }
        }
    }
}
