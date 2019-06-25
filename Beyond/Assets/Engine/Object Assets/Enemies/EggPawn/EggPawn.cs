using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LayerHelper;

public class EggPawn : MonoBehaviour
{
    public Rigidbody rb;
    public float searchDist;
    public Collider[] playerScan = new Collider[1];
    public Animator animator;
    public Transform skin;
    public bool chasing;
    public float chaseSpeed;
    public float stopDistance;
    public float hitTrigger01 = 0.05f;
    public Collider thisCol;
    int mask;
    public float hp;
    bool strike;
    public float dieForce;
    public GameObject debris;
    public CameraTrigger trigger;
    public Transform cameraTrigger;
    public float scoreAdd;

    // Start is called before the first frame update
    void Start()
    {
        mask = (int)PlayerLayerHelper.Layers.Player;
        InvokeRepeating("Search", 0f, 0.5f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(strike == false)
        {
            chasing = false;
            if (playerScan[0])
            {
                chasing = true;
                Chase(playerScan[0].transform);
            }

            animator.SetBool("Chasing", chasing);
        }

        if(strike == true)
        {
            if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f)
            {
                Instantiate(debris, transform.position, transform.rotation);
                Destroy(transform.parent.gameObject);
                PlayerCore pc = playerScan[0].GetComponent<PlayerCore>();
                pc.score += scoreAdd;
                pc.UIManager.UpdateScore();
            }
        }
        cameraTrigger.position = transform.position;

    }

    void Search()
    {
        Physics.OverlapSphereNonAlloc(transform.position, searchDist, playerScan, mask);
    }

    //

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Enter");
            PlayerCore pc = other.gameObject.GetComponent<PlayerCore>();
            if (pc)
            {
		        if(pc.playerHpManager.attacking == true || pc.playerHpManager.invincible == true){
                	pc.playerHpManager.ExternalHurt(other);

                	if(hp > 0)
                	{
                        hp -= 1;
                	}
                	else
                	{
                        Hit(other);
                	}
		        }

            }

        }
    }

    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            EggPawn e = other.gameObject.GetComponent<EggPawn>();
            if (e)
            {
                if (e.strike == true)
                {
                    if(strike == false)
                    Hit(other.collider);

                }

                if (strike == true)
                {
                    if (e.strike == false)
                    e.Hit(thisCol);
                }
            }

        }
    }

    void Chase(Transform target)
    {
        PlayerCore pc = playerScan[0].GetComponent<PlayerCore>();
        if (Vector3.Distance(transform.position, target.position) > stopDistance)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Charge") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Happy") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Hit")){
                Vector3 dir = (target.position - transform.position).normalized;
                dir = new Vector3(dir.x, 0f, dir.z);
                Vector3 movementDir = rb.velocity;
                movementDir = new Vector3(movementDir.x, 0f, movementDir.z);
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Chase"))
                {
                    rb.velocity = dir * chaseSpeed;
                }

                skin.LookAt(skin.position + movementDir);
            }
        }
        else
        {
            if(animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            {
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > hitTrigger01)
                {
                 
                    
                    pc.playerHpManager.ExternalHurt(thisCol);
                }

            }

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Charge") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Happy") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Hit") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Ch1"))
            {
                animator.Play("Charge");
            }


        }

        animator.SetBool("Hit", pc.playerHpManager.recovering);

    }

    public void Hit(Collider cd)
    {
        strike = true;
        SoundCore.nonSpacialSource.PlayOneShot(DefaultSounds.MainDefSounds.defaultSounds.hit, 1f);
        animator.Play("Hit Front");
        Vector3 vel = (cd.transform.position - transform.position).normalized;
        vel = new Vector3(vel.x, 0f, vel.z);
        rb.velocity = vel * dieForce;

        if (trigger)
        {
            if (trigger.currentCam)
            {
                trigger.currentCam.orbitCam.currentLook = null;
            }

        }
    }
}
