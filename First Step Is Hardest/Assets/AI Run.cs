using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolAndFlee : MonoBehaviour
{
    public Transform[] patrolPoints; // Points for patrolling
    public Transform player; // Reference to the player
    public float patrolSpeed = 3f;
    public float fleeSpeed = 12f;
    public float detectionRange = 10f; // Range to detect the player
    public float fleeDistance = 15f; // Distance to flee from the player

    private int currentPatrolIndex = 0;
    private NavMeshAgent agent;
    private bool isFleeing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
        GoToNextPatrolPoint();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer < detectionRange)
        {
            isFleeing = true;
        }
        else
        {
            isFleeing = false;
        }

        if (isFleeing)
        {
            FleeFromPlayer();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPatrolPoint();
        }
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0)
            return;

        agent.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    void FleeFromPlayer()
    {
        agent.speed = fleeSpeed;
        Vector3 fleeDirection = (transform.position - player.position).normalized * fleeDistance;
        Vector3 fleeDestination = transform.position + fleeDirection;

        // Set a target position that ensures the AI flees from the player
        agent.destination = fleeDestination;
    }
}
