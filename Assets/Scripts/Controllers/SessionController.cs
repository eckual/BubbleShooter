using Bubbles;
using UnityEngine;
using Utils;

namespace Controllers
{
    public class SessionController : MonoSingleton<SessionController>
    {
        [SerializeField] private BubblesController bubblesController;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private ScoreController scoreController;
        [SerializeField] private VFXController vfxController;

        public BubblesController BubblesController => bubblesController;
        public PlayerController PlayerController => playerController;
        public ScoreController ScoreController => scoreController;

        public bool IsRunning { get; private set; }

        public override void ReleaseReferences()
        {
            base.ReleaseReferences();
            bubblesController = null;
            playerController = null;
            scoreController = null;
            vfxController = null;
        }

        public override void Init()
        {
            scoreController.Init();
            bubblesController.Init();
            playerController.Init();
            vfxController.Init();
        }

        public void StartSession() => IsRunning = true;

        public void PauseSession() => IsRunning = false;
    
    }
}
