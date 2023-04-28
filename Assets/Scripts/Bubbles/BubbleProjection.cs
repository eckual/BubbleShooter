using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bubbles
{
    public class BubbleProjection : MonoBehaviour
    {
        private const string SHOW_TRIGGER = "show";
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private new SpriteRenderer renderer;

        private Vector3 initialPosition;
        private int prevX;
        private int prevY;

        private BubblesSettings settings;
        private SessionController sessionController;
        public void Init()
        {
            settings = ResourceManager.GetResource<BubblesSettings>(GameConstants.BUBBLE_SETTINGS);
            sessionController = SessionController.Instance;
            initialPosition = transform.position;

            var raycastController = sessionController.PlayerController.RaycastController;
            raycastController.OnBubbleChanged += OnBubbleChanged;
            raycastController.OnPathChanged += OnPathChanged;
            raycastController.OnStartRaycasting += OnStartRaycasting;
            raycastController.OnStopRaycasting += OnStopRaycasting;

            sessionController.BubblesController.OnCurrentPowerChanged += OnCurrentPowerChanged;

            prevX = PlayerRaycastController.DEFAULT_X;
            prevY = PlayerRaycastController.DEFAULT_Y;
        }

        private void OnStopRaycasting()
        {
            gameObject.SetActive(false);
        }

        private void OnCurrentPowerChanged(int currentPower)
        {
            var bubbleDataIndex = settings.Bubbles.FindIndex(x => x.number == Bubble.GetNumber(sessionController.BubblesController.CurrentPower));
            if (bubbleDataIndex == -1)
                return;

            var color = settings.Bubbles[bubbleDataIndex].backColor;
            color.a = (byte)(renderer.color.a * 255);
            renderer.color = color;
        }

        private void OnStartRaycasting()
        {
            var bubbleDataIndex = settings.Bubbles.FindIndex(x => x.number == Bubble.GetNumber(sessionController.BubblesController.CurrentPower));
            if (bubbleDataIndex == -1)
                return;

            var color = settings.Bubbles[bubbleDataIndex].backColor;
            color.a = (byte)(renderer.color.a * 255);
            renderer.color = color;
            gameObject.SetActive(true);
        }

        private void OnBubbleChanged(int x, int y, Vector3 position)
        {
            if (x != prevX || y != prevY)
            {
                animator.SetTrigger(SHOW_TRIGGER);
                prevX = x;
                prevY = y;
            }
            transform.position = position;
        }
        private void OnPathChanged(List<Vector3> path)
        {
            if(path.Count == 0)
            {
                transform.position = initialPosition;
            }
        }
    }
}