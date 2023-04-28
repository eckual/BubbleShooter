using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bubbles
{
    public class BubbleProjectile : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer back;
        [SerializeField]
        private SpriteRenderer border;

        private BubblesSettings settings;

        public SpriteRenderer Back
        {
            get { return back; }
        }

        public SpriteRenderer Border
        {
            get { return border; }
        }

        public BubblesSettings Settings
        {
            get
            {
                if (!settings)
                {
                    settings = ResourceManager.GetResource<BubblesSettings>(GameConstants.BUBBLE_SETTINGS);
                }
                return settings;
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