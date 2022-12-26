using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

namespace Match3NonPhys
{
    [System.Serializable]
    public class Sound
    {
        public string _name;
        public AudioClip _clip;
        [Range(0f,1f)]
        public float _volume;
        [Range(0.1f, 3f)]
        public float _pitch;
        public bool _loop;

        public AudioSource _source { get; private set; }

        public void SetAudioSource(AudioSource source)
        {
            _source = source;
        }
    }
}
