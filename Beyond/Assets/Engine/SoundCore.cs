using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefaultSounds;

namespace DefaultSounds
{
    [System.Serializable]
    public struct DefaultSoundsStruct
    {
        public AudioClip spring;
        public AudioClip dashPad;
        public AudioClip normalRamp;
        public AudioClip trickRamp;
        public AudioClip dashRing;
        public AudioClip trickDashRing;
        public AudioClip ring;
        public AudioClip hit;
    }

    static class MainDefSounds
    {
        public static DefaultSoundsStruct defaultSounds;
    }
}

public class SoundCore : MonoBehaviour
{
    public static AudioSource nonSpacialSource;


    public DefaultSoundsStruct DefaultSounds;

    void Awake()
    {
        nonSpacialSource = GetComponent<AudioSource>();
        MainDefSounds.defaultSounds = DefaultSounds;
    }

    public void Play1D(AudioClip clip)
    {
        nonSpacialSource.PlayOneShot(clip);
    }
}
