using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Utils;

namespace UI
{
    public class AudioLoader<T>
    {
        private AsyncOperationHandle<SoundsStorage> _loadHandle;

        public void LoadSounds(string addressableKey, Action<List<T>> onComplete)
        {
            _loadHandle = Addressables.LoadAssetAsync<SoundsStorage>(addressableKey);
            _loadHandle.Completed += handle =>
            {
                if (handle.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError($"Failed to load sounds addressable key = '{addressableKey}'");
                    return;
                }

                var sounds = handle.Result.SoundData;
                //onComplete?.Invoke(sounds);
            };
        }

        public void UnloadSounds()
        {
            if (!_loadHandle.IsValid() || _loadHandle.IsDone) return;
            
            Addressables.Release(_loadHandle);
        }
    }
}