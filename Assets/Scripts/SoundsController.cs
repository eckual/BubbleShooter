using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bubbles;
namespace Sound
{
    public class SoundsController : MonoSingleton<SoundsController>
    {
        private const string SOUND_MERGE = "Merge";
        private const string SOUND_START_SHOOT = "StartShoot";
        private const string SOUND_END_SHOOT = "EndShoot";

        [SerializeField]
        private AudioSource audioSource;

        private SoundsStorage sounds;
        private SessionController sessionController;

        public override void Init()
        {
            sounds = ResourceManager.GetResource<SoundsStorage>(GameConstants.SOUNDS_STORAGE);
            sessionController = SessionController.Instance;
            sessionController.BubblesController.OnMerge += OnMerge;
            sessionController.PlayerController.BubbleShootController.OnShootStarted += OnStartShoot;
            sessionController.PlayerController.BubbleShootController.OnShootEnded += OnEndShoot;
        }

        public void PlaySound(string id)
        {
            var soundIndex = sounds.SoundData.FindIndex(x => x.id == id);
            if (soundIndex == -1)
                return;

            var soundData = sounds.SoundData[soundIndex];
            audioSource.PlayOneShot(soundData.sound);
        }

        private void OnMerge(MergeInfo mergInfo)
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