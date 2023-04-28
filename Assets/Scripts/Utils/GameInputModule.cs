using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInputModule : MonoBehaviour
{
    public virtual bool IsAvailable { get; }
    public virtual Vector2 InputPosition { get; }

    public event Action OnInputStart;
    public event Action OnInputStay;
    public event Action OnInputEnd;

    public virtual void Init()
    {

    }

    public void InputStart()
    {
        OnInputStart?.Invoke();
    }

    public void InputStay()
    {
        OnInputStay?.Invoke();
    }

    public void InputEnd()
    {
        OnInputEnd?.Invoke();
    }

    protected virtual void Update()
    {

    }
}
