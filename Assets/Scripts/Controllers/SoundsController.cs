using Bubbles;
using Sound;
using UnityEngine;
using Utils;

namespace Controllers
{
    public class SoundsController : MonoSingleton<SoundsController>
    {
        private const string SOUND_MERGE = "Merge";
        private const string SOUND_START_SHOOT = "StartShoot";
        private const string SOUND_END_SHOOT = "EndShoot";

        [SerializeField] private AudioSource audioSource;

        private SoundsStorage _sounds;
        private SessionController _sessionController;

        public override void Init()
        {
            _sounds = ResourceManager.GetResource<SoundsStorage>(GameConstants.SoundsStorage);
            _sessionController = SessionController.Instance;
            _sessionController.BubblesController.OnMerge += OnMerge;
            _sessionController.PlayerController.BubbleShootController.OnShootStarted += OnStartShoot;
            _sessionController.PlayerController.BubbleShootController.OnShootEnded += OnEndShoot;
        }

        public void PlaySound(string id)
        {
            var soundIndex = _sounds.SoundData.FindIndex(x => x.id == id);
            if (soundIndex == -1)
                return;

            var soundData = _sounds.SoundData[soundIndex];
            audioSource.PlayOneShot(soundData.sound);
        }

        private void OnMerge(MergeInfo mergeInfo)
        {
            PlaySound(SOUND_MERGE);
        }

        private void OnStartShoot()
        {
            PlaySound(SOUND_START_SHOOT);
        }

        private void OnEndShoot()
        {
            PlaySound(SOUND_END_SHOOT);
        }
    }
}