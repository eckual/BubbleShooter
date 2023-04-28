using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainMenuWindow : UIBaseWindow
{
    public void OnPlayButtonClick()
    {
        CloseWindow();
        SessionController.Instance.StartSession();
    }

}
