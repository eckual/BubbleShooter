using Controllers;
using UnityEngine;
using Utils;

namespace Bubbles
{
    public class VFXController : MonoBehaviour
    {
        private const string EXPLOSION_EFFECT_ID = "BubblesExplosion";
        private const string MERGE_EFFECT_ID = "BubblesMerge";

        [SerializeField] private VFXParticleEffectsPool effectsPool;
        
        private SessionController _sessionController;
        private BubblesSettings _bubbleSettings;

        public void Init()
        {
            _sessionController = SessionController.Instance;
            _sessionController.BubblesController.OnExplosion += OnExplosion;
            _sessionController.BubblesController.OnBubbleReleased += OnBubbleReleased;
            _bubbleSettings = ResourceManager.GetResource<BubblesSettings>(GameConstants.BubbleSettings);
        }

        private void OnBubbleReleased(Bubble bubble)
        {
            var bubbleSettingsIndex = _bubbleSettings.Bubbles.FindIndex(x => x.number == bubble.CurrentScore);
            if (bubbleSettingsIndex == -1)
                return;

            var bubbleData = _bubbleSettings.Bubbles[bubbleSettingsIndex];

            var effect = effectsPool.GetOrInstantiate(MERGE_EFFECT_ID);
            effect.transform.position = bubble.transform.position;

            effect.SetColor(bubbleData.backColor);
            effect.Init(effectsPool);
        }

        private void OnExplosion(ExplosionInfo info)
        {
            var effect = effectsPool.GetOrInstantiate(EXPLOSION_EFFECT_ID);
            effect.transform.position = _sessionController.BubblesController.GetGlobalSpawnPosition(info.x, info.y);
            effect.Init(effectsPool);
        }
        
    }
}