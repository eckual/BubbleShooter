using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WindowState
{
    Opened,
    Closed
}

public class UIBaseWindow : MonoBehaviour
{
    [SerializeField]
    protected string id;
    [SerializeField]
    protected WindowState windowState;
    public event Action<WindowState> OnWindowStateChanged;

    public virtual string Id
    {
        get { return id; }
    }

    public WindowState WindowState
    {
        get { return windowState; }
        set
        {
            if (windowState != value)
            {
                windowState = value;
                OnWindowStateChanged?.Invoke(WindowState);
            }
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

    public virtual void CloseWindow()
    {
        gameObject.SetActive(false);
        WindowState = WindowState.Closed;
    }

}
