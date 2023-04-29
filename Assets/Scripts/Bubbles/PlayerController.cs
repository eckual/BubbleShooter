using Controllers;
using UnityEngine;

namespace Bubbles
{
    public class PlayerController : ControllerBase
    {
        [SerializeField] private BubbleProjection bubbleProjection;
        [SerializeField] private BubblePathRenderer bubblePathRenderer;
        [SerializeField] private BubbleShootController bubbleShotController;
        [SerializeField] private PlayerRaycastController raycastController;

        public BubbleShootController BubbleShootController => bubbleShotController;
        public PlayerRaycastController RaycastController => raycastController;

        public override void Init()
        {
            bubbleProjection.Init();
            bubblePathRenderer.Init();
            bubbleShotController.Init();
            raycastController.Init();
        }
        
    }
}