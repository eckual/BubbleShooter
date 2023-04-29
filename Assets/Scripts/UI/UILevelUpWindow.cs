using Controllers;
using Sound;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UILevelUpWindow : UIBaseWindow
    {
        [SerializeField] private string levelUpSoundId = "LevelUp";
        [SerializeField] private TMP_Text level;

        public void OpenWindow(int currentLevel)
        {
            level.text = currentLevel.ToString();
            SessionController.Instance.PauseSession();
            SoundsController.Instance.PlaySound(levelUpSoundId);
            OpenWindow();
        }

        protected override void CloseWindow()
        {
            base.CloseWindow();

            SessionController.Instance.StartSession();
        }
    }
}
