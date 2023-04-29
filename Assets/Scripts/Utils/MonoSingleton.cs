using UnityEngine;

namespace Utils
{
    public class MonoSingleton<T> : MonoBehaviour where T:MonoBehaviour
    {
        [SerializeField] protected bool dontDestroyOnLoad = false;
        private static object lockObject = new object();
        private static T instance = null;

        private void Awake()
        {
            if (Instance != this) return;
            
            if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
            Init();
        }
        
        public static T Instance
        {
            get
            {
                if (instance !=null ) return instance;
            
                var instances = FindObjectsOfType<T>(); 
                if (instances.Length <= 0)
                    return instance;
            
                lock (lockObject) instance = instances[0];
                for(var i = 1; i < instances.Length; i++) 
                    Destroy(instances[i].gameObject);

                if (instance) return instance;
                lock (lockObject)
                {
                    var gameObject = new GameObject();
                    instance = gameObject.AddComponent<T>();
                    gameObject.name = typeof(T).Name;
                }
                return instance;
            }
        }
        public virtual void Init()
        {
        }

        protected virtual void OnDestroy()
        {
            ReleaseReferences();
        }

        public virtual void ReleaseReferences()
        {
            lockObject = null;
        }
       
        
    }
}
