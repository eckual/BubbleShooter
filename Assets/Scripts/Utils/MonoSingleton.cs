using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T:MonoBehaviour
{
    private static object lockObject = new object();
    private static T instance = null;
    private static bool isApplicationQuiting = false;

    public static T Instance
    {
        get
        {
            if (isApplicationQuiting)
                return null;

            if(!instance)
            {

                var instances = FindObjectsOfType<T>();
                if(instances.Length > 0)
                {
                    lock (lockObject)
                    {
                        instance = instances[0];
                    }
                    for(int i = 1; i < instances.Length; i++)
                    {
                        Destroy(instances[i].gameObject);
                    }

                    if (!instance)
                    {
                        lock (lockObject)
                        {
                            var gameObject = new GameObject();
                            instance = gameObject.AddComponent<T>();
                            gameObject.name = typeof(T).Name;
                        }
                    }
                }
            }
            return instance;
        }
    }

    public static bool HasInstance
    {
        get { return instance; }
    }

    [SerializeField]
    protected bool dontDestroyOnLoad = false;

    public static void CreateInstance(bool dontDestroyOnLoad = false)
    {
        if (!instance)
        {
            var gameObject = new GameObject();
            instance = gameObject.AddComponent<T>();
            gameObject.name = typeof(T).Name;

            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }

    protected void Awake()
    {
        if (Instance == this)
        {
            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
            Init();
        }
    }

    public virtual void Init()
    {

    }

    protected virtual void OnDestroy()
    {

    }

    protected virtual void OnApplicationPause(bool pause)
    {

    }

    protected virtual void OnApplicationQuit()
    {
        isApplicationQuiting = true;
    }
}
