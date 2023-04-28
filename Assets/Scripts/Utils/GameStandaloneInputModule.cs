using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStandaloneInputModule : GameInputModule
{
    public override bool IsAvailable { get { return Input.mousePresent; } }
    public override Vector2 InputPosition { get { return Input.mousePosition; } }

    protected override void Update()
    {
        if (!IsAvailable)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            InputStart();
        }

        if (Input.GetMouseButton(0))
        {
            InputStay();
        }

        if (Input.GetMouseButtonUp(0))
        {
            InputEnd();
        }
    }
}
