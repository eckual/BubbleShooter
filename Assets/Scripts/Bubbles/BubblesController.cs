using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

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
        private const float INPUT_ANGLE_IN_RADIANS = Mathf.PI / 3;
        private const int SIDES_COUNT = 6;

        private int maxPower;
        private int minPower;
        private int currentPower;

        private int minX;
        private int maxX;

        private int minY;
        private int maxY;
        private int deadlineY;
        private int bubblesVisibleCount;

        private int initialRowsCount;
        private int initialColumnsCount;

        private int generationMinPower;
        private int generationMaxPower;

        private float moveSpeed;
        private float fallingSpeed;
        [SerializeField]
        private Transform fallingReleasePosition;

        private bool isRootMoving;
        private Vector3 rootInitialPosition;
        private Vector3 rootNextPosition;

        [SerializeField]
        private BubblesPool bubblesPool;

        private BubblesSettings settings;
        #region Filters
        private BubblesCollection bubbles = new BubblesCollection();
        private BubblesCollection merged = new BubblesCollection();
        private BubblesCollection highest = new BubblesCollection();
        private BubblesCollection askedAttached = new BubblesCollection();
        private BubblesCollection falling = new BubblesCollection();
        private BubblesCollection notFalling = new BubblesCollection();
        #endregion

        public event Action<Bubble> OnBubbleAdded;
        public event Action<Bubble> OnBubbleReleased;
        public event Action<MergeInfo> OnMerge;
        public event Action<ExplosionInfo> OnExplosion;
        public event Action OnBubblesCleared;
        public event Action<int> OnCurrentPowerChanged;

        public int MinX
        {
            get { return minX; }
        }

        public int MaxX
        {
            get { return maxX; }
        }

        public int MinY
        {
            get { return minY; }
        }

        public int MaxY
        {
            get { return maxY; }
        }

        public int DeadlineY
        {
            get { return deadlineY; }
        }

        public BubblesCollection Bubbles
        {
            get { return bubbles; }
        }

        public int CurrentPower
        {
            get { return currentPower; }
            set
            {
                if (currentPower != value)
                {
                    currentPower = value;
                    OnCurrentPowerChanged?.Invoke(currentPower);
                }
            }
        }

        public bool Locked { get; set; }

        public void Init()
        {
            settings = ResourceManager.GetResource<BubblesSettings>(GameConstants.BUBBLE_SETTINGS);
            minPower = 1;
            maxPower = settings.Bubbles.Count;
            minX = 0;
            maxX = settings.Width;
            minY = 0;
            maxY = settings.Height;
            deadlineY = minY + 1;
            moveSpeed = settings.MoveSpeed;
            fallingSpeed = settings.FallingSpeed;
            bubblesVisibleCount = settings.BubblesVisibleCount;
            initialRowsCount = settings.InitialRowsCount;
            initialColumnsCount = settings.InitialColumnsCount;
            rootInitialPosition = bubblesPool.Root.transform.localPosition;
            generationMinPower = settings.GenerationMinPower;
            generationMaxPower = settings.GenerationMaxPower;

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

        public void SpawnBubbles()
        {
            for (int y = maxY - 1; y >= maxY  - initialRowsCount; y--)
            {
                for (int x = 0; x < initialColumnsCount; x++)
                {
                    var bubble = bubblesPool.GetOrInstantiate(BUBBLE_ID);
                    bubble.transform.localPosition = GetLocalSpawnPosition(x, y);
                    bubble.X = x;
                    bubble.Y = y;
                    bubbles.Add(bubble);

                    bubble.Power = UnityEngine.Random.Range(generationMinPower, generationMaxPower);
                    bubble.gameObject.SetActive(true);

                    OnBubbleAdded?.Invoke(bubble);
                }
            }
            LinkBubbles();
        }

        public Bubble SpawnBubble(int x, int y, int power = 1, bool link = true,bool merge = true)
        {
            var bubble = bubblesPool.GetOrInstantiate(BUBBLE_ID);
            bubble.transform.localPosition = GetLocalSpawnPosition(x, y);
            bubble.X = x;
            bubble.Y = y;
            bubble.Power = power;
            bubbles.Add(bubble);
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

                if (link == null || link.Power != bubble.Power || prevBubble == link)
                    continue;

                if (!linked.Contains(link))
                {
                    linked.Add(link);
                    CheckBubblePower(bubble, link, linked);
                }
            }
        }

        private void CheckAutomatic(int power, ref int x, ref int y)
        {
            highest.Clear();
            for (int i = 0; i < merged.Count; i++)
            {
                var bubble = merged[i];
                for (int j = 0; j < SIDES_COUNT; j++)
                {
                    var link = bubble.GetLinked((BubbleSide)j);
                    if (link && link.Power == power)
                    {
                        x = bubble.X;
                        y = bubble.Y;
                        return;
                    }
                }
            }

            var maxY = merged.Max(bubble => bubble.Y);
            highest.AddRange(merged.FindAll(bubble => bubble.Y == maxY));

            var projectileBubble = merged[merged.Count - 1];
            var minXToProjectile = highest.Min(bubble => Mathf.Abs(bubble.X - projectileBubble.X));

            var closestToFirst = highest.Find(bubble => Mathf.Abs(bubble.X - projectileBubble.X) == minXToProjectile);
            if (closestToFirst)
            {
                x = closestToFirst.X;
                y = closestToFirst.Y;
            }
        }

        private void CheckAttachedToTop(Bubble bubble, ref bool attached)
        {
            if (bubble == null || merged.Contains(bubble))
                return;

            for(int i = 0;  i < SIDES_COUNT; i++)
            {
                var bubbleSide = i;
                var link = bubble.GetLinked((BubbleSide)bubbleSide);
                if (link == null || merged.Contains(link))
                    continue;

                if(link.Y == maxY - 1)
                {
                    attached = true;
                    break;
                }
                if (!askedAttached.Contains(link))
                {
                    askedAttached.Add(link);
                    CheckAttachedToTop(link, ref attached);
                }
            }
        }

        private void GetAllLinks(Bubble bubble, List<Bubble> buffer)
        {
            for(int i = 0; i < SIDES_COUNT; i++)
            {
                var bubbleSide = i;
                var link = bubble.GetLinked((BubbleSide)bubbleSide);

                if (link == null || buffer.Contains(link) || merged.Contains(link))
                    continue;

                buffer.Add(link);
                GetAllLinks(link, buffer);
            }
        }

        private void CheckMergedFall()
        {
            askedAttached.Clear();
            for (int i = 0; i < merged.Count; i++)
            {
                var bubble = merged[i];
                for(int j = 0; j < SIDES_COUNT; j++)
                {
                    askedAttached.Clear();

                    var link = bubble.GetLinked((BubbleSide)j);
                    if (link == null || merged.Contains(link) || link.Y == MaxY-1)
                        continue;

                    var attached = false;
                    CheckAttachedToTop(link, ref attached);

                    if (!attached)
                    {
                        if (!falling.Contains(link))
                            falling.Add(link);
                        GetAllLinks(link, falling);
                    }
                }
            }
        }

        private void CheckFall(Bubble bubble, out bool attached)
        {
            attached = false;
            CheckAttachedToTop(bubble, ref attached);
            if (!attached) falling.Add(bubble);
        }
        
        public void TryMerge(Bubble bubble)
        {
            merged.Clear();
            CheckBubblePower(null, bubble, merged);

            if (merged.Count > 0)
            {
                var x = bubble.X;
                var y = bubble.Y;
                var power = Mathf.Clamp(bubble.Power + merged.Count, minPower, maxPower);

                merged.Add(bubble);
                CheckAutomatic(power, ref x, ref y);

                CheckMergedFall();

                OnMerge?.Invoke(new MergeInfo()
                {
                    x = x,
                    y = y,
                    power = power,
                    involvedCount = merged.Count
                });

                for (int i = merged.Count -1 ; i >= 0; i--)
                {
                    ReleaseBubble(merged[i]);
                }

                var newBubble = SpawnBubble(x, y, power);
                if (newBubble.X != -1 && newBubble.Y != -1)
                {
                    CheckFall(newBubble, out bool attached);
                    if (attached)
                    {
                        GetAllLinks(newBubble, notFalling);

                        for (int i = 0; i < notFalling.Count; i++)
                        {
                            var notFallingBubble = notFalling[i];
                            if (falling.Contains(notFallingBubble))
                                falling.Remove(notFallingBubble);
                        }

                        notFalling.Clear();
                        if (power == maxPower) Explode(newBubble);
                    }
                }
            }
            CheckToClear();
        }

        private void CheckToClear()
        {
            var intersected = falling.Intersect(bubbles).Count();
            if (bubbles.Count - intersected == 0)
            {
                OnBubblesCleared?.Invoke();

                minX = 0;
                maxX = settings.Width;
                minY = 0;
                maxY = settings.Height;
                deadlineY = minY + 1;

                for (int i = falling.Count - 1; i >= 0; i--)
                {
                    var bubble = falling[i];
                    ReleaseBubble(bubble);
                }

                SpawnBubbles();
                rootNextPosition = rootInitialPosition;
                bubblesPool.Root.transform.localPosition = rootInitialPosition + initialRowsCount* Vector3.up * Mathf.Sin(INPUT_ANGLE_IN_RADIANS);
                isRootMoving = true;
            }
        }

        public void Explode(Bubble bubble)
        {
            var x = bubble.X;
            var y = bubble.Y;
            var power = bubble.Power;

            merged.Add(bubble);
            for (int bubbleSide = 0; bubbleSide < SIDES_COUNT; bubbleSide++)
            {
                var link = bubble.GetLinked((BubbleSide)bubbleSide);
                if (!link)
                    continue;
                if(!merged.Contains(link))
                    merged.Add(link);
            }

            CheckMergedFall();

            for (int i = merged.Count - 1; i >= 0; i--)
            {
                ReleaseBubble(merged[i]);
            }

            OnExplosion?.Invoke(new ExplosionInfo()
            {
                x = x,
                y = y,
                power = power
            });
        }

        public void MoveBubbles()
        {
            if (isRootMoving)
                return;

            var bubblesMinY = bubbles.Min(bubble => bubble.Y);
            if (bubblesMinY >= deadlineY)
            {
                if (bubbles.Count <= bubblesVisibleCount)
                    LowerBubbles();
            }
            else
            {
                RiseBubbles();
            }
        }

        private void RiseBubbles()
        {
            maxY--;
            minY--;
            deadlineY--;

            rootNextPosition = bubblesPool.Root.localPosition + Vector3.up * Mathf.Sin(INPUT_ANGLE_IN_RADIANS);
            isRootMoving = true;
        }

        private void LowerBubbles()
        {
            maxY++;
            minY++;
            deadlineY++;

            for (int x = 0; x < maxX; x++)
            {
                var bubble = bubblesPool.GetOrInstantiate(BUBBLE_ID);
                bubble.transform.localPosition = GetLocalSpawnPosition(x, maxY-1);
                bubble.X = x;
                bubble.Y = maxY-1;
                bubbles.Add(bubble);

                bubble.Power = UnityEngine.Random.Range(generationMinPower, generationMaxPower);
                bubble.gameObject.SetActive(true);

                OnBubbleAdded?.Invoke(bubble);
            }
            LinkBubbles();

            rootNextPosition = bubblesPool.Root.localPosition + Vector3.down * Mathf.Sin(INPUT_ANGLE_IN_RADIANS);
            isRootMoving = true;
        }

        private void OnRootMovedFinished()
        {
            for (int i = bubbles.Count - 1; i >= 0; i--)
            {
                var bubble = bubbles[i];
                if (bubble.Y == maxY)
                    ReleaseBubble(bubble);
            }

            LinkBubbles();
        }

        public void ReleaseBubble(Bubble bubble)
        {
            bubbles.Remove(bubble);
            falling.Remove(bubble);
            highest.Remove(bubble);
            askedAttached.Remove(bubble);
            OnBubbleReleased?.Invoke(bubble);
            bubblesPool.Release(bubble);
            bubble.gameObject.SetActive(false);
        }

        public void LinkBubbles()
        {

            for (int i = 0; i < bubbles.Count; i++)
            {
                var bubble = bubbles[i];

                bubble.LinkBubble(BubbleSide.Right, bubbles[bubble.X + 1, bubble.Y]);
                bubble.LinkBubble(BubbleSide.Left, bubbles[bubble.X - 1, bubble.Y]);

                if (bubble.Y % 2 != 0)
                {
                    bubble.LinkBubble(BubbleSide.BotRight, bubbles[bubble.X + 1, bubble.Y - 1]);
                    bubble.LinkBubble(BubbleSide.BotLeft, bubbles[bubble.X, bubble.Y - 1]);

                    bubble.LinkBubble(BubbleSide.TopRight, bubbles[bubble.X + 1, bubble.Y + 1]);
                    bubble.LinkBubble(BubbleSide.TopLeft, bubbles[bubble.X, bubble.Y + 1]);
                }
                else
                {
                    bubble.LinkBubble(BubbleSide.BotRight, bubbles[bubble.X, bubble.Y - 1]);
                    bubble.LinkBubble(BubbleSide.BotLeft, bubbles[bubble.X - 1, bubble.Y - 1]);

                    bubble.LinkBubble(BubbleSide.TopRight, bubbles[bubble.X, bubble.Y + 1]);
                    bubble.LinkBubble(BubbleSide.TopLeft, bubbles[bubble.X - 1, bubble.Y + 1]);
                }
            }
        }


        private void Update()
        {
            Locked = falling.Count > 0 || isRootMoving;

            if (Locked)
            {
                for (int i = falling.Count - 1; i >= 0; i--)
                {
                    var fallingBubble = falling[i];
                    fallingBubble.transform.position += Vector3.down * fallingSpeed * Time.deltaTime;
                    if (fallingBubble.transform.position.y < fallingReleasePosition.position.y)
                    {
                        ReleaseBubble(fallingBubble);
                    }
                }

                if (isRootMoving)
                {
                    bubblesPool.Root.position = Vector3.MoveTowards(bubblesPool.Root.position,
                                                                    rootNextPosition,
                                                                    moveSpeed * Time.deltaTime);
                    if (bubblesPool.Root.position.ApproximatelyEqual(rootNextPosition))
                    {
                        isRootMoving = false;
                        OnRootMovedFinished();
                    }
                }
            }
        }
    }
}