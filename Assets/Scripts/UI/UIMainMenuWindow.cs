using Controllers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIMainMenuWindow : UIBaseWindow
    {
        private const string BACKGROUND_AUDIO ="Background";
        [SerializeField] private Button startGameBtn;
        [SerializeField] private Image triangleImage;

        public override void Init()
        {
            base.Init();
            AnimateTriangleImage();
            startGameBtn.onClick.AddListener(OnPlayButtonClick);
            isInitialized = true;
        }

        private void AnimateTriangleImage() => triangleImage.DOColor(Color.gray, 1.2f).SetLoops(-1, LoopType.Yoyo);

        private void OnPlayButtonClick()
        {
            CloseWindow();
            SessionController.Instance.StartSession();
            SoundsController.Instance.StopSound(BACKGROUND_AUDIO);
            DOTween.Kill(triangleImage.color);
            triangleImage.color = Color.white;
        }

        public override void OpenWindow()
        {
            base.OpenWindow();
            if (!isInitialized) return;
            SoundsController.Instance.PlaySound(BACKGROUND_AUDIO);
            AnimateTriangleImage();
        }
    }
}
