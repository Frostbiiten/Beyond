using LayerHelper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

public class PlayerHpManager : MonoBehaviour
{
    [Required("PlayerCore is required")]
    public PlayerCore playerCore;

    #region Rings

    [System.Serializable]
    public struct RingShinePoolObj
    {
        public GameObject shine;
        public ParticleSystem pSystem;
        public bool free;
        public RingShinePoolObj(GameObject inShine, ParticleSystem inPSystem, bool inFree)
        {
            shine = inShine;
            pSystem = inPSystem;
            free = inFree;
        }
    }

    [System.Serializable]
    public struct RingPoolObj
    {
        public GameObject ring;
        public Rigidbody ringRb;
        public float timeleft;
        public Ring Ring;
        public RingPoolObj(GameObject inRing, Rigidbody inRingRb, float inTimeLeft, Ring inRingScr)
        {
            ring = inRing;
            ringRb = inRingRb;
            timeleft = inTimeLeft;
            Ring = inRingScr;
        }
    }

    [Header("Rings")]

    [BoxGroup("Rings")]
    public GameObject ringShinePreset;

    [BoxGroup("Rings")]
    public Transform ringShineParent;

    [BoxGroup("Rings")]
    public Transform ringObjParent;

    [BoxGroup("Rings")]
    public float ringNormalTimeLeft;

    [BoxGroup("Rings")]
    [ReorderableList]
    public List<RingShinePoolObj> ringShines = new List<RingShinePoolObj>();

    [BoxGroup("Rings")]
    [ReorderableList]
    public List<RingPoolObj> rings = new List<RingPoolObj>();

    [BoxGroup("Rings")]
    public int maxRingsToLose;

    #endregion

    [Header("Health Variables")]
    #region Variables

    [BoxGroup("Main")]
    [Tooltip("The player's Health (or rings)")]
    public float hp;

    [BoxGroup("Main")]
    [Tooltip("The current hurt bounce back force")]
    public Vector3 bounceBack;

    [BoxGroup("Main")]
    [Tooltip("The hurt bounce back Y value")]
    public float bounceBackY = 1f;

    [BoxGroup("Main")]
    [Tooltip("How far back does the enemy push you")]
    public float bounceBackPower;

    [BoxGroup("Main")]
    [Tooltip("Can the player collect rings at the moment?")]
    public bool canCollectRings;

    [BoxGroup("Main")]
    [Tooltip("The prefab for dropped rings")]
    public GameObject ringObject;

    [BoxGroup("Main")]
    [Tooltip("How long is the player invincible after hit")]
    public float recoveryTime;

    [BoxGroup("Main")]
    [Tooltip("How many lives the player has")]
    public float lives;

    #endregion

    #region Conditions
    [Header("Main Conditions")]


    [BoxGroup("Conditions")]
    [Tooltip("Is the player in an attack state")]
    public bool attacking;

    [BoxGroup("Conditions")]
    [Tooltip("Is the player (not) allowed to be damaged")]
    public bool invincible;

    [BoxGroup("Conditions")]
    [Tooltip("Is the player recovering")]
    public bool recovering;
    #endregion



    public enum Shield
    {
        none = 0,
        normal = 1,
        electric = 2,
        fire = 3,
        water = 4,
    }

    [BoxGroup("Shield")]
    public Shield shield;

    [BoxGroup("Other")]
    public Vector3 theVoid;

    [BoxGroup("Other")]
    public float ringMagnetivityRange;

    [BoxGroup("Other")]
    public float electricRingMagnetivity;

    [BoxGroup("Other")]
    public int ringMagnetivityLayerMask;

    [BoxGroup("Other")]
    public Collider[] magnetRings;

    [BoxGroup("Other")]
    public float ringMagnetivityStrength;

    [BoxGroup("Other")]
    public float ringMagnetivitySpeed;


    [BoxGroup("Other")]
    public CheckPoint checkpoint;

    [BoxGroup("Other")]
    public bool dying = false;

