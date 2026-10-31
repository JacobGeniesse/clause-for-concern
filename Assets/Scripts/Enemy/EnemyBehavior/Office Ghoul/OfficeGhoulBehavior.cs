using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms.Impl;

public class OfficeGhoulBehavior : MonoBehaviour
{
    private BehaviorTree actions; //Behavior Tree Reference
    private Node.Status treeStatus = Node.Status.RUNNING;

    [Tooltip("Does the enemy wander or does it idle in one place?")]
    [SerializeField] private bool wandering = false;
    [SerializeField, HideInInspector] private Vector3 originPoint; //Spawn point of the enemy

    [Tooltip("Range the enemy can wander around the origin point.")]
    [SerializeField] private float wanderExtents = 10f;

    [Tooltip("Show the editor elements?")]
    [SerializeField] private bool showHandles = true;
    
    private Transform playerTrans; //Reference to the player's transform

    #region Helper Scripts
    private NavMeshAgent agent; //Reference to the nav mesh agent
    private MoveToPoint move; //Reference to the movetopoint script
    private StunSequence stunSequence; //Reference to the stun sequence script
    private HearingSequence hearingSequence; //Reference to the hearing sequence script
    private OGAttacks attackScript; //Refrence to the office ghoul's attacking script
    private OfficeGhoulPatrol patrolScript; //Reference to the patrolling script
    #endregion

    void Start()
    {
        originPoint = transform.position; //Set the origin point

        AssignReferences(); //Assign variable references

        CreateBehaviorTree(); //Create the behavior tree

        //Debug message to prevent a warning in console
        if (showHandles == true)
        {
            Debug.Log("Showing handles!");
        }
    }

