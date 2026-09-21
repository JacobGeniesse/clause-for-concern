using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehavior : MonoBehaviour
{
    //Behavior Tree vars
    BehaviorTree actions;//Declaration for the Enemy's Behavior tree
    Node.Status treeStatus = Node.Status.RUNNING; //Current Status of the behavior tree

    //Patroling vars
    private List<GameObject> unclearedPatrols = new List<GameObject>(); //List for uncleared patrol points
    private List<GameObject> clearedPatrols = new List<GameObject>(); //List for cleared patrol points
    private int currentPatrol = 0; //Current patrol point of interest
    GameObject closestPatrol = null; //Closest patrol point

    [Header("Patrolling Variables")]
    [Tooltip("How close the enemy needs to be to a patrol point to clear it")]
    [SerializeField] private float clearDistance = 7; //Var for the distance at which the AI can clear a patrol point
    private bool increment = true; //Var for whether to mark a patrol point as cleared
    private float hearingDelay = 25; //Distance gaps to prevent constant path re-drawing

    //Chasing variables
    [Tooltip("To be linked with Task System, toggles if the enemy is capable of chasing the player")]
    /*[HideInInspector]*/ public bool aggressive = false; //Is the enemy capable of killing the player, to be linked with task system when that is ready
    private Vector3 interestingNoise; //Current noise to investigate
    private Transform playerTrans; //Ref to the player's transform
    private Vector3 lastKnownLocation; //player's last known location

    [Header("Movement Variables")]
    //List for storing various movement speeds
    [Tooltip("0 = standard,\n1 = investigating,\n2 = furious")]
    [SerializeField] private List<float> movementSpeed = new List<float>() 
    {
        3.5f,
        7f,
        10f
    };
    //List for storing various accelerations
    [Tooltip("0 = standard,\n1 = investigating,\n2 = furious")]
    [SerializeField] private List<float> accel = new List<float>() 
    {
        8f,
        16f,
        32f
    };
    private NavMeshAgent agent; //Reference to the nav mesh agent

    private float chaseTimer = 0f; //Var for how long the enemy will remain at max movement speed for
    [Header("Chasing Variables")]
    [Tooltip("How long the enemy will stay at high movement speed while pursuing the player")]
    [SerializeField] private float maxChaseTimer = 10f; //Maximum time the enemy will remain enraged for
    
    [Tooltip("How close the enemy needs to be in order to kill the player")]
    [SerializeField] private float killRange = 2; //Var for kill range of the enemy, currently debug only

    //Helper Functions
    private SightCheck sightCone;
    private Hearing hearing;

    void Start()
    {
        //Behavior tree definintion
        actions = new BehaviorTree();

        //Sequence Definitions
        Sequence patrol = new Sequence("Wander-Map");
        Sequence aggressivePatrol = new Sequence("Search-For-Player");

        //Leaf Definitions
        Leaf playerCheck = new Leaf("Look-For-Player", PlayerCheck);
        //Leaf obtainPoint = new Leaf("Get-new-POI", ObtainPOI);
        //Leaf passiveWander = new Leaf("Wander-Map", PassiveTravel);
        Leaf clearPoint = new Leaf("Clear-Curren-tTarget", ClearPOI);

        //Slot leaves under sequences

        patrol.AddChild(playerCheck);
        
        //patrol.AddChild(obtainPoint);
        //patrol.AddChild(passiveWander);
        patrol.AddChild(clearPoint);

        //Add Passive Patrol to the behavior tree list of actions
        actions.AddChild(patrol);

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

        //Find and assign the sight cone variable
        try
        {
            sightCone = this.GetComponent<SightCheck>();
        }
        catch
        {
            throw new ArgumentException("Unable to pair SightCheck with EnemyBehavior", nameof(EnemyBehavior));
        }

        //Find and assign the hearing variable
        try
        {
            hearing = GetComponent<Hearing>();
        }
        catch
        {
            throw new ArgumentException("Unable to pair Hearing script with EnemyBehavior", nameof(EnemyBehavior));
        }

        //Find and assign the player's transform
        try
        {
            playerTrans = GameObject.Find("PlayerBody").GetComponent<Transform>();
        }
        catch
        {
            throw new ArgumentException("Unable to find player's transform!", nameof(SightCheck));
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

        //If the chase timer is greater than 0 lower it
        if(chaseTimer > 0)
        {
            chaseTimer -= Time.deltaTime;
        }
    }

    //Node checks what task to undergo
    public Node.Status PlayerCheck()
    {
        //Check if the enemy is aggressive
        if(aggressive == true)
        {
            //If the chase timer is not active set the enemy's move speed to aggressive
            if (chaseTimer <= 0)
            {
                SetSpeed(1);
            }

            //Calculate player "distance" once for all functions
            Vector3 playerOffest = playerTrans.position - transform.position;
            float distanceToPlayer = Vector3.SqrMagnitude(playerOffest);
            if (sightCone.SeePlayer(playerTrans))
            {
                //If you can see the player while aggressive, chase them
                return ChasePlayer(distanceToPlayer);
            }
            else if (hearing.HearingCheck(distanceToPlayer) == true || hearing.unexaminedSound == true)
            {
                //If you can hear the player while aggressive, check if a new target is needed and move towards it
                increment = false;

                //set possible target for comparison
                Vector3 possibleTarget = hearing.importantSound;

                //Decalre local var for calcing distance from sound
                float distanceToTarget = 0;

                if (interestingNoise != Vector3.zero)
                {
                    //If there is a current interesting noise check based off of that
                    Vector3 currOffset = possibleTarget - interestingNoise;
                    distanceToTarget = Vector3.SqrMagnitude(currOffset);
                }

                //If the distance check goes through set what the interesting noise is
                if (distanceToTarget > hearingDelay || interestingNoise == Vector3.zero)
                {
                    interestingNoise = possibleTarget;
                }

                //Move towards the interesting noise
                return MoveToPOI(interestingNoise);
            }
            else
            {
                //If there is a last known location move towards that
                if (lastKnownLocation != Vector3.zero)
                {
                    return LosePlayer();
                }
                else
                { //Otherwise roam like normal
                    return ObtainPOI();
                }
            }
        }
        else
        {
            //If not aggressive just roam like normal
            return ObtainPOI();
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
                agent.CalculatePath(unclearedPatrols[i].transform.position, pathStorage); //Calc desired path
                if (pathStorage != null && !agent.pathPending)
                {
                    //determine the length of the path
                    Vector3 offsetPoint = pathStorage.corners[0] - transform.position;
                    float distanceToNode = Vector3.SqrMagnitude(offsetPoint);
                    //For each corner calc the distance between the points to get total distance
                    for (int j = 1; j < pathStorage.corners.Length; j++)
                    {
                        offsetPoint = pathStorage.corners[j] - pathStorage.corners[j - 1];
                        distanceToNode += Vector3.SqrMagnitude(offsetPoint);
                    }

                    //Check if the length of the path is shorter than what is currently stored
                    if (distanceToNode < lowestDistance || closestPatrol == null)
                    {
                        //Set new values if the check goes through
                        lowestDistance = distanceToNode;
                        closestPatrol = unclearedPatrols[i];
                        currentPatrol = i;
                    }
                }
            }

            SetSpeed(0); //Set speed

            //If there is a new patrol to travel to, proceed to next step
            if (closestPatrol != null)
            {
                return PassiveTravel();
            }
        }
        else if (closestPatrol != null)
        {
            return PassiveTravel();
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
            increment = true;
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
        //Clear other values that would cause infinite loops
        lastKnownLocation = Vector3.zero;
        interestingNoise = Vector3.zero;
        hearing.unexaminedSound = false;
        
        //If the current patrol is a valid patrol
        if (currentPatrol < unclearedPatrols.Count && increment == true)
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
        }
        return Node.Status.SUCCESS; //Once everything is done return success
    }

    //Func for chasing the player
    public Node.Status ChasePlayer(float playerDistance)
    {
        SetSpeed(2); //Set speed
        chaseTimer = maxChaseTimer; //Set the maxChaseTimer

        lastKnownLocation = playerTrans.position; //Update last known location

        if (!agent.isOnNavMesh)
        {
            //Throw an error if the agent isn't on the nav mesh
            Debug.LogError("Agent not on NavMesh!");
            return Node.Status.FAILURE;
        }

        //If the distance to the player is greater than the kill distance update the player's position
        if(playerDistance > killRange)
        {
            agent.SetDestination(playerTrans.position);
        }

        //If the player is within kill range give a debug and then ClearPOI
        if (playerDistance <= killRange)
        {
            Debug.Log("Gotcha!");
            //aggressive = false;
            increment = false;
            SetSpeed(0);
            return Node.Status.RUNNING;
        }
        else if(playerDistance > killRange)
        {
            return Node.Status.RUNNING; //Keep it running if outside kill range
        }
        return Node.Status.FAILURE; //If this is somehow reached fail the func
    }

    //Func for when the player has been lost during chase
    public Node.Status LosePlayer()
    {
        if (!agent.isOnNavMesh)
        {
            return Node.Status.FAILURE;
        }
        increment = false;
        return MoveToPOI(lastKnownLocation);
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

        if(agent.destination != destination)
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
        else if(distanceToNode <= clearDistance)
        {
            SetSpeed(0);
            //If not pass success
            return Node.Status.SUCCESS;
        }
        return Node.Status.FAILURE; //If something breaks fail the func
    }

    private void SetSpeed(int setSpeed)
    {
        agent.speed = movementSpeed[setSpeed];
        agent.acceleration = accel[setSpeed];
    }
}
