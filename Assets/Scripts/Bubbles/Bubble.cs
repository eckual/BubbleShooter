using UnityEngine;
using Utils;

namespace Bubbles
{
    public enum BubbleSide
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
        [SerializeField] private string id;
        
        private readonly Bubble[] _linked = new Bubble[6];
        private int _power;
        private int _currentScore;

        public string Id
        {
            get => id;
            set => id = value;
        }

        public int Power
        {
            get => _power;
            set
            {
                _power = value;
                _currentScore = GetNumber(_power);
            }
        }

        public int CurrentScore => _currentScore;

        public int X { get; set; }
        public int Y { get; set; }

        public Bubble GetLinked(BubbleSide side)
        {
            return side == BubbleSide.None ? null : _linked[(int)side];
        }

        public void LinkBubble(BubbleSide side, Bubble bubble)
        {
            if (side == BubbleSide.None || !bubble)
                return;

            _linked[(int)side] = bubble;
        }

        private void DetachBubble(Bubble bubble)
        {
            for (var i = 0; i < _linked.Length; i++)
            {
                if (_linked[i] != bubble) continue;
                
                _linked[i] = null;
                break;
            }
        }

        public void Release()
        {
            for (var i = 0; i < _linked.Length; i++)
            {
                if (_linked[i] == null) continue;
                
                _linked[i].DetachBubble(this);
                _linked[i] = null;
            }
            
            _power = 1;
            X = -1;
            Y = -1;
        }
        
        public override string ToString()
        {
            return $"[{X},{Y}];P:{Power};Pos:{transform.position}";
        }

        public static int GetNumber(int power) => (int)Mathf.Pow(GameConstants.BaseNumber, power);
        
    }
}
