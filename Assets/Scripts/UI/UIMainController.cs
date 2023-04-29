using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using Utils;

public class UIMainController : MonoSingleton<UIMainController>
{
    [SerializeField]
    private RectTransform windowsRoot;
    [SerializeField]
    private List<UIBaseWindow> windows = new List<UIBaseWindow>();
    private Dictionary<string, UIBaseWindow> windowsDict = new Dictionary<string, UIBaseWindow>();

    public event Action OnWindowsReady;
    public event Action<UIBaseWindow> OnWindowAdded;
    public event Action<string> OnWindowRemoved;

    public override void Init()
    {
        base.Init();

        for(int i = 0; i < windows.Count; i++)
        {
            var window = windows[i];
            var cloned = Instantiate(window, windowsRoot);
            switch (cloned.WindowState)
            {
                case WindowState.Opened: cloned.gameObject.SetActive(true);break;
                case WindowState.Closed: cloned.gameObject.SetActive(false);break;
            }
            cloned.Init();
            windowsDict.Add(cloned.Id, cloned);
        }

        OnWindowsReady?.Invoke();
    }


    public UIBaseWindow GetWindow(string id)
    {
        return windowsDict.ContainsKey(id) ? windowsDict[id] : null;
    }

    public T GetWindow<T>(string id) where T: UIBaseWindow
    {
        return windowsDict.ContainsKey(id) ? windowsDict[id] as T : null;
    }

    public void AddWindow(UIBaseWindow window)
    {
        if (!windowsDict.ContainsKey(window.Id))
        {
            var cloned = Instantiate(window, windowsRoot);
            cloned.Init();
            windowsDict.Add(cloned.Id, cloned);
            OnWindowAdded(cloned);
        }
    }

    public void RemoveWindow(string id)
    {
        if (windowsDict.ContainsKey(id))
        {
            var window = windowsDict[id];
            windowsDict.Remove(id);
            Destroy(window.gameObject);
            OnWindowRemoved?.Invoke(id);
        }
    }

}
