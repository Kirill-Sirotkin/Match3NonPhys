using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

namespace Match3NonPhys
{
    public class SoundManager : MonoBehaviour
    {
        [field: SerializeField] private Sound[] _sounds;

        private void Awake()
        {
            foreach(Sound sound in _sounds)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                sound.SetAudioSource(source);

                sound._source.clip = sound._clip;
                sound._source.volume = sound._volume;
                sound._source.pitch = sound._pitch;
                sound._source.loop = sound._loop;
            }
        }

        public void PlaySound(string name)
        {
            Sound sound = Array.Find(_sounds, sound => sound._name == name);
            if (sound == null) { Debug.Log("No sound with that name"); return; }

            sound._source.Play();
        }
    }
}
