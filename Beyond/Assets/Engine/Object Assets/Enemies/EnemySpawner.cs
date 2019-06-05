using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    public Object spawnObject;
    public float spawnTime;
    public Renderer rend;
    public Vector3 rotation;
    public ParticleSystem fx;
	void Start () {
        InvokeRepeating("SlowUpdate", 0f, 0.2f);
	}
	
	void SlowUpdate () {
        if (transform.childCount < 1)
        {
            StartCoroutine(Spawn());
        }
        if(transform.childCount >= 1)
        {
            StopAllCoroutines();
        }
	}

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(spawnTime);
        fx.Play();
        Instantiate(spawnObject, transform.position, Quaternion.Euler(rotation), transform);
    }

    [ExecuteInEditMode]
    void Update()
    {
        if (Application.isPlaying)
        {
            rend.enabled = false;
        }
        else
        {
            rend.enabled = true;
        }
    }
}
