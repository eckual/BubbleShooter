using System;
using System.Linq;
using System.Collections.Generic;
using Extensions;
using UnityEngine;
using Utils;

namespace Bubbles
{
    [Serializable]
    public class BubblesCollection : List<Bubble>
    {
        public Bubble this[int X, int Y]
        {
            get { return Find(bubble => bubble.X == X && bubble.Y == Y); }
        }
    }
    
    [Serializable]
    public struct MergeInfo
    {
        public int x;
        public int y;
        public int power;
        public int involvedCount;
    }

    [Serializable]
    public struct ExplosionInfo
    {
        public int x;
        public int y;
        public int power;
    }

    public class BubblesController : MonoBehaviour
    {
        private const string BUBBLE_ID = "Bubble";
        private const float INPUT_ANGLE_IN_RADIANS = Mathf.PI * 0.33f;
        private const int SIDES_COUNT = 6;
        
        public event Action<Bubble> OnBubbleAdded;
        public event Action<Bubble> OnBubbleReleased;
        public event Action<MergeInfo> OnMerge;
        public event Action<ExplosionInfo> OnExplosion;
        public event Action OnBubblesCleared;
        public event Action<int> OnCurrentPowerChanged;
        
        [SerializeField] private BubblesPool bubblesPool;
        [SerializeField] private Transform fallingReleasePosition;

        private BubblesSettings _settings;
        private BubblesCollection _bubbles = new BubblesCollection();
        private BubblesCollection _merged = new BubblesCollection();
        private BubblesCollection _highest = new BubblesCollection();
        private BubblesCollection _askedAttached = new BubblesCollection();
        private BubblesCollection _falling = new BubblesCollection();
        private BubblesCollection _notFalling = new BubblesCollection();
        public BubblesCollection Bubbles => _bubbles;
        
        private int _maxPower;
        private int _minPower;
        private int _currentPower;

        private int _minX;
        private int _maxX;

        private int _minY;
        private int _maxY;
        private int _deadlineY;
        private int _bubblesVisibleCount;

        private int _initialRowsCount;
        private int _initialColumnsCount;

        private int _generationMinPower;
        private int _generationMaxPower;

        private float _moveSpeed;
        private float _fallingSpeed;

        private bool _isRootMoving;
        private Vector3 _rootInitialPosition;
        private Vector3 _rootNextPosition;

        public int MaxX => _maxX;


        public int CurrentPower
        {
            get => _currentPower;
            set
            {
                if (_currentPower == value) return;
                
                _currentPower = value;
                OnCurrentPowerChanged?.Invoke(_currentPower);
            }
        }

        public bool Locked { get; private set; }

        public void Init()
        {
            _settings = ResourceManager.GetResource<BubblesSettings>(GameConstants.BubbleSettings);
            _minPower = 1;
            _maxPower = _settings.Bubbles.Count;
            _minX = 0;
            _maxX = _settings.Width;
            _minY = 0;
            _maxY = _settings.Height;
            _deadlineY = _minY + 1;
            _moveSpeed = _settings.MoveSpeed;
            _fallingSpeed = _settings.FallingSpeed;
            _bubblesVisibleCount = _settings.BubblesVisibleCount;
            _initialRowsCount = _settings.InitialRowsCount;
            _initialColumnsCount = _settings.InitialColumnsCount;
            _rootInitialPosition = bubblesPool.Root.transform.localPosition;
            _generationMinPower = _settings.GenerationMinPower;
            _generationMaxPower = _settings.GenerationMaxPower;

            CurrentPower = 1;
            SpawnBubbles();
        }

        public Vector3 GetLocalSpawnPosition(int x, int y)
        {
            return new Vector3(x + (y % 2 == 0 ? 0 : Mathf.Cos(INPUT_ANGLE_IN_RADIANS)), (y + 1) * Mathf.Sin(INPUT_ANGLE_IN_RADIANS), 0);
        }

        public Vector3 GetGlobalSpawnPosition(int x, int y)
        {
            return bubblesPool.Root.transform.position + GetLocalSpawnPosition(x, y);
        }

