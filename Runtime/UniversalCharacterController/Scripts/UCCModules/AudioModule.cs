using System;
using UnityEngine;

namespace UniversalCharacterController.Scripts.Modules
{
    [Serializable]
    public class AudioModuleInspector
    {
        [Tooltip("Audio source used for footstep sounds")]
        public AudioSource footstepsSource;

        [Tooltip("Audio source used for landing sounds")]
        public AudioSource landingSource;

        [Tooltip("Audio source used for foley / ambient movement sounds")]
        public AudioSource foleySource;

        [Tooltip("Footstep audio clips")]
        public AudioClip[] footstepClips;

        [Tooltip("Audio clip played on landing")]
        public AudioClip landingClip;

        [Range(0f, 1f)]
        [Tooltip("Volume multiplier for footstep sounds")]
        public float footstepVolume = 0.5f;
    }

    public class AudioModule
    {
        private AudioModuleInspector _data;

        public void Initialize(AudioModuleInspector data)
        {
            _data = data;
        }

        public void PlayFootstep(float animationWeight)
        {
            if (animationWeight <= 0.5f) return;
            if (_data.footstepsSource != null) _data.footstepsSource.Play();
            if (_data.foleySource != null) _data.foleySource.Play();
        }

        public void PlayLanding(float animationWeight)
        {
            if (animationWeight <= 0.5f) return;
            if (_data.landingSource != null) _data.landingSource.Play();
        }
    }
}

