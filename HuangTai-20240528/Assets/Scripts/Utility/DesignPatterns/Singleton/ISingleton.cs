using System;

namespace Utility.DesignPatterns
{
    public interface ISingleton<T> where T : class
    {
        private static T _instance;
        private static object _lock = new object();
        protected static bool initialized;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = (T)Activator.CreateInstance(typeof(T), true);
                        }
                        return _instance;
                    }
                }
                return _instance;
            }
        }
    }
}