    [BoxGroup("Shield")]
    public ParticleSystem electric;

    [BoxGroup("Shield")]
    public ParticleSystem fire;

    [BoxGroup("Shield")]
    public GameObject normalShield;

    [BoxGroup("Shield")]
    public GameObject elecShield;

    [BoxGroup("Shield")]
    public GameObject fireShield;

    [BoxGroup("Shield")]
    public GameObject waterShield;

    [BoxGroup("Other")]
    public string gameOverScene;


    void Start()
    {
        //TO FIX RING LOSS GLITCH?
        for(int q = 0; q < maxRingsToLose; q++)
        {
            SpawnRing(theVoid, Vector3.zero);
        }

        UpdateLives();

        ringMagnetivityLayerMask = (int)PlayerLayerHelper.Layers.StaticRing;
    }

    void FixedUpdate()
    {
        #region Ring Magnet
        if (playerCore.velocityMagnitude >= ringMagnetivitySpeed)
        {
            if(shield == Shield.electric)
            {
                Physics.OverlapSphereNonAlloc(transform.position, electricRingMagnetivity, magnetRings, ringMagnetivityLayerMask, QueryTriggerInteraction.Collide);

            }
            else
            {
                Physics.OverlapSphereNonAlloc(transform.position, ringMagnetivityRange, magnetRings, ringMagnetivityLayerMask, QueryTriggerInteraction.Collide);
            }
            
            if(magnetRings.Length > 0)
            {
                for(int z = 0; z < magnetRings.Length; z++)
                {
                    if (magnetRings[z])
                    {
                        //magnetRings[z].transform.position = Vector3.Lerp(magnetRings[z].transform.position, transform.position, 0.05f * ringMagnetivityStrength);
                        magnetRings[z].transform.position = Vector3.MoveTowards(magnetRings[z].transform.position, transform.position, ringMagnetivityStrength * 0.33f);

                    }

                }
            }
        }
        #endregion

        #region Basic Conditions
        if (playerCore.ball == true || playerCore.playerStompSlide.sliding == true || playerCore.playerStompSlide.stomping == true || playerCore.playerAnimationManager.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Slide Kick") || playerCore.playerAnimationManager.driftBall.gameObject.activeSelf == true) // Add || for more conditions
        {
            attacking = true;
        }
        else
        {
            attacking = false;
        }

        if(playerCore.isSuperSonic == true)
        {
            invincible = true;
        }
        else
        {
            invincible = false;
        }
        #endregion
    }

    void Update()
    {
        #region Shields
        RemoveShields();
        if(shield == Shield.normal)
        {
            normalShield.SetActive(true);
        }

        if (shield == Shield.electric)
        {
            elecShield.SetActive(true);
            electric.Play();
        }
        else
        {
            electric.Stop();
        }

        if (shield == Shield.fire)
        {
            fireShield.SetActive(true);
            fire.Play();
        }
        else
        {
            fire.Stop();
        }

        if (shield == Shield.water)
        {
            waterShield.SetActive(true);
        }


        #endregion

        for (int e = 0; e < rings.Count; e++)
        {
            rings[e] = new RingPoolObj(rings[e].ring, rings[e].ringRb, rings[e].timeleft - Time.deltaTime, rings[e].Ring);

            if (rings[e].timeleft <= 0f)
            {
                rings[e].ring.transform.position = theVoid;
            }
        }

        if(playerCore.playerAnimationManager.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("DamageGround") || playerCore.playerAnimationManager.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Getup"))
        {
            playerCore.inputCore.InputLockForFrame();
        }
    }

    void RemoveShields()
    {
        normalShield.SetActive(false);
        elecShield.SetActive(false);
        fireShield.SetActive(false);
        waterShield.SetActive(false);

    }
    void OnCollisionEnter(Collision collision)
    {
        CollisionEvent(collision);
    }

    #region POOL
    void SpawnRingShine(Vector3 position)
    {
        for (int w = 0; w < ringShines.Count; w++)
        {
            ringShines[w] = new RingShinePoolObj(ringShines[w].shine, ringShines[w].pSystem, !ringShines[w].pSystem.isPlaying);
            if (ringShines[w].free == true)
            {
                //Debug.Log("Recycled an object!");
                ringShines[w].shine.transform.position = position;
                ringShines[w].pSystem.Play();
                return;
            }
        }

        //Debug.Log("Spawned a ring shine into the pool");
        GameObject g = Instantiate(ringShinePreset, position, Quaternion.identity, ringShineParent);
        ringShines.Add(new RingShinePoolObj(g, g.GetComponent<ParticleSystem>(), false));
        ringShines[0].pSystem.Play();
    }

    void SpawnRing(Vector3 position, Vector3 velocity)
    {

        for (int w = 0; w < rings.Count; w++)
        {
            if (rings[w].timeleft <= 0f)
            {
                //Debug.Log("Recycled an object!");
                rings[w].ring.transform.position = position;
                rings[w].ringRb.velocity = velocity;
                rings[w].Ring.StartMain();
                rings[w] = new RingPoolObj(rings[w].ring, rings[w].ringRb, ringNormalTimeLeft, rings[w].Ring);
                return;
            }
        }

        //Debug.Log("Spawned a ring obj into the pool");
        GameObject r = Instantiate(ringObject, position, Quaternion.identity, ringObjParent);
        rings.Add(new RingPoolObj(r, r.GetComponent<Rigidbody>(), ringNormalTimeLeft, r.GetComponent<Ring>()));
        rings[0].ring.transform.position = position;
        rings[0].ringRb.velocity = velocity;
        rings[0].Ring.StartMain();


    }
    #endregion

    public void CollisionEvent(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Hazard"))
        {
            if (invincible == false && attacking == false && recovering == false)
            {
                bounceBack = -playerCore.playerAnimationManager.playerSkin.forward;
                bounceBack.y = bounceBackY;
                playerCore.rb.velocity = bounceBack * bounceBackPower;
                if (hp > 0f)
                {
                    StartCoroutine(playerCore.inputCore.InputLock(0.5f));
                    if (shield == Shield.none)
                    {
                        StartCoroutine(StopRingCollection());
                        StartCoroutine(LoseRings(hp));
                        StartCoroutine(Recovery());
                        playerCore.playerAnimationManager.playerAnimator.Play("Damage");
                        playerCore.playerSoundCore.PlayHurt();
                        hp = 0f;
                    }
                    else
                    {
                        shield = 0;
                        StartCoroutine(Recovery());
                        playerCore.playerAnimationManager.playerAnimator.Play("Damage");
                    }

                    //ScriptCore.SoundCore.mainPlayerAudio.PlayOneShot(ScriptCore.SoundCore.hurt[Random.Range(0, ScriptCore.SoundCore.hurt.Count - 1)]);
                }
                else
                {
                    UpdateLives();
                    
                    if(dying == false)
                    {
                        StartCoroutine(Die());
                    }
                    playerCore.playerAnimationManager.playerAnimator.Play("Die");
                    //StartCoroutine(Die);
                }
            }
            UpdateRings();
        }
    }

    public void ExternalHurt(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Hazard"))
        {
            if (invincible == false && attacking == false && recovering == false)
            {
                bounceBack = -playerCore.playerAnimationManager.playerSkin.forward;
                bounceBack.y = bounceBackY;
                playerCore.rb.velocity = bounceBack * bounceBackPower;
                if (hp > 0f)
                {
                    if (shield == Shield.none)
                    {
                        StartCoroutine(StopRingCollection());
                        StartCoroutine(LoseRings(hp));
                        StartCoroutine(Recovery());
                        playerCore.playerAnimationManager.playerAnimator.Play("Damage");
                        playerCore.playerSoundCore.PlayHurt();
                        hp = 0f;
                    }
                    else
                    {
                        shield = 0;
                        StartCoroutine(Recovery());
                        playerCore.playerAnimationManager.playerAnimator.Play("Damage");
                    }

                    //ScriptCore.SoundCore.mainPlayerAudio.PlayOneShot(ScriptCore.SoundCore.hurt[Random.Range(0, ScriptCore.SoundCore.hurt.Count - 1)]);
                }
                else
                {
                    UpdateLives();
                    lives--;
                    if (dying == false)
                    {
                        StartCoroutine(Die());
                    }
                    playerCore.playerAnimationManager.playerAnimator.Play("Die");
                    //StartCoroutine(Die);
                }
            }
            UpdateRings();
        }
    }

    public IEnumerator Die()
    {
        lives--;
        UpdateLives();
        dying = true;
        playerCore.UIManager.sonicEye.sprite = playerCore.UIManager.deadEye;
        playerCore.fadeAnimator.Play("FadeOut");
        playerCore.playerSoundCore.PlayDie();
        StartCoroutine(playerCore.inputCore.InputLock(1.9f));
        yield return new WaitForSeconds(2f);

        playerCore.UIManager.sonicEye.sprite = playerCore.UIManager.defaultEye;

        if (lives < 0)
        {
            SceneManager.LoadScene(gameOverScene);
        }
        else
        {
            transform.position = checkpoint.transform.position + checkpoint.offset;
        }

        dying = false;
    }

    void OnTriggerEnter(Collider other)
    {


        if (other.CompareTag("ShieldBox"))
        {
            // whomst
        }

        if (canCollectRings == true)
        {
            if (other.CompareTag("Ring"))
            {
                //shouldn't destroy, instead set free in pool
                //Destroy(other.gameObject)
                int t = 0;
                for(int r = 0; r < rings.Count; r++)
                {
                    if(rings[r].ring == other.gameObject)
                    {
                        t++;
                        rings[r] = new RingPoolObj(rings[r].ring, rings[r].ringRb, 0f, rings[r].Ring);
                    }
                }

                if(t == 0)
                {
                    Destroy(other.gameObject);
                }
                
                hp += 1;
                UpdateRings();
                SpawnRingShine(other.transform.position);
                SoundCore.nonSpacialSource.PlayOneShot(DefaultSounds.MainDefSounds.defaultSounds.ring, 0.075f);
            }

            if (other.CompareTag("Static Ring"))
            {
                Destroy(other.gameObject);

                hp += 1;
                SpawnRingShine(other.transform.position);
                UpdateRings();
                SoundCore.nonSpacialSource.PlayOneShot(DefaultSounds.MainDefSounds.defaultSounds.ring, 0.075f);
            }
        }

    }

    public void UpdateRings()
    {
        playerCore.UIManager.UpdateRings();
    }
    public void UpdateLives()
    {
        playerCore.UIManager.UpdateLives();
    }

    #region IEnumerators

    public IEnumerator StopRingCollection()
    {
        canCollectRings = false;
        yield return new WaitForSeconds(0.5f);
        canCollectRings = true;
    }

    public IEnumerator LoseRings(float amount)
    {
        float d = 360f / Mathf.Clamp(amount, 0f, maxRingsToLose);
        for (int r = 0; r < Mathf.Clamp(amount, 0f, maxRingsToLose); r++)
        {
            Vector3 currentRingVector = Quaternion.AngleAxis(d * r, Vector3.up) * Vector3.forward;
            //Instantiate(ringObject, transform.position + currentRingVector, Quaternion.identity).GetComponent<Rigidbody>().velocity = new Vector3(currentRingVector.x, 1f, currentRingVector.z);
            SpawnRing(transform.position + currentRingVector, new Vector3(currentRingVector.x, 1f, currentRingVector.z));
        }
        yield return null;
    }

    public IEnumerator Recovery()
    {
        recovering = true;
        yield return new WaitForSeconds(recoveryTime);
        recovering = false;
    }

    #endregion
}
