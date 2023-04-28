using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bubbles
{
    public enum BubbleSide : int
    {
        None = -1,
        Right = 0,
        TopRight = 1,
        TopLeft = 2,
        Left = 3,
        BotLeft = 4,
        BotRight = 5
    }

    public class Bubble : MonoBehaviour, IPoolObject
    {
        [SerializeField]
        private string id;
        private Bubble[] linked = new Bubble[6];
        private int power;
        private int currentNumber;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public int Power
        {
            get { return power; }
            set
            {
                power = value;
                currentNumber = GetNumber(power);
            }
        }

        public int CurrentNumber
        {
            get { return currentNumber; }
        }

        public int X { get; set; }
        public int Y { get; set; }

        public Bubble GetLinked(BubbleSide side)
        {
            return side == BubbleSide.None ? null : linked[(int)side];
        }

        public void LinkBubble(BubbleSide side, Bubble bubble)
        {
            if (side == BubbleSide.None || !bubble)
                return;

            linked[(int)side] = bubble;
        }

        public void DetachBubble(BubbleSide side)
        {
            if (side == BubbleSide.None)
                return;

            linked[(int)side] = null;
        }

        public void DetachBubble(Bubble bubble)
        {
            for (int i = 0; i < linked.Length; i++)
            {
                if (linked[i] == bubble)
                {
                    linked[i] = null;
                    break;
                }
            }
        }

        public void Release()
        {
            for (int i = 0; i < linked.Length; i++)
            {
                if (linked[i])
                    linked[i].DetachBubble(this);
                linked[i] = null;
            }
            power = 1;
            X = -1;
            Y = -1;
        }
        public override string ToString()
        {
            return $"[{X},{Y}];P:{Power};Pos:{transform.position}";
        }

        public static int GetNumber(int power)
        {
            return (int)Mathf.Pow(GameConstants.BASE_NUMBER, power);
        }
    }
}