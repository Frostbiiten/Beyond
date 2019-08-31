using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Experimental.Rendering.HDPipeline;
using UnityEngine.Rendering;


public class PlayerSoundManager : MonoBehaviour
{
    public PlayerCore playerCore;
    public AudioSource footStepSource;
    public AudioClip[] trickSounds;
    public AudioClip[] homingSounds;
    public AudioSource sfxSoundSource;
    public AudioSource voiceSource;
    public AudioClip initialJumpSound;
    public AudioClip loseRings;
    public AudioClip[] hurtVoice;
    public AudioClip dieVoice;
    public AudioClip jumpBallSound;
    public AudioSource jumpBallSource; //ONLY ONE SOUND
    public bool inWater;
    public AudioMixer mixer;
    public float waterCutoff = 620f;
    public Volume waterFx;

    public void FixedUpdate()
    {
        inWater = false;
    }


    public void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 4 && other.isTrigger == true)
        {
            inWater = true;
        }
    }

    public void Update()
    {
        footStepSource.volume = Mathf.Lerp(footStepSource.volume, Mathf.Clamp(1f - playerCore.velocityMagnitude / 16f, 0f, 1f), 0.2f);
        if(inWater == true)
        {
            waterFx.weight = Mathf.Lerp(waterFx.weight, 1f, 0.1f);
            mixer.SetFloat("Cutoff Frequency", waterCutoff);
        }
        else
        {
            waterFx.weight = Mathf.Lerp(waterFx.weight, 0f, 0.1f);
            if(waterFx.weight < 0.05f)
            {
                waterFx.weight = 0f;
            }
            mixer.SetFloat("Cutoff Frequency", 222000f);
        }
    }

    public void AltJump()
    {
        jumpBallSource.PlayOneShot(jumpBallSound, 0.5f);
    }


    public void PlayTrick()
    {
        voiceSource.PlayOneShot(trickSounds[Random.Range(0, trickSounds.Length - 1)]);
    }

    public void PlayHome()
    {
        voiceSource.PlayOneShot(homingSounds[Random.Range(0, homingSounds.Length - 1)]);
    }

    public void PlayHurt()
    {
        voiceSource.PlayOneShot(hurtVoice[Random.Range(0, hurtVoice.Length - 1)]);
        sfxSoundSource.PlayOneShot(loseRings);
    }

    public void PlayDie()
    {
        voiceSource.PlayOneShot(dieVoice);
    }
}
