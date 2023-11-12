using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMovement : MonoBehaviour
{
    public BoxCollider zone;

    private NavMeshAgent navMeshAgent;
    private bool inCoroutine = false;
    private Vector3 target = Vector3.zero;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (!inCoroutine)
            StartCoroutine(Patrol());
    }

    private IEnumerator Patrol()
    {
        inCoroutine = true;

        while (target == Vector3.zero)
        {
            yield return new WaitForSeconds(0.01f);
            target = CalculatePath();
            navMeshAgent.SetDestination(target);
        }

        while ((transform.position.x != target.x) && (transform.position.z != target.z))
        {
            yield return new WaitForSeconds(0.01f);
        }

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
