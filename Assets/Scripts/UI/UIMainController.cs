using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace UI
{
    public class UIMainController : MonoSingleton<UIMainController>
    {
        [SerializeField] private RectTransform windowsRoot;
        [SerializeField] private List<UIBaseWindow> windows = new List<UIBaseWindow>();
        
        private Dictionary<string, UIBaseWindow> _currentUIWindows = new Dictionary<string, UIBaseWindow>();

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
                    case WindowState.None: break;
                }
                cloned.Init();
                _currentUIWindows.Add(cloned.Id, cloned);
            }
        }

        public T GetWindow<T>(string id) where T: UIBaseWindow
        {
            return _currentUIWindows.ContainsKey(id) ? _currentUIWindows[id] as T : null;
        }

    }
}
