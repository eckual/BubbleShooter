using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bubbles
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private BubbleProjection bubbleProjection;
        [SerializeField]
        private BubblePathRenderer bubblePathRenderer;
        [SerializeField]
        private BubbleShootController bubbleShotController;
        [SerializeField]
        private PlayerRaycastController raycastController;

        public BubbleProjection BubbleProjection
        {
            get { return bubbleProjection; }
        }

        public BubblePathRenderer BubblePathRenderer
        {
            get { return bubblePathRenderer; }
        }

        public BubbleShootController BubbleShootController
        {
            get { return bubbleShotController; }
        }

        public PlayerRaycastController RaycastController
        {
            get { return raycastController; }
        }

        public void Init()
        {
            bubbleProjection.Init();
            bubblePathRenderer.Init();
            bubbleShotController.Init();
            raycastController.Init();
        }
    }
}