using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIMainMenuWindow : UIBaseWindow
    {
        [SerializeField] private Button startGameBtn;

        public override void Init()
        {
            base.Init();
            startGameBtn.onClick.AddListener(OnPlayButtonClick);
        }

        private void OnPlayButtonClick()
        {
            CloseWindow();
            SessionController.Instance.StartSession();
        }

    }
}
