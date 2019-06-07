using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    public PlayerCore playerCore;
    public AudioClip[] trickSounds;
    public AudioClip[] homingSounds;
    public AudioSource sfxSoundSource;
    public AudioSource voiceSource;
    public AudioClip initialJumpSound;

    void Update()
    {


    }

    public void PlayTrick()
    {
        voiceSource.PlayOneShot(trickSounds[Random.Range(0, trickSounds.Length - 1)]);
    }

    public void PlayHome()
    {
        voiceSource.PlayOneShot(homingSounds[Random.Range(0, homingSounds.Length - 1)]);
    }
}
