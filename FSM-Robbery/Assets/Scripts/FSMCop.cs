using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FSMCop : MonoBehaviour
{
    public Transform treasure;
    private Moves moves;
    private NavMeshAgent police;
    private WaitForSeconds wait = new WaitForSeconds(0.05f); // == 1/20
    private PoliceVision vision;
    private Animator animator;

    internal delegate IEnumerator CopState();
    internal CopState Copstate;

    private float approachEvery = 10.0f; //10 secs
    private float distToCheck = 5.0f;
    internal bool Stolen = false;
    private bool checkTreasure = false;
    private float lastApproachTime; // Last execution of "Approach"

    IEnumerator Start()
    {
        moves = gameObject.GetComponent<Moves>();
        police = gameObject.GetComponent<NavMeshAgent>();
        vision = GetComponent<PoliceVision>();
        animator = GetComponent<Animator>();

        lastApproachTime = Time.time; 

        yield return wait;

        Copstate = CopWander;
        Debug.Log("Cop: Wander state");

        while (enabled)
            yield return StartCoroutine(Copstate());
    }

    IEnumerator CopWander()
    {
        animator.SetFloat("VelX", police.speed);
        animator.SetFloat("VelY", police.speed);

        moves.Wander();

        if (Time.time - lastApproachTime >= approachEvery)
        {
            checkTreasure = false;
            Copstate = Approaching;
            Debug.Log("Cop: Approaching state");
        }
        yield return wait;
    }

    IEnumerator Approaching()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(treasure.position, out hit, distToCheck, NavMesh.AllAreas))
        {
            moves.Seek(hit.position);
        }

        if (Vector3.Distance(treasure.position, transform.position) <= distToCheck && !checkTreasure)
        {
            Renderer[] childRenderers = treasure.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in childRenderers)
            {
                if(!renderer.enabled)
                {
                    Stolen = true;
                    break;
                }
            }
            if (Stolen)
            {
                police.speed += 2;
                Copstate = Searching;
                Debug.Log("Cop: Treasure Stolen! Looking for the Thief!");
                yield return wait;
            }
            else
            {
                Stolen = false;
                checkTreasure = true;
                lastApproachTime = Time.time;
                Debug.Log("Cop: The treasure is safe, wandering again");
                Copstate = CopWander;
                yield return wait;
            }
        }

        yield return wait;
    }

    IEnumerator Searching()
    {
        moves.Wander();
        yield return wait;
    }

}
