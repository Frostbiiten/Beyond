using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefaultSounds;

public class Spring : MonoBehaviour
{
    public Vector3 force;
    public bool setCameraDirection = false;
    public AudioSource audioSource;

    public enum Type
    {
        spring = 0,
        dashPad = 1,
        normalRamp = 2,
        trickRamp = 3,
        dashRing = 4,
        trickDashRing = 5,
    }

    public Type objType;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (objType)
            {
                case Type.spring:
                    audioSource.PlayOneShot(MainDefSounds.defaultSounds.spring);
                    break;
                case Type.dashPad:
                    audioSource.PlayOneShot(MainDefSounds.defaultSounds.dashPad);
                    break;
                case Type.normalRamp:
                    audioSource.PlayOneShot(MainDefSounds.defaultSounds.normalRamp);
                    break;
                case Type.trickRamp:
                    audioSource.PlayOneShot(MainDefSounds.defaultSounds.trickRamp);
                    break;
                case Type.dashRing:
                    audioSource.PlayOneShot(MainDefSounds.defaultSounds.dashRing);
                    break;
                case Type.trickDashRing:
                    audioSource.PlayOneShot(MainDefSounds.defaultSounds.trickDashRing);
                    break;
            }
            PlayerCore pc = other.GetComponent<PlayerCore>();

            pc.rb.velocity = transform.TransformDirection(force);

            if(setCameraDirection == true)
                pc.orbitCam.x = Quaternion.LookRotation(transform.InverseTransformVector(force).normalized).eulerAngles.y;
            
            pc.ball = false;
	    StartCoroutine(Set(pc));
        }
    }

    IEnumerator Set(PlayerCore pc){
        yield return null;
	yield return null;
	pc.ball = false;
    }
}