    void Update()
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
    }

    //Func for assigning variable references
    private void AssignReferences()
    {
        //Assign reference to the stun sequence
        try
        {
            stunSequence = GetComponent<StunSequence>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign StunSequence component", nameof(OfficeGhoulBehavior));
        }

        //Assign reference to the hearingSequence
        try
        {
            hearingSequence = GetComponentInChildren<HearingSequence>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign HearingSequence component", nameof(OfficeGhoulBehavior));
        }

        //assign reference to the player's transform
        try
        {
            playerTrans = GameObject.Find("PlayerBody").GetComponent<Transform>();
        }
        catch
        {
            throw new ArgumentException("Unable to obtain player body component", nameof(OfficeGhoulBehavior));
        }

        //assign reference to the move to point script
        try
        {
            move = GetComponentInChildren<MoveToPoint>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign MoveToPoint component", nameof(OfficeGhoulBehavior));
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
        }
        
        //assign reference to the attacking script
        try
        {
            attackScript = GetComponentInChildren<OGAttacks>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign OGAttacks component", nameof(OfficeGhoulBehavior));
        }

        //assign reference to the patrolling script
        try
        {
            patrolScript = GetComponentInChildren<OfficeGhoulPatrol>();
            patrolScript.InitializeVars(originPoint, wanderExtents, wandering);
        }
        catch
        {
            throw new ArgumentException("Unable to assign OfficeGhoulPatrol component", nameof(OfficeGhoulBehavior));
        }
    }

    //Function for defining the behavior tree
    private void CreateBehaviorTree()
    {
        //SECTION 0: BEHAVIOR TREE SETUP
        actions = new BehaviorTree(); //Define behavior tree

        Selector choices = new Selector("Pick Behavior"); //Selector for determining enemy behavior


        //SECTION 1: STUN SEQUENCE
        Sequence stun = new Sequence("Stunned-Cooldown"); //Sequence for the enemy stun
        Leaf stunCheck = new Leaf("Check-For-Stun", stunSequence.StunCheck); //Leaf 1: check if the enemy is stunned
        Leaf activeStun = new Leaf("Execute-Stun", stunSequence.ExecuteStun); //Leaf 2: the actual stun coroutine
        Leaf stunEnd = new Leaf("End-Stun", stunSequence.EndStun); //Leaf 3: End the stun of the enemy
        //Add leaves to the sequence
        stun.AddChild(stunCheck);
        stun.AddChild(activeStun);
        stun.AddChild(stunEnd);

        //SECTION 2: CHASE SEQUENCE
        Sequence chase = new Sequence("Chase"); //Sequence for chasing the player
        Leaf look = new Leaf("Scan-for-Player", attackScript.Look); //Leaf 1: Check if the enemy can see the player
        Leaf lunge = new Leaf("Lunge-at-Player", attackScript.Lunge); //Leaf 2: Try to lunge at the player

        Selector lungeResult = new Selector("Grab-or-No"); //Selector for determining if this is a successful lunge
        
        Leaf attackConfirmation = new Leaf("Grab-Check", attackScript.GrabCheck); //Leaf A1: Confirm that the player was hit
        Leaf attack = new Leaf("Attack-Player", attackScript.Attack); //Leaf A2: Execute the attack
        Sequence successfulLunge = new Sequence("Player-Grab"); //Sequence for the attack
        //Assign the leaves to the sequence
        successfulLunge.AddChild(attackConfirmation);
        successfulLunge.AddChild(attack);
        
        Leaf failureConfirmation = new Leaf("Failed-Grab", attackScript.FailureConfirmation); //Leaf B1: Confirm failure
        Leaf attackFail = new Leaf("Attack-Failed", attackScript.GrabFail); //Leaf B2: Execute the failure cooldown
        Sequence failedLunge = new Sequence("Grab-Failure"); //Sequence for failed attack
        //Assign the leaves to the sequence
        failedLunge.AddChild(failureConfirmation);
        failedLunge.AddChild(attackFail);
        
        //Add the sequences to the selector
        lungeResult.AddChild(successfulLunge);
        lungeResult.AddChild(failedLunge);
        //Add leaves to the sequence
        chase.AddChild(look);
        chase.AddChild(lunge);
        chase.AddChild(lungeResult);


        //SECTION 3: HEARING SEQUENCE
        Sequence hear = new Sequence("Listen"); //Make the sequence
        Leaf listen = new Leaf("Audio-Input", hearingSequence.Hear); //Leaf 1: Conduct hearing check
        Leaf investigateSound = new Leaf("Investigate-Sound", hearingSequence.InvestigateSound); //Leaf 2: Investigate the origin point of the sound
        Leaf clearSound = new Leaf("Clear-Sound-Location", hearingSequence.DismissSound); //Leaf 3: After reaching the sound, clear the point of origin.
        //Add the leaves to the hear sequence.
        hear.AddChild(listen);
        hear.AddChild(investigateSound);
        hear.AddChild(clearSound);

        //SECTION 4: PATROLING SEQUENCE
        Sequence patrol = new Sequence("Patrol"); //Make the sequence
        Selector retreatCheck = new Selector("Check-Retreat"); //Selector for if the enemy should retreat
        
        Sequence retreat = new Sequence("Back-to-Origin"); //Sequence for returning to the origin point
        Leaf verify = new Leaf("Should-this-Retreat", patrolScript.CheckPosition); //Leaf 1: Check if the enemy should return
        Leaf flee = new Leaf("Return to origin", patrolScript.ReturnToOrigin); //Leaf 2: Return to the origin point
        //Add leaves to sequence
        retreat.AddChild(verify);
        retreat.AddChild(flee);

        Sequence wander = new Sequence("Wander-Around"); //Sequence for wandering

        Selector wanderCheck = new Selector("Should-this-Wander"); //Selector for choosing the enemy should wander or idle
        wander.AddChild(wanderCheck); //Add the wander check sequence to the wander sequence

        Sequence roam = new Sequence("Wandering-Sequence"); //Sequence for wandering
        Leaf check = new Leaf("Check-To-Patrol", patrolScript.WanderCheck); //Leaf 1: Check if this should wander
        Leaf move = new Leaf("Wander-To-Node", patrolScript.Wander); //Leaf 2: Wander towards a point
        Leaf reset = new Leaf("ClearWander", patrolScript.ClearWander); //Leaf 3: Clear the wandering point and wait a cooldown period
        
        //Add leaves to sequence
        roam.AddChild(check);
        roam.AddChild(move);
        roam.AddChild(reset);

        Leaf idle = new Leaf("Stand-Still", patrolScript.Idle); //Leaf for idling

        //Add the different options the wander check selector
        wanderCheck.AddChild(roam);
        wanderCheck.AddChild(idle);

        //Add the different options to the retreat check selector
        retreatCheck.AddChild(retreat);
        retreatCheck.AddChild(wanderCheck);

        //Add retreat check to the patrol sequence
        patrol.AddChild(retreatCheck);

        //SECTION 5: COMPILE TREE

        //Assign the different choices the enemy has in order of priority
        choices.AddChild(stun);
        choices.AddChild(chase);
        choices.AddChild(hear);
        choices.AddChild(patrol);

        actions.AddChild(choices); //Make choices the child of the behavior tree
    }
}