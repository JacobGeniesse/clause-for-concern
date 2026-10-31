using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MoveToPoint))]

public class TraverseNodes : MonoBehaviour
{
    [SerializeField, HideInInspector]private List<Vector3> patrolPoints; //Patrol points to navigate between
    [SerializeField, HideInInspector] private int currentPatrol = 0; //Current patrol point

    private MoveToPoint move; //Ref to the move to point script

    void Start()
    {
        //Assign the MoveToPoint script ref
        if (TryGetComponent<MoveToPoint>(out MoveToPoint moveComponent))
        {
            move = moveComponent;
        }
        else
        {
            Debug.LogError($"No MoveToPoint script attached to {this.gameObject}!");
        }
    }

    //Helper func for assigning the patrol points list. Doing it this way allows for modularity for other patroling enemies who have a different sample pool
    public bool AssignPatrolPoints(List<Vector3> nodes)
    {
        try
        {
            patrolPoints.AddRange(nodes); //Try to add the nodes
        }
        catch
        {
            //If this fails for whatever reason, return false and throw an error
            return false;
            throw new ArgumentException("Unable to assign patrol points", nameof(TraverseNodes));
        }

        PickClosestPatrol(); //Pick the closest node for the enemy to path to

        return true; //If all goes well return true
    }

    //Helper func for picking the closest node to patrol towards
    public void PickClosestPatrol()
    {
        //Assign placeholder vars for the closest node and the closeness of that node
        int closestNodeSlot = 0; 
        float closeness = Mathf.Infinity;

        //For each node, calc its proximity to the enemy and determine if its closer than another node
        for (int i = 0; i < patrolPoints.Count; i++)
        {
            float disToNode = DistanceCheck(transform, patrolPoints[i]);

            //If that node is closer to the enemy than other nodes, mark it as the closest node and repeat until no nodes remain
            if(disToNode < closeness)
            {
                closestNodeSlot = i;
                closeness = disToNode;
            }
        }

        currentPatrol = closestNodeSlot; //After going through every node, assign the currentPatrol to the closestNodeSlot
    }

    //Helper func for moving to a patrol point
    public Node.Status Patrol(NavMeshAgent agent)
    {
        //If there are no patrol points to move between return failure
        if (patrolPoints.Count == 0)
        {
            Debug.LogWarning("No Patrol Points to Move Between");
            return Node.Status.FAILURE;
        }
        
        //Otherwise return the progress in moving between the current nodes
        return move.MoveToPOI(agent, patrolPoints[currentPatrol]);
    }

    //Helper func for incrementing the current patrol point target
    public void IncrementPatrol()
    {
        //If increasing the current patrol won't cause an overflow error increase it
        if(currentPatrol + 1 < patrolPoints.Count)
        {
            currentPatrol++;
        }
        else
        {
            //If it will cause an overflow reset it to 0
            currentPatrol = 0;
        }
    }

    private float DistanceCheck(Transform playerTrans, Vector3 nodePos)
    {
        Vector3 playerOffest = playerTrans.position - nodePos;
        float distanceToPlayer = Vector3.SqrMagnitude(playerOffest);
        return distanceToPlayer;
    }
}
