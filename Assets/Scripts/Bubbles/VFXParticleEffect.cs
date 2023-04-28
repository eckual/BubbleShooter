using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bubbles
{
    public class VFXParticleEffect : MonoBehaviour, IPoolObject
    {
        [SerializeField]
        private string id;
        [SerializeField]
        private ParticleSystem mainParticles;
        [SerializeField]
        private List<ParticleSystem> childParticles = new List<ParticleSystem>();

        private VFXParticleEffectsPool parentPool;

        [SerializeField]
        private float duration;
        private float time;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public ParticleSystem MainParticles
        {
            get { return mainParticles; }
        }

        public List<ParticleSystem> ChildParticles
        {
            get { return childParticles; }
        }

        public void Init(VFXParticleEffectsPool parentPool)
        {
            this.parentPool = parentPool;
            gameObject.SetActive(true);
            mainParticles.Play(true);
        }

        public void SetColor(Color color)
        {
            var mainModule = mainParticles.main;
            mainModule.startColor = new ParticleSystem.MinMaxGradient(color);

            for(int i = 0; i < childParticles.Count; i++)
            {
                var module = childParticles[i].main;
                module.startColor = new ParticleSystem.MinMaxGradient(color);
            }
        }

        public void Release()
        {
            mainParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            time = 0.0f;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            time += Time.deltaTime;
            if(time >= duration)
            {
                parentPool.Release(this);
            }
        }
    }
}