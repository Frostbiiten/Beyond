using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using LayerHelper;

public class LightSpeedDash : MonoBehaviour
{
    public PlayerCore playerCore;
    public Collider[] dashColliders = new Collider[32];
    public List<Collider> sortedDashColliders;
    public float detectionRadius;
    public Collider currentRing;
    public float dashSpeed;
    public bool dashing;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(Dash());
        }

        playerCore.playerAnimationManager.playerAnimator.SetBool("LightSpeedDash", dashing);
    }

    public IEnumerator Dash()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, dashColliders, (int)PlayerLayerHelper.Layers.LightSpeedDash);
        
        while(count > 0)
        {
            yield return new WaitForFixedUpdate();
            count = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, dashColliders, (int)PlayerLayerHelper.Layers.LightSpeedDash);

            sortedDashColliders = dashColliders.ToList();

            for (var i = sortedDashColliders.Count - 1; i > -1; i--)
            {
                if (sortedDashColliders[i] == null)
                    sortedDashColliders.RemoveAt(i);
            }

            sortedDashColliders.Sort(delegate (Collider a, Collider b)
            {
                return Vector3.Distance(transform.position, a.transform.position)
            .CompareTo(
                  Vector3.Distance(transform.position, b.transform.position));
            });

            currentRing = sortedDashColliders[0];

        }
    }

    public void FixedUpdate()
    {
        if (currentRing)
        {
            dashing = true;
            playerCore.rb.velocity = (currentRing.transform.position - transform.position).normalized * dashSpeed;
        }
        else
        {
            dashing = false;
        }
    }

}
