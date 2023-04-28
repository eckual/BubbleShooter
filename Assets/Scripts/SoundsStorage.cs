using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    [Serializable]
    public struct SoundData
    {
        public string id;
        public AudioClip sound;
    }
    [CreateAssetMenu(menuName ="ScriptableObjects/Sounds/SoundsStorage")]
    public class SoundsStorage : ScriptableObject
    {
        [SerializeField]
        private List<SoundData> soundData = new List<SoundData>();
        public List<SoundData> SoundData
        {
            get { return soundData; }
        }
    }
}