using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationSpring : MonoBehaviour
{
    public float speed;
    public Transform destination;
    public float stopDistance;

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
        while(Vector3.Distance(other.position, destination.position) > stopDistance)
        {
            //rb.MovePosition(Vector3.MoveTowards(other.position, destination.position, speed * 60f * Time.deltaTime));
            pc.rb.velocity = (destination.position - other.position).normalized * speed;
            pc.ball = false;
            yield return null;
        }
        //set final velocity here
        yield return null;
    }
}
