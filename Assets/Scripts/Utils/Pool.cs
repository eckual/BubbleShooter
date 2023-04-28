using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolObject
{
    string Id { get; set; }
    void Release();
}

public class Pool<T> : MonoBehaviour where T:MonoBehaviour, IPoolObject
{
    [SerializeField]
    private Transform root = null;
    [SerializeField]
    private List<T> templates = new List<T>();
    private List<T> busyObjects = new List<T>();
    private List<T> freeObjects = new List<T>();

    public Transform Root
    {
        get { return root; }
    }

    public T GetOrInstantiate(string id)
    {
        var freeObject = freeObjects.Find(x => x.Id == id);
        if (freeObject)
        {
            freeObjects.Remove(freeObject);
            busyObjects.Add(freeObject);
            return freeObject;
        }
        else
        {
            var template = templates.Find(x => x.Id == id);
            if (template)
            {
                var clone = Instantiate(template, root);
                busyObjects.Add(clone);
                return clone;
            }
        }
        return null;
    }

    public void Release(T busyObject)
    {
        if (!freeObjects.Contains(busyObject))
        {
            busyObject.Release();
            busyObjects.Remove(busyObject);
            freeObjects.Add(busyObject);
        }
    }

    public void ReleaseAll()
    {
        for(int i = busyObjects.Count -1 ; i >= 0; i--)
        {
            var busyObject = busyObjects[i];
            Release(busyObject);
        }
    }
}
