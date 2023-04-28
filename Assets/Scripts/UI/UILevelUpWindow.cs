using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sound;
public class UILevelUpWindow : UIBaseWindow
{
    [SerializeField]
    private string levelUpSoundId = "LevelUp";
    [SerializeField]
    private TMP_Text level;

    public void OpenWindow(int currentLevel)
    {
        level.text = currentLevel.ToString();
        SessionController.Instance.PauseSession();
        SoundsController.Instance.PlaySound(levelUpSoundId);
        OpenWindow();
    }

    public override void CloseWindow()
    {
        base.CloseWindow();

        SessionController.Instance.StartSession();
    }
}
