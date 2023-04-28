using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bubbles
{
    public class BubbleShootController : MonoBehaviour
    {
        [SerializeField]
        private PlayerRaycastController raycastController;
        private List<Vector3> cachedPath = new List<Vector3>();

        [SerializeField]
        private float projectileSpeed;
        [SerializeField]
        private BubbleProjectile projectile;

        private Vector3 initialPosition;
        private Vector3 targetPosition;
        private bool isMoving = false;
        private int cachedX;
        private int cachedY;

        public event Action OnShootStarted;
        public event Action OnShootEnded;

        private SessionController sessionController;

        public void Init()
        {
            sessionController = SessionController.Instance;
            raycastController.OnPathChanged += OnPathChanged;
            raycastController.OnBubbleChanged += OnBubbleChanged;
            raycastController.OnStopRaycasting += OnStopRaycasting;
            initialPosition = projectile.transform.position;
            targetPosition = initialPosition;
            projectile.gameObject.SetActive(isMoving);
        }

        private void OnBubbleChanged(int x, int y, Vector3 position)
        {
            if (!isMoving)
            {
                var bubblesController = sessionController.BubblesController;
                if (!bubblesController.Bubbles[x, y])
                {
                    targetPosition = position;
                    cachedX = x;
                    cachedY = y;
                }
                else
                {
                    cachedX = PlayerRaycastController.DEFAULT_X;
                    cachedY = PlayerRaycastController.DEFAULT_Y;
                }
            }
        }

        private void OnPathChanged(List<Vector3> path)
        {
            if (!isMoving)
            {
                cachedPath.Clear();
                cachedPath.AddRange(path);
                if (cachedPath.Count > 0)
                {
                    cachedPath[cachedPath.Count - 1] = targetPosition;
                }

                if(cachedX == PlayerRaycastController.DEFAULT_X ||
                   cachedY == PlayerRaycastController.DEFAULT_Y)
                {
                    cachedPath.Clear();
                }
            }
        }

        private void StartShoot()
        {
            isMoving = true;
            projectile.gameObject.SetActive(isMoving);
            OnShootStarted?.Invoke();

            projectile.Init(sessionController.BubblesController.CurrentPower);
        }

        private void EndShoot()
        {
            isMoving = false;
            projectile.transform.position = initialPosition;
            projectile.transform.rotation = Quaternion.identity;
            projectile.gameObject.SetActive(isMoving);

            var bubblesController = sessionController.BubblesController;
            bubblesController.SpawnBubble(cachedX, cachedY, bubblesController.CurrentPower);
            bubblesController.MoveBubbles();
            OnShootEnded?.Invoke();
        }

        private void OnStopRaycasting()
        {
            if(sessionController.BubblesController.Locked)
                return;

            if (!isMoving && cachedPath.Count > 0)
            {
                StartShoot();
            }
        }

        private void Update()
        {
            if (isMoving)
            {
                if (cachedPath.Count > 0)
                {
                    var point = cachedPath[0];
                    if (point.ApproximatelyEqual(projectile.transform.position))
                    {
                        cachedPath.RemoveAt(0);
                    }
                    else
                    {

                        var direction = point - projectile.transform.position;
                        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
                        projectile.transform.rotation = Quaternion.Slerp(projectile.transform.rotation,
                                                                         Quaternion.AngleAxis(angle, Vector3.forward),
                                                                         Time.deltaTime * projectileSpeed);

                        projectile.transform.position = Vector3.MoveTowards(projectile.transform.position,
                                                                            point,
                                                                            Time.deltaTime * projectileSpeed);
                    }
                }
                else
                {
                    EndShoot();
                }
            }
        }
    }
}