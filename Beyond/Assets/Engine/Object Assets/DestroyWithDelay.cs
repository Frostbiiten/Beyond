using System.Collections;
using UnityEngine;

public class DestroyWithDelay : MonoBehaviour {
    public float delay;
	void Start () {
        StartCoroutine(Destroy());
	}

	public IEnumerator Destroy()
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