        private void SpawnBubbles()
        {
            for (var i = _maxY - _initialRowsCount; i < _maxY; i++)
            {
                for (var j = 0; j < _initialColumnsCount; j++)
                {
                    var bubble = bubblesPool.GetOrInstantiate(BUBBLE_ID);
                    bubble.transform.localPosition = GetLocalSpawnPosition(j, i);
                    bubble.X = j;
                    bubble.Y = i;
                    _bubbles.Add(bubble);

                    bubble.Power = UnityEngine.Random.Range(_generationMinPower, _generationMaxPower);
                    bubble.gameObject.SetActive(true);

                    OnBubbleAdded?.Invoke(bubble);
                }
            }
            LinkBubbles();
        }

        public Bubble SpawnBubble(int xPos, int y, int power = 1, bool link = true, bool merge = true)
        {
            var bubble = bubblesPool.GetOrInstantiate(BUBBLE_ID);
            bubble.transform.localPosition = GetLocalSpawnPosition(xPos, y);
            bubble.X =xPos;
            bubble.Y = y;
            bubble.Power = power;
            _bubbles.Add(bubble);
            bubble.gameObject.SetActive(true);
            OnBubbleAdded?.Invoke(bubble);
            if (link) LinkBubbles();
            if (merge) TryMerge(bubble);
            
            return bubble;
        }

        private void CheckBubblePower(Bubble prevBubble, Bubble bubble, List<Bubble> linked)
        {
            for (int i = 0; i < SIDES_COUNT; i++)
            {
                var bubbleSide = i;
                var link = bubble.GetLinked((BubbleSide)bubbleSide);

                if (link == null || link.Power != bubble.Power || prevBubble == link) continue;
                if (linked.Contains(link)) continue;
                
                linked.Add(link);
                CheckBubblePower(bubble, link, linked);
            }
        }

        private void CheckAutomatic(int power, ref int x, ref int y)
        {
            _highest.Clear();

            for (var index = 0; index < _merged.Count; index++)
            {
                var bubble = _merged[index];
                for (var j = 0; j < SIDES_COUNT; j++)
                {
                    var link = bubble.GetLinked((BubbleSide) j);
                    if (!link || link.Power != power) continue;

                    x = bubble.X;
                    y = bubble.Y;
                    return;
                }
            }

            var maxY = _merged.Max(bubble => bubble.Y);
            _highest.AddRange(_merged.FindAll(bubble => bubble.Y == maxY));

            var projectileBubble = _merged[_merged.Count - 1];
            var minXToProjectile = _highest.Min(bubble => Mathf.Abs(bubble.X - projectileBubble.X));

            var closestToFirst = _highest.Find(bubble => Mathf.Abs(bubble.X - projectileBubble.X) == minXToProjectile);
            if (!closestToFirst) return;
            
            x = closestToFirst.X;
            y = closestToFirst.Y;
        }

        private void CheckAttachedToTop(Bubble bubble, ref bool attached)
        {
            if (bubble == null || _merged.Contains(bubble))
                return;

            for(int i = 0;  i < SIDES_COUNT; i++)
            {
                var bubbleSide = i;
                var link = bubble.GetLinked((BubbleSide)bubbleSide);
                if (link == null || _merged.Contains(link))
                    continue;

                if(link.Y == _maxY - 1)
                {
                    attached = true;
                    break;
                }

                if (_askedAttached.Contains(link)) continue;
                
                _askedAttached.Add(link);
                CheckAttachedToTop(link, ref attached);
            }
        }

        private void GetAllLinks(Bubble bubble, List<Bubble> buffer)
        {
            for(var i = 0; i < SIDES_COUNT; i++)
            {
                var bubbleSide = i;
                var link = bubble.GetLinked((BubbleSide)bubbleSide);

                if (link == null || buffer.Contains(link) || _merged.Contains(link))
                    continue;

                buffer.Add(link);
                GetAllLinks(link, buffer);
            }
        }

