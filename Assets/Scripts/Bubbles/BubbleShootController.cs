using System;
using System.Collections.Generic;
using Controllers;
using Extensions;
using UnityEngine;

namespace Bubbles
{
    public class BubbleShootController : ControllerBase
    {
        public event Action OnShootStarted;
        public event Action OnShootEnded;
        
        [SerializeField] private PlayerRaycastController raycastController;
        [SerializeField] private BubbleProjectile projectile;
        [SerializeField] private float projectileSpeed;

        private SessionController _sessionController;
        private List<Vector3> _cachedPath = new();
        private Vector3 _initialPosition;
        private Vector3 _targetPosition;
        private bool _isMoving = false;
        private int _cachedX;
        private int _cachedY;

        public override void Init()
        {
            _sessionController = SessionController.Instance;
            raycastController.OnPathChanged += OnPathChanged;
            raycastController.OnBubbleChanged += OnBubbleChanged;
            raycastController.OnStopRaycasting += OnStopRaycasting;
            _initialPosition = projectile.transform.position;
            _targetPosition = _initialPosition;
            projectile.gameObject.SetActive(_isMoving);
        }

        private void OnBubbleChanged(int x, int y, Vector3 position)
        {
            if (_isMoving) return;

            var bubblesController = _sessionController.BubblesController;
            if (!bubblesController.Bubbles[x, y])
            {
                _targetPosition = position;
                _cachedX = x;
                _cachedY = y;
                return;
            }

            _cachedX = PlayerRaycastController.DEFAULT_X;
            _cachedY = PlayerRaycastController.DEFAULT_Y;
        }

        private void OnPathChanged(List<Vector3> path)
        {
            if (_isMoving) return;
            
            _cachedPath.Clear();
            _cachedPath.AddRange(path);
            if (_cachedPath.Count > 0)
            {
                _cachedPath[_cachedPath.Count - 1] = _targetPosition;
            }

            if(_cachedX == PlayerRaycastController.DEFAULT_X || _cachedY == PlayerRaycastController.DEFAULT_Y)
                _cachedPath.Clear();
        }

        private void StartShoot()
        {
            _isMoving = true;
            projectile.gameObject.SetActive(_isMoving);
            OnShootStarted?.Invoke();

            projectile.Init(_sessionController.BubblesController.CurrentPower);
        }

        private void EndShoot()
        {
            _isMoving = false;
            projectile.transform.position = _initialPosition;
            projectile.transform.rotation = Quaternion.identity;
            projectile.gameObject.SetActive(_isMoving);

            var bubblesController = _sessionController.BubblesController;
            bubblesController.SpawnBubble(_cachedX, _cachedY, bubblesController.CurrentPower);
            bubblesController.MoveBubbles();
            OnShootEnded?.Invoke();
        }

        private void OnStopRaycasting()
        {
            if(_sessionController.BubblesController.Locked)
                return;

            if (!_isMoving && _cachedPath.Count > 0) StartShoot();
        }

        private void Update()
        {
            if (!_isMoving) return;
            if (_cachedPath.Count > 0)
            {
                var point = _cachedPath[0];
                if (point.ApproximatelyEqual(projectile.transform.position))
                {
                    _cachedPath.RemoveAt(0);
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
                EndShoot();
        }
    }
}