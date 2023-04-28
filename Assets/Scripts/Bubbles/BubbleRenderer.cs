using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bubbles
{
    public class BubbleRenderer : MonoBehaviour
    {
        [SerializeField]
        private Bubble bubble;
        [SerializeField]
        private SpriteRenderer backRenderer;
        [SerializeField]
        private SpriteRenderer borderRenderer;

        private void OnEnable()
        {
            var settings = ResourceManager.GetResource<BubblesSettings>(GameConstants.BUBBLE_SETTINGS);
            var itemIndex = settings.Bubbles.FindIndex(x => x.number == bubble.CurrentNumber);
            if(itemIndex != -1)
            {
                var item = settings.Bubbles[itemIndex];
                backRenderer.color = item.backColor;
                borderRenderer.color = item.borderColor;
            }
        }
    }
}