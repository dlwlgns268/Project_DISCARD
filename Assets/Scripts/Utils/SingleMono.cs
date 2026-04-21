using UnityEngine;

namespace Utils
{
    public class SingleMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        public bool destroyOnLoad = true;
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (!_instance) _instance = FindAnyObjectByType<T>();
                return _instance;
            }
        }
        
        public virtual void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }
            if (!destroyOnLoad) DontDestroyOnLoad(gameObject);
            _instance = this as T;
        }
    }
}
