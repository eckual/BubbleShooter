using UnityEngine;
using Utils;

namespace Bubbles
{
    public class BubbleProjectile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer back;
        [SerializeField] private SpriteRenderer border;

        private BubblesSettings _settings;

        private BubblesSettings Settings
        {
            get
            {
                if (_settings == null) _settings = ResourceManager.GetResource<BubblesSettings>(GameConstants.BubbleSettings);
                return _settings;
            }
        }

        public void Init(int power)
        {
            var bubbleDataIndex = Settings.Bubbles.FindIndex(x => x.number == Bubble.GetNumber(power));
            if (bubbleDataIndex == -1)
                return;

            var bubbleData = Settings.Bubbles[bubbleDataIndex];
            back.color = bubbleData.backColor;
            border.color = bubbleData.borderColor;
        }
        
    }
}