        private void CheckMergedFall()
        {
            _askedAttached.ResetList();
            
            foreach (var bubble in _merged)
            {
                for(var j = 0; j < SIDES_COUNT; j++)
                {
                    _askedAttached.Clear();

                    var link = bubble.GetLinked((BubbleSide)j);
                    if (link == null || _merged.Contains(link) || link.Y == _maxY-1)
                        continue;

                    var attached = false;
                    CheckAttachedToTop(link, ref attached);

                    if (attached) continue;
                    if (!_falling.Contains(link))
                        _falling.Add(link);
                    GetAllLinks(link, _falling);
                }
            }
        }

        private void CheckFall(Bubble bubble, out bool attached)
        {
            attached = false;
            CheckAttachedToTop(bubble, ref attached);
            if (!attached) _falling.Add(bubble);
        }

        private void TryMerge(Bubble bubble)
        {
            _merged.ResetList();
            
            CheckBubblePower(null, bubble, _merged);

            if (_merged.Count > 0)
            {
                var x = bubble.X;
                var y = bubble.Y;
                var power = Mathf.Clamp(bubble.Power * (int)Mathf.Pow(2, _merged.Count), _minPower, _maxPower);

                _merged.Add(bubble);
                CheckAutomatic(power, ref x, ref y);

                CheckMergedFall();

                OnMerge?.Invoke(new MergeInfo()
                {
                    x = x,
                    y = y,
                    power = power,
                    involvedCount = _merged.Count
                });

                for (var i = 0; i < _merged.Count; i++)
                {
                    ReleaseBubble(_merged[i]);
                }

                var newBubble = SpawnBubble(x, y, power);
                if (newBubble.X != -1 && newBubble.Y != -1)
                {
                    CheckFall(newBubble, out var attached);
                    if (attached)
                    {
                        GetAllLinks(newBubble, _notFalling);

                        for (var i = 0; i < _notFalling.Count; i++)
                        {
                            var notFallingBubble = _notFalling[i];
                            if (_falling.Contains(notFallingBubble))
                                _falling.Remove(notFallingBubble);
                        }

                        _notFalling.Clear();
                        if (power >= 11) Explode(newBubble);
                    }
                }
            }
            CheckToClear();
        }


        private void CheckToClear()
        {
            var intersected = _falling.Intersect(_bubbles).Count();
            if (_bubbles.Count - intersected != 0) return;
            
            OnBubblesCleared?.Invoke();

            _minX = 0;
            _maxX = _settings.Width;
            _minY = 0;
            _maxY = _settings.Height;
            _deadlineY = _minY + 1;

            for (var i = 0; i < _falling.Count; i++)
            {
                var bubble = _falling[i];
                ReleaseBubble(bubble);
            }

            SpawnBubbles();
            _rootNextPosition = _rootInitialPosition;
            bubblesPool.Root.transform.localPosition = _rootInitialPosition + _initialRowsCount* Vector3.up * Mathf.Sin(INPUT_ANGLE_IN_RADIANS);
            _isRootMoving = true;
        }

        private void Explode(Bubble bubble)
        {
            var x = bubble.X;
            var y = bubble.Y;
            var power = bubble.Power;

            _merged.Add(bubble);
            for (var bubbleSide = 0; bubbleSide < SIDES_COUNT; bubbleSide++)
            {
                var link = bubble.GetLinked((BubbleSide)bubbleSide);
                if (!link)
                    continue;
                if(!_merged.Contains(link))
                    _merged.Add(link);
            }

            CheckMergedFall();

            for (var i = 0; i < _merged.Count; i++) 
                ReleaseBubble(_merged[i]);

            OnExplosion?.Invoke(new ExplosionInfo()
            {
                x = x,
                y = y,
                power = power
            });
        }

        public void MoveBubbles()
        {
            if (_isRootMoving)
                return;

            var bubblesMinY = _bubbles.Min(bubble => bubble.Y);
            if (bubblesMinY >= _deadlineY)
            {
                if (_bubbles.Count <= _bubblesVisibleCount)
                    LowerBubbles();
            }
            else
            {
                RiseBubbles();
            }
        }

