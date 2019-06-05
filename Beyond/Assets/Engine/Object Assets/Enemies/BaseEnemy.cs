using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour {
    public float hp;
    public GameObject debris;


    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            PlayerCore pc = other.GetComponent<PlayerCore>();
            if(pc.playerHpManager.attacking == true || pc.playerHpManager.invincible == true)
            {
                hp = hp - 1f;
            }

            if (hp <= 0f)
            {
                Instantiate(debris, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }

    }

}
