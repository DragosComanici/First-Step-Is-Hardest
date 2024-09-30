using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float patrolSpeed = 3f;
    public float chaseSpeed = 6f;
    public float detectionRange = 10f;
    public Transform player;
    private int currentPatrolIndex;
    private bool chasingPlayer = false;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentPatrolIndex = 0;
        agent.speed = patrolSpeed;
        PatrolToNextPoint();
    }

    void Update()
    {
        // Check distance to the player
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer < detectionRange)
        {
            chasingPlayer = true;
        }
        else
        {
            chasingPlayer = false;
        }

        if (chasingPlayer)
        {
            ChasePlayer();
        }
        else
        {
            // Continue patrolling if not chasing
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                PatrolToNextPoint();
            }
        }
    }

    void PatrolToNextPoint()
    {
        if (patrolPoints.Length == 0)
            return;

        // Set the agent to go to the current patrol point.
        agent.destination = patrolPoints[currentPatrolIndex].position;

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    void ChasePlayer()
    {
        agent.speed = chaseSpeed; // Set AI speed for chasing
        agent.destination = player.position; // Follow the player
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
