using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMovement : MonoBehaviour
{
    public BoxCollider zone;
    public bool freeze;
    public Animator animator;

    private NavMeshAgent navMeshAgent;
    private bool inCoroutine = false;
    private Vector3 target = Vector3.zero;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (!inCoroutine && !freeze)
            StartCoroutine(Patrol());
    }

    private IEnumerator Patrol()
    {
        inCoroutine = true;

        while (target == Vector3.zero && !freeze)
        {
            yield return new WaitForSeconds(0.01f);
            target = CalculatePath();
            navMeshAgent.SetDestination(target);
        }

        if (animator != null)
            animator.SetBool(Animator.StringToHash("Moving"), true);

        while ((transform.position.x != target.x) && (transform.position.z != target.z) && !freeze)
        {
            yield return new WaitForSeconds(0.01f);
        }

        if (animator != null)
            animator.SetBool(Animator.StringToHash("Moving"), false);

        navMeshAgent.SetDestination(transform.position);
        target = Vector3.zero;
        inCoroutine = false;
    }

    private Vector3 CalculatePath()
    {
        Vector3 extents = zone.size / 2f;
        Vector3 point = new Vector3(
            Random.Range(-extents.x, extents.x),
            Random.Range(-extents.y, extents.y),
            Random.Range(-extents.z, extents.z)
        );
        NavMeshHit hit;

        if (NavMesh.SamplePosition(zone.transform.TransformPoint(point), out hit, 1.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return Vector3.zero;
    }
}
