using UnityEngine;
namespace Utility.DesignPatterns
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        private static object _lock = new object();

        protected static bool initialized;

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    return null;
                }
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));
                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T).ToString();
                            DontDestroyOnLoad(singleton);
                        }
                        return _instance;
                    }
                }
                return _instance;
            }
        }
        private static bool applicationIsQuitting = false;

        protected virtual void OnDestroy()
        {
            applicationIsQuitting = true;
        }
    }
}