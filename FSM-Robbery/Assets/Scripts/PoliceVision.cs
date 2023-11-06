using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PoliceVision : MonoBehaviour
{
    public Camera frustum;
    public NavMeshAgent agent;
    private FSMCop police;

    internal bool iSeeThief = false;
    public float yourMaxRaycastDistance = 30;
    internal bool isThief = false;
    internal Vector3 thiefWas; 
    
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        police = GetComponent<FSMCop>();
    }

    private void Update()
    {
        Vision();
    }

    public void Vision()
    {
        iSeeThief = false;

        Vector3 raycastOrigin = frustum.transform.position;
        float maxDistance = yourMaxRaycastDistance;
        float fovRad = frustum.fieldOfView * Mathf.Deg2Rad;

        for (float angle = -fovRad / 2; angle <= fovRad / 2; angle += 0.1f)
        {
            Vector3 raycastDirection = frustum.transform.forward;
            raycastDirection = Quaternion.Euler(0, angle * Mathf.Rad2Deg, 0) * raycastDirection;

            RaycastHit hit;
            if (Physics.Raycast(raycastOrigin, raycastDirection, out hit, maxDistance))
            {
                Debug.DrawRay(raycastOrigin, raycastDirection * maxDistance, Color.red); 

                if (hit.collider.CompareTag("Target"))
                {
                    Debug.DrawRay(raycastOrigin, raycastDirection * maxDistance, Color.green);

                    if (police.Stolen)
                    {
                        Debug.Log("I see the Thief!");
                    }
                    else
                    {
                        Debug.Log("I see someone suspicious");
                    }
                }
                else iSeeThief = false;
            }
        }
    }
}
