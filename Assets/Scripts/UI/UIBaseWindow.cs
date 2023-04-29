using System;
using UnityEngine;

namespace UI
{
    public enum WindowState
    {
        Opened,
        Closed
    }

    public class UIBaseWindow : MonoBehaviour
    {
        public event Action<WindowState> OnWindowStateChanged;
        
        [SerializeField] protected string id;
        [SerializeField] protected WindowState windowState;

        protected bool isInitialized;
        public virtual string Id => id;

        public WindowState WindowState
        {
            get => windowState;
            private set
            {
                if (windowState == value) return;
                windowState = value;
                OnWindowStateChanged?.Invoke(WindowState);
            }
        }

        public virtual void Init()
        {

        }

        public virtual void OpenWindow()
        {
            gameObject.SetActive(true);
            WindowState = WindowState.Opened;
        }

        protected virtual void CloseWindow()
        {
            gameObject.SetActive(false);
            WindowState = WindowState.Closed;
        }

    }
}