using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTouchInputModule : GameInputModule
{
    public override bool IsAvailable { get { return Input.touchSupported; } }

    public override Vector2 InputPosition
    {
        get { return Input.touchCount > 0 ? Input.GetTouch(0).position : Vector2.zero; }
    }

    public override void Init()
    {
        base.Init();
        Input.multiTouchEnabled = false;
    }

    protected override void Update()
    {
        if (!IsAvailable)
            return;

        if (Input.touchCount == 0)
            return;

        var touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            InputStart();
        }
        if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        {
            InputStay();
        }
        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            InputEnd();
        }
    }
    
}
