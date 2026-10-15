using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MoveToPoint))]

public class TraverseNodes : MonoBehaviour
{
    [SerializeField]private List<Vector3> patrolPoints; //Patrol points to navigate between
    [SerializeField] private int currentPatrol = 0; //Current patrol point

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
        return true; //If all goes well return true
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

}
