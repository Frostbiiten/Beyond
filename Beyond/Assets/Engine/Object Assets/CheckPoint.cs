using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CheckPoint : MonoBehaviour
{
    public Vector3 offset;
    public float dist;
    public Transform l;
    public Transform r;
    public Transform m;
    public BoxCollider c;
    public Animator poleAnimator;
    public GameObject laser;
    public GameObject lightL;
    public GameObject lightR;
    public AudioSource soundSource;
    public float scoreAdd = 250f;
    void Update()
    {
        if(l && r && c)
        {
            l.localPosition = new Vector3(dist, 0f, 0f);
            r.localPosition = new Vector3(-dist, 0f, 0f);
            m.localScale = new Vector3(dist * 2f, 1f, 1f);
            m.localPosition = Vector3.zero;
            c.center = m.localPosition + Vector3.up * 0.5f;
            c.size = m.localScale;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCore pc;
            pc = other.gameObject.GetComponent<PlayerCore>();
            Enter(pc);

        }
    }

    public void Enter(PlayerCore pc)
    {
        if(pc.playerHpManager.checkpoint != this)
        {
            soundSource.PlayOneShot(DefaultSounds.MainDefSounds.defaultSounds.checkPoint, 0.3f);
            if (pc.playerHpManager.checkpoint)
            {
                pc.playerHpManager.checkpoint.Out(pc);
            }

            pc.playerHpManager.checkpoint = this;
            poleAnimator.Play("In");
            laser.SetActive(false);      
            lightL.SetActive(true);
            lightR.SetActive(true);
            pc.score += scoreAdd;
            pc.UIManager.UpdateScore();
        }
    }

    public void Out(PlayerCore pc)
    {
        poleAnimator.Play("Out");
        laser.SetActive(true);
        lightL.SetActive(false);
        lightR.SetActive(false);
    }
}
