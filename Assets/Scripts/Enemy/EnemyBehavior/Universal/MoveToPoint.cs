using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MoveToPoint : MonoBehaviour
{

    [Tooltip("How close the enemy needs to be to a patrol point to clear it")]
    [SerializeField] private float clearDistance = 7; //Var for the distance at which the AI can clear a patrol point


    //Helper Func for Moving to a point of interest
    public Node.Status MoveToPOI(NavMeshAgent agent, Vector3 destination)
    {
        //If the agent isn't on the nav mesh fail the pass immediately
        if (!agent.isOnNavMesh)
        {
            Debug.LogError($"{this.gameObject} is not on a nav mesh!");
            return Node.Status.FAILURE;
        }

        if (agent.destination != destination)
        {
            agent.SetDestination(destination); //Set destination to the POI
        }

        //Calculate how far away the enemy is from the POI
        Vector3 offsetPOI = destination - transform.position;

        float distanceToNode = Vector3.SqrMagnitude(offsetPOI);

        if (distanceToNode > clearDistance)
        {
            //If the node is farther from the clear distance, keep the tree on this node
            return Node.Status.RUNNING;
        }
        else if (distanceToNode <= clearDistance)
        {
            //SetSpeed(0);
            //If not pass success
            return Node.Status.SUCCESS;
        }
        return Node.Status.FAILURE; //If something breaks fail the func
    }

}
