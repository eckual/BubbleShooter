using System.Collections.Generic;
using Extensions;
using UnityEngine;
using Utils;

namespace Bubbles
{
    public class BubbleProjection : MonoBehaviour
    {
        private const string SHOW_TRIGGER = "show";
        [SerializeField] private Animator animator;
        [SerializeField] private new SpriteRenderer renderer;

        private BubblesSettings _settings;
        private SessionController _sessionController;
        private Vector3 _initialPosition;
        private int _prevX;
        private int _prevY;
        
        public void Init()
        {
            _settings = ResourceManager.GetResource<BubblesSettings>(GameConstants.BubbleSettings);
            _sessionController = SessionController.Instance;
            _initialPosition = transform.position;

            var raycastController = _sessionController.PlayerController.RaycastController;
            raycastController.OnBubbleChanged += OnBubbleChanged;
            raycastController.OnPathChanged += OnPathChanged;
            raycastController.OnStartRaycasting += OnStartRaycasting;
            raycastController.OnStopRaycasting += OnStopRaycasting;

            _sessionController.BubblesController.OnCurrentPowerChanged += OnCurrentPowerChanged;

            _prevX = PlayerRaycastController.DEFAULT_X;
            _prevY = PlayerRaycastController.DEFAULT_Y;
        }

        private void OnStopRaycasting()
        {
            gameObject.SetActive(false);
        }

        private void OnCurrentPowerChanged(int currentPower)
        {
            var bubbleDataIndex = _settings.Bubbles.FindIndex(x => x.number == Bubble.GetNumber(_sessionController.BubblesController.CurrentPower));
            if (bubbleDataIndex == -1)
                return;

            var color = _settings.Bubbles[bubbleDataIndex].backColor;
            color.a = (byte)(renderer.color.a * 255);
            renderer.color = color;
        }

        private void OnStartRaycasting()
        {
            var bubbleDataIndex = _settings.Bubbles.FindIndex(x => x.number == Bubble.GetNumber(_sessionController.BubblesController.CurrentPower));
            if (bubbleDataIndex == -1)
                return;

            var color = _settings.Bubbles[bubbleDataIndex].backColor;
            color.a = (byte)(renderer.color.a * 255);
            renderer.color = color;
            gameObject.SetActive(true);
        }

        private void OnBubbleChanged(int x, int y, Vector3 position)
        {
            if (x != _prevX || y != _prevY)
            {
                animator.SetTrigger(SHOW_TRIGGER);
                _prevX = x;
                _prevY = y;
            }
            transform.position = position;
        }
        private void OnPathChanged(List<Vector3> path)
        {
            if (path.IsEmpty()) transform.position = _initialPosition;
        }
        
    }
}