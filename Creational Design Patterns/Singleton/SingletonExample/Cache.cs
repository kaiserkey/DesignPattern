using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace SingletonExample
{
    public sealed class Cache : ICloneable
    {
        /**
         * the singleton instance of the Cache class
         */
        private static readonly Lazy<Cache> instance = new Lazy<Cache>(()=>new Cache());

        /**
         * object to lock the access to the cache data structure
         */
        private static readonly object syncLock = new object();

        /**
         * Flag to check if the instance is created
         */
        private static bool isInstanceCreated = false;

        /**
         * Dictionary to store cache data
         */
        private Dictionary<string, string> cacheData;

        /**
         * Private constructor to prevent instantiation
         */
        private Cache() {
            if(isInstanceCreated)
            {
                throw new InvalidOperationException("An instance of Cache already exists.");
            }

            isInstanceCreated = true;
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
            lock (syncLock) {
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
            lock(syncLock) {
                return cacheData.ContainsKey(key) ? cacheData[key] : null;
            }
        }

        /**
         * Clone method to prevent cloning of the Cache object
         */
        public object Clone()
        {
            throw new NotSupportedException("Cloning of Cache is not allowed.");
        }

        /**
         * Explicitly handle deserialization to ensure only one instance
         */
        protected Cache(SerializationInfo info, StreamingContext context)
        {
            throw new InvalidOperationException("Serialization of Cache is not allowed.");
        }

    }
}
