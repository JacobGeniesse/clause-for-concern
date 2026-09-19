using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehavior : MonoBehaviour
{
    //Definition for the Enemy's Behavior tree
    BehaviorTree actions;

    //Current Status of the behavior tree
    Node.Status treeStatus = Node.Status.RUNNING;

    //List for uncleared patrol points
    private List<GameObject> unclearedPatrols = new List<GameObject>();

    //List for cleared patrol points
    private List<GameObject> clearedPatrols = new List<GameObject>();

    //Current patrol point of interest
    private int currentPatrol = 0;

    //Closest patrol point
    GameObject closestPatrol = null;


    //Reference to the nav mesh agent
    private NavMeshAgent agent;

    //Var for the distance at which the AI can clear a patrol point
    [SerializeField] private float clearDistance = 2;

    void Start()
    {
        //Behavior tree definintion
        actions = new BehaviorTree();

        //Sequence Definitions
        Sequence passivePatrol = new Sequence("Wander-Map");
        Sequence aggressivePatrol = new Sequence("Search-For-Player");

        //Leaf Definitions
        Leaf obtainPoint = new Leaf("Get-new-POI", ObtainPOI);
        Leaf passiveWander = new Leaf("Wander-Map", PassiveTravel);
        Leaf clearPoint = new Leaf("ClearCurrentTarget", ClearPOI);

        //Slot leaves under sequences
        passivePatrol.AddChild(obtainPoint);
        passivePatrol.AddChild(passiveWander);
        passivePatrol.AddChild(clearPoint);

        //Add Passive Patrol to the behavior tree list of actions
        actions.AddChild(passivePatrol);

        //Add Patrol points to uncleared patrols
        unclearedPatrols.AddRange(GameObject.FindGameObjectsWithTag("Patrol Node"));

        //Assign reference to NavMeshAgent
        try
        {
            agent = this.gameObject.GetComponent<NavMeshAgent>();
        }
        catch
        {
            //If this fails throw an error
            throw new ArgumentException("Unable to locate nav mesh agent!", nameof(EnemyBehavior));
        }
    }
    
    void Update()
    {
        //If the treeStatus is not successful then set it equal to the process active under actions
        if(treeStatus != Node.Status.SUCCESS)
        {
            treeStatus = actions.Process();
        }

        //If treeStatus is successful then set it back to running so the process can continue
        if(treeStatus == Node.Status.SUCCESS)
        {
            treeStatus = Node.Status.RUNNING;
        }
    }

    //Leaf for obtaining the next point of interest
    public Node.Status ObtainPOI()
    {
        //local var for the current lowest distance to travel
        float lowestDistance = 0;

        //Check if there are uncleared patrols to clear
        if (unclearedPatrols.Count > 0 && closestPatrol == null)
        {
            /*
             * For each node, calculate a path to the node, add up the distance between the corners of the path
             * and then compare to the current logged distance in order to determine if the node is less than
             * the current distance to figure out what the next node to search should be
             */
            for(int i = 0; i < unclearedPatrols.Count; i++)
            {
                //Calculate the path for comparison
                var pathStorage = new NavMeshPath();
                agent.CalculatePath(unclearedPatrols[i].transform.position, pathStorage);

                if (pathStorage != null && !agent.pathPending)
                {
                    //determine the length of the path
                    Vector3 offsetPoint = pathStorage.corners[0] - transform.position;
                    float distanceToNode = Vector3.SqrMagnitude(offsetPoint);
                    for (int j = 1; j < pathStorage.corners.Length; j++)
                    {
                        offsetPoint = pathStorage.corners[j] - pathStorage.corners[j - 1];
                        distanceToNode += Vector3.SqrMagnitude(offsetPoint);
                    }

                    //Check if the length of the path is shorter than what is currently stored
                    if (distanceToNode < lowestDistance || closestPatrol == null)
                    {
                        lowestDistance = distanceToNode;
                        closestPatrol = unclearedPatrols[i];
                        currentPatrol = i;
                    }
                }
            }

            //If there is a new patrol to travel to, proceed to next leaf
            if(closestPatrol != null)
            {
                return Node.Status.SUCCESS;
            }
        }
        //Fail the task is this is somehow reached
        Debug.LogWarning("Problem identified with ObtainPOI in EnemyBehavior!");
        return Node.Status.FAILURE;
    }

    //Leaf for travling to the node
    public Node.Status PassiveTravel()
    {
        //Run the function through MoveToPOI to move to the destination
        try
        {
            return MoveToPOI(closestPatrol.transform.position);
        }
        catch
        {
            return Node.Status.FAILURE;
            throw new ArgumentException("Unable to assign POI!", nameof(EnemyBehavior));
        }
    }

    //Leaf for clearing a point of interest and restarting the cycle
    public Node.Status ClearPOI()
    {
        //If the current patrol is a valid patrol
        if(currentPatrol < unclearedPatrols.Count)
        {
            //Move the current patrol from uncleared patrols to clearedPatrols
            clearedPatrols.Add(unclearedPatrols[currentPatrol]);
            unclearedPatrols.RemoveAt(currentPatrol);
            closestPatrol = null; //void the current closest patrol to prevent errors

            //If there are no more patrol in unclearedPatrols
            if(unclearedPatrols.Count == 0)
            {
                //Move all patrols from clearedPatrols back to unclearedPatrols
                while(clearedPatrols.Count > 0)
                {
                    unclearedPatrols.Add(clearedPatrols[0]);
                    clearedPatrols.RemoveAt(0);
                }
            }
            //Return succes if all goes well
            return Node.Status.SUCCESS;
        }
        return Node.Status.FAILURE;
    }

    //Helper Func for Moving to a point of interest
    public Node.Status MoveToPOI(Vector3 destination)
    {
        //If the agent isn't on the nav mesh fail the pass immediately
        if (!agent.isOnNavMesh)
        {
            Debug.LogError("Agent is not on a nav mesh!");
            return Node.Status.FAILURE;
        }

        //Calculate how far away the enemy is from the POI
        Vector3 offsetPOI = destination - transform.position;

        float distanceToNode = Vector3.SqrMagnitude(offsetPOI);

        agent.SetDestination(destination); //Set destination to the POI

        if (distanceToNode > clearDistance)
        {
            //If the node is farther from the clear distance, keep the tree on this node
            return Node.Status.RUNNING;
        }
        else if(distanceToNode <= clearDistance)
        {
            //If not pass success
            return Node.Status.SUCCESS;
        }
        return Node.Status.FAILURE; //If something breaks fail the func
    }
}
