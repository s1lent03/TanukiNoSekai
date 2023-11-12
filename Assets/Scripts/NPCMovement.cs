using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMovement : MonoBehaviour
{
    public float range;

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
            RandomPoint(transform.position, range, out target);
            navMeshAgent.SetDestination(target);
        }

        while ((transform.position.x != target.x) && (transform.position.z != target.z))
        {
            yield return new WaitForSeconds(0.01f);
        }

        target = Vector3.zero;
        inCoroutine = false;
    }

    private bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }
}
