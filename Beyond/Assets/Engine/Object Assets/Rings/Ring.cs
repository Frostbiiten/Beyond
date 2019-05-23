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

    void FixedUpdate()
    {
        transform.Rotate(0f, rotSpeed, 0f);
        rb.AddForce(Vector3.down * downForce);
    }

    IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(0.1f);
        col.enabled = true;
    }
}
