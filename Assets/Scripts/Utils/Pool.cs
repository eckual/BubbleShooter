using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Utils
{
    public interface IPoolObject
    {
        string Id { get; set; }
        void Release();
    }

    public class Pool<T> : MonoBehaviour where T:MonoBehaviour, IPoolObject
    {
        [SerializeField] private Transform root = null;
        [SerializeField] private List<T> templates = new List<T>();
        
        private List<T> _busyObjects = new List<T>();
        private List<T> _freeObjects = new List<T>();

        public Transform Root => root;

        public T GetOrInstantiate(string id)
        {
            var freeObject = _freeObjects.FirstOrDefault(fO => fO.Id == id);
            if (freeObject)
            {
                _freeObjects.Remove(freeObject);
                _busyObjects.Add(freeObject);
                return freeObject;
            }

            var template = templates.FirstOrDefault(temp => temp.Id == id);
            if (template == null) return null;
            
            var clone = Instantiate(template, root);
            _busyObjects.Add(clone);
            return clone;
        }

        public void Release(T busyObject)
        {
            if (_freeObjects.Contains(busyObject)) return;
            
            if(busyObject == null) return;
            
            busyObject.Release();
            _busyObjects.Remove(busyObject);
            _freeObjects.Add(busyObject);
        }

        [Button] public void ReleaseAll()
        {
            for(var i = 0; i < _busyObjects.Count; i++)
            {
                var busyObject = _busyObjects[i];
                Release(busyObject);
            }
        }
    }
    
}
