using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class HRRepBehavior : MonoBehaviour
{
    #region Behavior Tree Vars
    private BehaviorTree actions; //Behavior tree vars
    Node.Status treeStatus = Node.Status.RUNNING; //variable for the status of the tree

    #endregion
    private NavMeshAgent agent; //Ref to the nav mesh agent

    [Tooltip("Is the enemy aggressive?")]
    [SerializeField] private bool aggressive = true; //Determines if the enemy is aggressive currently unused.

    #region hearing Vars
    private HearingSequence hearingNodes; //Ref to the hearing sequence script

    #endregion

    #region chase Vars
    private HRAttacks attackScript; //Ref to the attacking script

    private Vector3 lastKnownLocation; //Last spot the enemy spotted the player at
    private float attackDistance = 5f; //Max distance the enemy can attack from

    #endregion

    #region Stun Vars
    private bool stunned; //Is the enemy stunned

    private StunSequence stunSequence;

    #endregion


    #region cheat Vars

    private HRTeleport teleportScript; //Ref  to the teleportation script

    private float minCheatCooldown = 10; //Lowest time limit before the enemy can warp
    private float maxCheatCooldown = 25f; //Highest time limit before the enemy can warp
    private float cheatCooldown = 0; //Current warp cooldown

    #endregion


    private Transform playerTrans; //Point representing the player's position

    #region Helper Scripts
    private TraverseNodes patrolScript; //Helper script for patrolling between points
    private Sight sightScript; //Helper script for vision checks
    private MoveToPoint move; //Helper script for moving to a point

    private HRSpeedControl speedHandler;
    #endregion


    void Start()
    {
        //Assign the necessary script references
        AssignReference();

        //Create the HR Rep's Behavior Tree
        CreateBehaviorTree();

        //Add patrols
        AddPatrols();

        //Add warp points
        AssignWarpPoints();
    }

    private void Update()
    {
        //If the treeStatus is not successful then set it equal to the process active under actions
        if (treeStatus != Node.Status.SUCCESS)
        {
            treeStatus = actions.Process();
        }

        //If treeStatus is successful then set it back to running so the process can continue
        if (treeStatus == Node.Status.SUCCESS)
        {
            treeStatus = Node.Status.RUNNING;
        }

        //If the enemy's cheat cooldown is greater than 0 reduce it
        if (cheatCooldown > 0)
        {
            cheatCooldown -= Time.deltaTime;
        }
    }

    #region Chase Sequence Leaves
    
    //Node for the HR Vision check
    private Node.Status Look()
    {
        //Do a sight check, if the player is spotted, move towards them
        if (aggressive && sightScript.SightCheck(playerTrans))
        {
            lastKnownLocation = playerTrans.position; //Update the last known location
            //Debug.Log("I see you!"); //Debug Message
            move.MoveToPOI(agent, playerTrans.position); //Start moving towards the player's position

            if (speedHandler.ShouldChangeSpeed(1))
            {
                speedHandler.ChangeAgentSpeed(1); //Set the speed to the minimum
            }

            //If the player is within a certain range, continue to the next node for the attack sequence
            if (DistanceCheck(playerTrans) <= attackDistance)
            {
                return Node.Status.SUCCESS;
            }
            else
            {
                return Node.Status.RUNNING; //If not, continue moving towards the player
            }
        }
        else
        {
            return Node.Status.FAILURE; //Otherwise, return failure
        }
    }

    //!!! Other Leaves Contained in the "HRATTACKS" Script!!!

    #endregion

    #region Lose Player Leaves

    //Node for losing track of the player
    private Node.Status LosePlayer()
    {
        //If there is a last known location, move towards it
        if (aggressive && lastKnownLocation != Vector3.zero)
        {
            //Debug.Log("I can't see or hear you. I will investigate where I saw you last");
            return move.MoveToPOI(agent, lastKnownLocation);
        }
        else
        {
            //If not fail the func
            return Node.Status.FAILURE;
        }
    }

    //Node for resetting the chase and returning to patrolling behavior
    private Node.Status ResetChase()
    {
        //Reset the lastKnownLocation
        lastKnownLocation = Vector3.zero; //Clear the lastKnownLocation
        return Node.Status.SUCCESS; //End the Sequence
    }
    #endregion

    #region Patrol Sequence Leaves

    //Node for teleporting to a jumpscare location, currently not being called for some reason
    private Node.Status Teleport()
    {
        //If the enemy is aggressive and the cheatCooldown is less than 0, do a warp check
        if (aggressive && cheatCooldown <= 0)
        {
            return teleportScript.WarpCheck();
        }
        else
        {
            return Node.Status.FAILURE; //Otherwise skip this
        }
    }

    //Node for executing a warp to a jumpscare position
    private Node.Status ExecuteWarp()
    {
        Debug.Log("Warping"); //Debug Message
        return teleportScript.Teleport(); //Try to teleport
    }

    //Node for finishign a warp
    private Node.Status FinishWarp()
    {
        patrolScript.PickClosestPatrol(); //Pick the closest patrol node to continue patroling from
        cheatCooldown = UnityEngine.Random.Range(minCheatCooldown, maxCheatCooldown); //Set the cheat cooldown to a random value between the min and max
        return Node.Status.SUCCESS; //End the node
    }

    //Node for patrolling behavior
    private Node.Status Wander()
    {
        if (speedHandler.ShouldChangeSpeed(0))
        {
            speedHandler.ChangeAgentSpeed(0); //Set the speed to the minimum
        }
        return patrolScript.Patrol(agent); //Return the result of the Patrol function.
    } 

    //Node for incrementing to the next patrol point
    private Node.Status ResetPatrol()
    {
        Debug.Log("Reset"); //Debug Message indicating a reset
        patrolScript.IncrementPatrol(); //Move to the next patrol point
        return Node.Status.SUCCESS; //End the sequence
    }
    #endregion

    //Helper Func for calculating a distance check
    private float DistanceCheck(Transform playerTrans)
    {
        Vector3 playerOffest = playerTrans.position - transform.position;
        float distanceToPlayer = Vector3.SqrMagnitude(playerOffest);
        return distanceToPlayer;
    }

    //Func for making the enemy aggressive
    public void MakeAggressive()
    {
        aggressive = true;
        cheatCooldown = UnityEngine.Random.Range(minCheatCooldown, maxCheatCooldown);
    }


    #region Start Helpers
    //Function for adding the patrol points for the TraverseNodes script
    void AddPatrols()
    {
        List<Vector3> nodePoints = GrabTransforms("Patrol Node");

        //Assign the patrol points in conversionList to the patrol script
        if (patrolScript != null && nodePoints.Count > 0)
        {
            patrolScript.AssignPatrolPoints(nodePoints);
        }
        else
        {
            Debug.LogError("Patrol Script not assigned!");
        }
    }

    //Func for assigning the points the enemy can jump to for a jumpscare
    private void AssignWarpPoints()
    {
        List<Vector3> fillerList = GrabTransforms("TeleportSpot");
        //Assign the patrol points in conversionList to the patrol script
        if (teleportScript != null && fillerList.Count > 0)
        {
            teleportScript.AddWarps(fillerList);
        }
        else
        {
            Debug.LogError("Teleport Script not assigned!");
        }
    }

    private List<Vector3> GrabTransforms(string tag)
    {
        //Grab all node game objects
        List<GameObject> fillerList = GameObject.FindGameObjectsWithTag(tag).ToList();

        //Convert all of the fillerList GameObjects into a transform point
        List<Vector3> conversionList = new List<Vector3>();

        //If there are patrol points in filler list
        if (fillerList.Count > 0)
        {

            foreach (GameObject GM in fillerList)
            {
                conversionList.Add(GM.transform.position);
            }

            return conversionList;
        }
        else
        {
            Debug.LogError("No patrol nodes found!");
            return conversionList;
        }
    }



    //Func run at start to assign the references for the script
    private bool AssignReference()
    {
        //Get the TraverseNodes component
        if (TryGetComponent<TraverseNodes>(out TraverseNodes nodeScript))
        {
            patrolScript = nodeScript;
        }
        else
        {
            //throw an error if this fails and return false
            Debug.LogError($"No TraverseNodes script attached to {this.gameObject}!");
            return false;
        }

        //Get the Sight component
        try
        {
            sightScript = GetComponentInChildren<Sight>();
        }
        catch
        {
            //Give an error if this fails and return false
            Debug.LogError($"No Sight script attached to {this.gameObject}");
            return false;
        }

        //Get the MoveToPoint component
        try
        {
            move = GetComponentInParent<MoveToPoint>();
        }
        catch
        {
            //Give an error if this fails and returns false
            Debug.LogError($"No MoveToPoint script attached to {this.transform.parent}!");
            return false;
        }

        //Get reference to the nav mesh agent component
        if (TryGetComponent<NavMeshAgent>(out NavMeshAgent navAgent))
        {
            agent = navAgent;
        }
        else
        {
            //Give an error message if this fails and return false
            Debug.LogError($"No NavMeshAgent attached to {this.gameObject}!");
            return false;
        }

        //Get reference to the jumpscare teleport script
        if (TryGetComponent<HRTeleport>(out HRTeleport tele))
        {
            teleportScript = tele;
        }
        else
        {
            Debug.LogError($"No HRTeleport script attached to {this.gameObject}!");
            return false;
        }

        //Add the attacking script
        if (TryGetComponent<HRAttacks>(out HRAttacks attack))
        {
            attackScript = attack;
        }
        else
        {
            Debug.LogError($"No HRAttacks script attached to {this.gameObject}!");
            return false;
        }

        //Add the hearingSequence
        try
        {
            hearingNodes = GetComponentInChildren<HearingSequence>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign HearingSequence component", nameof(HRRepBehavior));
        }

        //Add the HRSpeedController component
        try
        {
            speedHandler = GetComponent<HRSpeedControl>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign HRSpeedController component", nameof(HRRepBehavior));
        }

        try
        {
            stunSequence = GetComponent<StunSequence>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign StunSequence component", nameof(HRRepBehavior));
        }
        //Get the player body component
        playerTrans = GameObject.Find("PlayerBody").GetComponent<Transform>();

        return true; //If everything works properly return true
    }

    //Func run at start to create the behavior tree
    private void CreateBehaviorTree()
    {
        //SECTION: BEHAVIOR TREE SETUP
        actions = new BehaviorTree(); //Define behavior tree

        Selector sensoryInput = new Selector("Sense Environment"); //Selector for determining enemy behavior

        //Define the stunned sequence
        Sequence stun = new Sequence("Stunned-Cooldown"); //Sequence for the enemy stun
        Leaf stunCheck = new Leaf("Check-For-Stun", stunSequence.StunCheck); //Leaf 1: check if the enemy is stunned
        Leaf activeStun = new Leaf("Execute-Stun", stunSequence.ExecuteStun); //Leaf 2: the actual stun coroutine
        Leaf stunEnd = new Leaf("End-Stun", stunSequence.EndStun); //Leaf 3: End the stun of the enemy
        //Add leaves to the sequence
        stun.AddChild(stunCheck);
        stun.AddChild(activeStun);
        stun.AddChild(stunEnd);


        //Define the patrol sequence
        Selector patrolMethod = new Selector("Teleport-or-Walk"); //Selector for determining if the enemy should teleport to a jumpscare spot or patrol the map normally
        Sequence teleport = new Sequence("Teleport"); //Sequence for if the enemy teleports to a jumpscare spot
        Leaf warpCheck = new Leaf("Warp-Check", Teleport); //Leaf 1: Check if enemy can teleport
        Leaf warpExecute = new Leaf("Warp-Execute", ExecuteWarp); //Leaf 2: Execute the teleport
        Leaf warpFinish = new Leaf("Finish-Warp", FinishWarp); //Leaf 3: Finish the teleport
        Sequence patrol = new Sequence("Wander-Map"); //Create sequence for patrolling
        Leaf wander = new Leaf("Sensory-Input", Wander); //Leaf 1: Pick a node and travel to it
        Leaf clearPatrol = new Leaf("Clear-Patrol-Points", ResetPatrol); //Leaf 2: Clear the node and prep for the next one
        //Add leaves to the sequence
        teleport.AddChild(warpCheck);
        teleport.AddChild(warpExecute);
        teleport.AddChild(warpFinish);
        patrolMethod.AddChild(teleport);
        patrol.AddChild(wander);
        patrol.AddChild(clearPatrol);
        patrolMethod.AddChild(patrol);


        //Define the chase sequence
        Sequence chase = new Sequence("Chase-Player"); //Make the sequence
        Leaf look = new Leaf("Sight-Check", Look); //Leaf 1: Conduct a sight check and if it passes, go to that point
        //Add leaves to the sequence
        chase.AddChild(look);

        //Attacking segments of the chase
        Selector violence = new Selector("Try-Attack"); //Selector for grabbing the player
        Sequence grabSuccess = new Sequence("Succeful-Grab"); //If the enemy successfully grabs the player
        Leaf tryGrab = new Leaf("Try-to-Grab", attackScript.GrabAttempt); //Leaf 1: Try to grab the player
        Leaf grabAttack = new Leaf("Grab-Attack", attackScript.GrabAndThrow); //Leaf 2: Execute the grab attack coroutine
        Leaf endGrab = new Leaf("End-of-Grab", attackScript.EndGrab); //Leaf 3: End the grab and resume the chase
        Sequence grabFail = new Sequence("Failed-Grab"); //Backup if the enemy fails to grab the player
        Leaf failGrab = new Leaf("Fail-to-Grab", attackScript.FailGrab); //Leaf 1: Reset and try to resume the chase
        //Add leaves to the sequence
        chase.AddChild(violence);
        violence.AddChild(grabSuccess);
        grabSuccess.AddChild(tryGrab);
        grabSuccess.AddChild(grabAttack);
        grabSuccess.AddChild(endGrab);
        violence.AddChild(grabFail);
        grabFail.AddChild(failGrab);



        //Define the losePlayer sequence
        Sequence losePlayer = new Sequence("Lose-Player"); //Make the sequence
        Leaf lose = new Leaf("Investigate-Last-Known-Location", LosePlayer); //Leaf 1: Investigate the last known location if it isn't 0
        Leaf endChase = new Leaf("End-Chase", ResetChase); //Leaf 2: Clear the last-known-location and prepare to start running through nodes again
        //Add the leaves to the sequence
        losePlayer.AddChild(lose);
        losePlayer.AddChild(endChase);

        //Define the hearing sequence
        Sequence hear = new Sequence("Listen"); //Make the sequence
        Leaf listen = new Leaf("Audio-Input", hearingNodes.Hear); //Leaf 1: Conduct hearing check
        Leaf investigateSound = new Leaf("Investigate-Sound", hearingNodes.InvestigateSound); //Leaf 2: Investigate the origin point of the sound
        Leaf clearSound = new Leaf("Clear-Sound-Location", hearingNodes.DismissSound); //Leaf 3: After reaching the sound, clear the point of origin.
        //Add the leaves to the hear sequence.
        hear.AddChild(listen);
        hear.AddChild(investigateSound);
        hear.AddChild(clearSound);


        //Add the different behavior routes to the selector
        sensoryInput.AddChild(stun);
        sensoryInput.AddChild(chase);
        sensoryInput.AddChild(hear);
        sensoryInput.AddChild(losePlayer);
        sensoryInput.AddChild(patrolMethod);


        actions.AddChild(sensoryInput); //Add sensory input to the master behavior tree
    }
    #endregion
}