using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bubbles
{
    public class BubblePathRenderer : MonoBehaviour
    {
        [SerializeField]
        private PlayerRaycastController raycastController;
        [SerializeField]
        private new LineRenderer renderer;

        public void Init()
        {
            renderer.positionCount = 0;
            raycastController.OnPathChanged += OnPathChanged;
        }

        private void OnPathChanged(List<Vector3> path)
        {
            renderer.positionCount = path.Count;
            renderer.SetPositions(path.ToArray());
        }
    }
}