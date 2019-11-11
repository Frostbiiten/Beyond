using System.Collections;
using UnityEngine;
public class Ring : MonoBehaviour {
    public Collider col;
    public float force;
    public float downForce;
    public Rigidbody rb;
    public float rotSpeed;
	void Start () {
        StartMain();
	}

    public void StartMain()
    {
        col.enabled = false;
        StartCoroutine(EnableCollider());
        rb.AddForce(rb.velocity * force, ForceMode.Impulse);
    }

    void Update()
    {
        transform.Rotate(0f, rotSpeed * Time.deltaTime, 0f);
        rb.AddForce(Vector3.down * downForce * Time.deltaTime);
    }

    IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(0.1f);
        col.enabled = true;
    }
}
