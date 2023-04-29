using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
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
        [SerializeField] private List<SoundData> soundData;
        public List<SoundData> SoundData => soundData;
        
    }
}