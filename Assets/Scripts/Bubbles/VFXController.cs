using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bubbles
{
    public class VFXController : MonoBehaviour
    {
        public const string EXPLOSION_EFFECT_ID = "BubblesExplosion";
        public const string MERGE_EFFECT_ID = "BubblesMerge";

        [SerializeField]
        private VFXParticleEffectsPool effectsPool;
        private SessionController sessionController;

        private BubblesSettings bubbleSettings;

        public void Init()
        {
            sessionController = SessionController.Instance;
            sessionController.BubblesController.OnExplosion += OnExplosion;
            sessionController.BubblesController.OnBubbleReleased += OnBubbleReleased;
            bubbleSettings = ResourceManager.GetResource<BubblesSettings>(GameConstants.BUBBLE_SETTINGS);
        }

        private void OnBubbleReleased(Bubble bubble)
        {
            var bubbleSettingsIndex = bubbleSettings.Bubbles.FindIndex(x => x.number == bubble.CurrentNumber);
            if (bubbleSettingsIndex == -1)
                return;

            var bubbleData = bubbleSettings.Bubbles[bubbleSettingsIndex];

            var effect = effectsPool.GetOrInstantiate(MERGE_EFFECT_ID);
            effect.transform.position = bubble.transform.position;

            effect.SetColor(bubbleData.backColor);
            effect.Init(effectsPool);
        }

        private void SpawnEffect(string id,int x, int y, Vector3 position)
        {
            var effect = effectsPool.GetOrInstantiate(id);
            effect.transform.position = position;
            effect.Init(effectsPool);
        }

        private void OnExplosion(ExplosionInfo info)
        {
            var effect = effectsPool.GetOrInstantiate(EXPLOSION_EFFECT_ID);
            effect.transform.position = sessionController.BubblesController.GetGlobalSpawnPosition(info.x, info.y);
            effect.Init(effectsPool);
        }

        private void OnMerge(MergeInfo info)
        {
            var bubbleSettingsIndex = bubbleSettings.Bubbles.FindIndex(x => x.number == Bubble.GetNumber(info.power));
            if (bubbleSettingsIndex == -1)
                return;

            var bubbleData = bubbleSettings.Bubbles[bubbleSettingsIndex];

            var effect = effectsPool.GetOrInstantiate(MERGE_EFFECT_ID);
            effect.transform.position = sessionController.BubblesController.GetGlobalSpawnPosition(info.x, info.y);

            effect.SetColor(bubbleData.backColor);
            effect.Init(effectsPool);
        }
    }
}