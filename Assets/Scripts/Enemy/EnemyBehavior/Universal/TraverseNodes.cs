using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MoveToPoint))]

public class TraverseNodes : MonoBehaviour
{
    private List<Vector3> patrolPoints;
    private int currentPatrol = 0;

    private MoveToPoint move;
    private NavMeshAgent agent;

    void Start()
    {
        if (TryGetComponent<MoveToPoint>(out MoveToPoint moveComponent))
        {
            move = moveComponent;
        }
        else
        {
            Debug.LogError($"No MoveToPoint script attached to {this.gameObject}!");
        }

        if (TryGetComponent<NavMeshAgent>(out NavMeshAgent navAgent))
        {
            agent = navAgent;
        }
        else
        {
            Debug.LogError($"No NavMeshAgent attached to {this.gameObject}!");
        }
    }

    public bool AssignPatrolPoints(List<Vector3> nodes)
    {
        try
        {
            patrolPoints.AddRange(nodes);
        }
        catch
        {
            return false;
            throw new ArgumentException("Unable to assign patrol points", nameof(TraverseNodes));
        }
        return true;
    }

    public Node.Status Patrol()
    {
        if (patrolPoints.Count == 0)
        {
            Debug.LogWarning("No Patrol Points to Move Between");
            return Node.Status.FAILURE;
        }
        
        return move.MoveToPOI(agent, patrolPoints[currentPatrol]);
    }

    public void IncrementPatrol()
    {
        if(currentPatrol + 1 < patrolPoints.Count)
        {
            currentPatrol++;
        }
        else
        {
            currentPatrol = 0;
        }
    }

}