        private void RiseBubbles()
        {
            _maxY--;
            _minY--;
            _deadlineY--;

            _rootNextPosition = bubblesPool.Root.localPosition + Vector3.up * Mathf.Sin(INPUT_ANGLE_IN_RADIANS);
            _isRootMoving = true;
        }

        private void LowerBubbles()
        {
            _maxY++;
            _minY++;
            _deadlineY++;

            for (var x = 0; x < _maxX; x++)
            {
                var bubble = bubblesPool.GetOrInstantiate(BUBBLE_ID);
                bubble.transform.localPosition = GetLocalSpawnPosition(x, _maxY-1);
                bubble.X = x;
                bubble.Y = _maxY-1;
                _bubbles.Add(bubble);

                bubble.Power = UnityEngine.Random.Range(_generationMinPower, _generationMaxPower);
                bubble.gameObject.SetActive(true);

                OnBubbleAdded?.Invoke(bubble);
            }
            LinkBubbles();

            _rootNextPosition = bubblesPool.Root.localPosition + Vector3.down * Mathf.Sin(INPUT_ANGLE_IN_RADIANS);
            _isRootMoving = true;
        }

        private void OnRootMovedFinished()
        {
            for (int i = _bubbles.Count - 1; i >= 0; i--)
            {
                var bubble = _bubbles[i];
                if (bubble.Y == _maxY)
                    ReleaseBubble(bubble);
            }

            LinkBubbles();
        }

        private void ReleaseBubble(Bubble bubble)
        {
            _bubbles.Remove(bubble);
            _falling.Remove(bubble);
            _highest.Remove(bubble);
            _askedAttached.Remove(bubble);
            OnBubbleReleased?.Invoke(bubble);
            bubblesPool.Release(bubble);
            bubble.gameObject.SetActive(false);
        }

        public void LinkBubbles()
        {

            for (int i = 0; i < _bubbles.Count; i++)
            {
                var bubble = _bubbles[i];

                bubble.LinkBubble(BubbleSide.Right, _bubbles[bubble.X + 1, bubble.Y]);
                bubble.LinkBubble(BubbleSide.Left, _bubbles[bubble.X - 1, bubble.Y]);

                if (bubble.Y % 2 != 0)
                {
                    bubble.LinkBubble(BubbleSide.BotRight, _bubbles[bubble.X + 1, bubble.Y - 1]);
                    bubble.LinkBubble(BubbleSide.BotLeft, _bubbles[bubble.X, bubble.Y - 1]);

                    bubble.LinkBubble(BubbleSide.TopRight, _bubbles[bubble.X + 1, bubble.Y + 1]);
                    bubble.LinkBubble(BubbleSide.TopLeft, _bubbles[bubble.X, bubble.Y + 1]);
                }
                else
                {
                    bubble.LinkBubble(BubbleSide.BotRight, _bubbles[bubble.X, bubble.Y - 1]);
                    bubble.LinkBubble(BubbleSide.BotLeft, _bubbles[bubble.X - 1, bubble.Y - 1]);

                    bubble.LinkBubble(BubbleSide.TopRight, _bubbles[bubble.X, bubble.Y + 1]);
                    bubble.LinkBubble(BubbleSide.TopLeft, _bubbles[bubble.X - 1, bubble.Y + 1]);
                }
            }
        }


        private void Update()
        {
            Locked = _falling.Count > 0 || _isRootMoving;

            if (!Locked) return;
            
            for (int i = _falling.Count - 1; i >= 0; i--)
            {
                var fallingBubble = _falling[i];
                fallingBubble.transform.position += Vector3.down * _fallingSpeed * Time.deltaTime;
                if (!(fallingBubble.transform.position.y < fallingReleasePosition.position.y)) continue;
                
                ReleaseBubble(fallingBubble);
            }

            if (!_isRootMoving) return;
            
            bubblesPool.Root.position = Vector3.MoveTowards(bubblesPool.Root.position,
                _rootNextPosition,
                _moveSpeed * Time.deltaTime);
            
            if (!bubblesPool.Root.position.ApproximatelyEqual(_rootNextPosition)) return;
            
            _isRootMoving = false;
            OnRootMovedFinished();
            
        }
    }
}