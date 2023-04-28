using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bubbles
{
    public class PlayerRaycastController : MonoBehaviour
    {
        public const string BUBBLE_TAG = "Bubble";
        public const string WALL_TAG = "Wall";
        public const int DEFAULT_X = 1000;
        public const int DEFAULT_Y = 1000;

        [SerializeField]
        private new Camera camera;

        private GameInputModule inputModule;
        public GameInputModule InputModule
        {
            get { return inputModule; }
        }

        private int minX;
        private int maxX;
        private RaycastHit2D hit;
        private RaycastHit2D wallHit;

        [SerializeField]
        private int x;
        [SerializeField]
        private int y;
        [SerializeField]
        private Vector3 position;
        public event Action<int, int, Vector3> OnBubbleChanged;

        [SerializeField]
        private List<Vector3> path = new List<Vector3>();
        public event Action<List<Vector3>> OnPathChanged;

        public event Action OnStartRaycasting;
        public event Action OnStopRaycasting;

        private SessionController sessionController;

        public void Init()
        {
            sessionController = SessionController.Instance;

            minX = 0;
            maxX = sessionController.BubblesController.MaxX;
            x = DEFAULT_X;
            y = DEFAULT_Y;
            position = transform.position;

            if (!inputModule)
            {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                inputModule = gameObject.AddComponent<GameTouchInputModule>();
#elif UNITY_STANDALONE || UNITY_EDITOR
                inputModule = gameObject.AddComponent<GameStandaloneInputModule>();
#else
                inputModule = gameObject.AddComponent<GameInputModule>();
#endif
                inputModule.OnInputEnd += OnInputEnd;
                inputModule.OnInputStart += OnInputStart;
                inputModule.OnInputStay += OnInputStay;
            }
        }

        private void ProceedBubbleRaycastHit(RaycastHit2D hit)
        {
            var bubble = hit.transform.GetComponent<Bubble>();
            if (Mathf.Abs(hit.transform.position.y - hit.point.y) <= 0.25f)
            {
                y = bubble.Y;
                x = hit.transform.position.x < hit.point.x ? bubble.X + 1 : bubble.X - 1;
            }
            else
            {

                if (bubble.Y % 2 != 0)
                {
                    x = hit.transform.position.x < hit.point.x && bubble.X + 1 < maxX ? bubble.X + 1 : bubble.X;
                    y = hit.transform.position.y < hit.point.y ? bubble.Y + 1 : bubble.Y - 1;
                }
                else
                {
                    x = hit.transform.position.x < hit.point.x && bubble.X < maxX ? bubble.X : bubble.X - 1;
                    y = hit.transform.position.y < hit.point.y ? bubble.Y + 1 : bubble.Y - 1;
                }
            }

            if (x >= minX && x < maxX)
            {
                var bubblesController = sessionController.BubblesController;
                var linkedBubble = bubblesController.Bubbles[x, y];
                if (!linkedBubble)
                {
                    position = hit.transform.parent.position + bubblesController.GetLocalSpawnPosition(x, y);
                    OnBubbleChanged?.Invoke(x, y, position);
                    return;
                }
            }

            x = DEFAULT_X;
            y = DEFAULT_Y;
            position = transform.position;
            OnBubbleChanged?.Invoke(x, y, position);
        }

        private void OnInputStart()
        {
            if (!sessionController.IsRunning)
                return;

            OnStartRaycasting?.Invoke();
        }

        private void OnInputStay()
        {
            if (!sessionController.IsRunning)
                return;

            var cameraPosition = camera.ScreenToWorldPoint(inputModule.InputPosition);
            hit = Physics2D.Raycast(transform.position, new Vector3(cameraPosition.x, cameraPosition.y, transform.position.z) - transform.position);
            if (hit)
            {
                if (hit.collider && hit.collider.gameObject.CompareTag(BUBBLE_TAG))
                {
                    ProceedBubbleRaycastHit(hit);
                    path.Clear();
                    path.Add(transform.position);
                    path.Add(hit.point);
                    OnPathChanged?.Invoke(path);
                }
                else if (hit.transform.gameObject.CompareTag(WALL_TAG))
                {
                    var offset = hit.point.x > transform.position.x ? -0.01f : 0.01f;
                    var reflectedOrigin = new Vector3(hit.point.x + offset, hit.point.y, transform.position.z);
                    var reflectedY = 2 * (reflectedOrigin.y - transform.position.y) + transform.position.y;

                    wallHit = Physics2D.Raycast(reflectedOrigin, new Vector3(transform.position.x, reflectedY, transform.position.z) - reflectedOrigin);

                    if (wallHit.collider && wallHit.collider.CompareTag(BUBBLE_TAG))
                    {
                        ProceedBubbleRaycastHit(wallHit);
                        path.Clear();
                        path.Add(transform.position);
                        path.Add(hit.point);
                        path.Add(wallHit.point);
                        OnPathChanged?.Invoke(path);
                    }
                }
            }
        }

        private void OnInputEnd()
        {
            if (!sessionController.IsRunning)
                return;

            OnStopRaycasting?.Invoke();
            path.Clear();
            OnPathChanged?.Invoke(path);
        }
    }
}