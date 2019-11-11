using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefaultSounds;
public class DestinationSpring : MonoBehaviour
{
    public float speed;
    public Transform destination;
    public float stopDistance;
    public float scoreAdd;
    public AudioSource audioSource;

    public bool setCameraDirection;
    public bool camDirNeg;

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
            StartCoroutine(MoveTo(other.transform));
        }
        
    }
    // Update is called once per frame
    IEnumerator MoveTo(Transform other)
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

        Vector3 dir = (destination.position - other.position).normalized;

        if (setCameraDirection == true)
        {
            if (camDirNeg)
            {
                pc.orbitCam.transform.rotation = Quaternion.LookRotation(-dir);
            }
            else
            {
                pc.orbitCam.transform.rotation = Quaternion.LookRotation(-dir);

            }
        }

        while (Vector3.Distance(other.position, destination.position) > stopDistance)
        {
            //rb.MovePosition(Vector3.MoveTowards(other.position, destination.position, speed * 60f * Time.deltaTime));
            pc.rb.velocity = (destination.position - other.position).normalized * speed;
            pc.ball = false;
            yield return new WaitForFixedUpdate();
        }
        //set final velocity here
        yield return new WaitForFixedUpdate();
    }
}
