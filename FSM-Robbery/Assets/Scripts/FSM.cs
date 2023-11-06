using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class FSM : MonoBehaviour
{
    public Transform cop;
    public GameObject treasure;
    public float dist2Steal = 15.0f;
    private float initialSpeed;
    private float running;
    private Animator animator;
    Moves moves;
    UnityEngine.AI.NavMeshAgent agent;

    private WaitForSeconds wait = new WaitForSeconds(0.05f); // == 1/20
    delegate IEnumerator State();
    private State state;

    IEnumerator Start()
    {
        moves = gameObject.GetComponent<Moves>();
        agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();

        animator = gameObject.GetComponent<Animator>();

        initialSpeed = agent.speed;
        running = agent.speed * 2;

        yield return wait;

        state = Wander;
        Debug.Log("Thief: Wander state");

        while (enabled)
            yield return StartCoroutine(state());
    }

    IEnumerator Wander()
    {
        agent.speed = initialSpeed;
        animator.SetFloat("VelX", agent.speed);
        animator.SetFloat("VelY", agent.speed);

        while (Vector3.Distance(cop.position, treasure.transform.position) < dist2Steal)
        {
            moves.Wander();
            yield return wait;
        };

        Debug.Log("Thief: Approaching state");
        state = Approaching;
    }

    IEnumerator Approaching()
    {
        agent.speed = running;
        animator.SetFloat("VelX", agent.speed);
        animator.SetFloat("VelY", agent.speed);

        moves.Seek(treasure.transform.position);

        while (Vector3.Distance(cop.position, treasure.transform.position) > dist2Steal)
        {
            if (Vector3.Distance(treasure.transform.position, transform.position) < 1f)
            {
                Renderer[] childRenderers = treasure.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in childRenderers)
                {
                    renderer.enabled = false;
                }
                Debug.Log("Thief: Stolen");
                state = Hiding;
                break;
            }
            else
            { 
                state = Wander;
            }
            yield return wait;
        };
    }


    IEnumerator Hiding()
    {
        Debug.Log("Thief: Hiding state");

        while (true)
        {
            moves.Hide();
            yield return wait;
        };
    }
}
