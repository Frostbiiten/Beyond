using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefaultSounds;

public class Spring : MonoBehaviour
{
    public Vector3 force;
    public bool setCameraDirection = false;
	public bool camDirNeg = true;
    public AudioSource audioSource;
    public float scoreAdd;
    public float camdistance = 10f;

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
            PlayerCore pc = other.GetComponent<PlayerCore>();
            pc.score += scoreAdd;
            pc.UIManager.UpdateScore();
            switch (objType)
            {
                case Type.spring:
                    audioSource.PlayOneShot(MainDefSounds.defaultSounds.spring);
                    break;
                case Type.dashPad:
                    audioSource.PlayOneShot(MainDefSounds.defaultSounds.dashPad);
                    pc.groundedPhysics.currentForce = force.magnitude;
                    break;
                case Type.normalRamp:
                    audioSource.PlayOneShot(MainDefSounds.defaultSounds.normalRamp);
                    break;
                case Type.trickRamp:
                    audioSource.PlayOneShot(MainDefSounds.defaultSounds.trickRamp);
                    pc.playerSoundCore.PlayTrick();
                    break;
                case Type.dashRing:
                    audioSource.PlayOneShot(MainDefSounds.defaultSounds.dashRing);
                    break;
                case Type.trickDashRing:
                    audioSource.PlayOneShot(MainDefSounds.defaultSounds.trickDashRing);
                    pc.playerSoundCore.PlayTrick();
                    break;
            }


            pc.rb.velocity = transform.TransformDirection(force);

            if(setCameraDirection == true){
			if(camDirNeg){
                Vector3 dir = -force;
                pc.orbitCam.transform.rotation = Quaternion.LookRotation(-dir);
			}
            else
            {
                Vector3 dir = force;
                pc.orbitCam.transform.rotation = Quaternion.LookRotation(-dir);
                
            }
		}
                
            
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
