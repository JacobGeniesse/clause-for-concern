using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms.Impl;

public class OfficeGhoulBehavior : MonoBehaviour
{
    private BehaviorTree actions; //Behavior Tree Reference
    private Node.Status treeStatus = Node.Status.RUNNING;

    [SerializeField] private bool wandering = false;

    [SerializeField] private Vector3 originPoint;
    [SerializeField] private float wanderExtents = 10f;

    private Transform playerTrans;

    private NavMeshAgent agent;
    private MoveToPoint move;
    private StunSequence stunSequence;
    private HearingSequence hearingSequence;

    private OGAttacks attackScript;

    private OfficeGhoulPatrol patrolScript;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //originPoint = transform.position;

        AssignReferences();

        CreateBehaviorTree();
    }

    // Update is called once per frame
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

    private void AssignReferences()
    {
        try
        {
            stunSequence = GetComponent<StunSequence>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign StunSequence component", nameof(OfficeGhoulBehavior));
        }

        try
        {
            hearingSequence = GetComponentInChildren<HearingSequence>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign HearingSequence component", nameof(OfficeGhoulBehavior));
        }

        try
        {
            playerTrans = GameObject.Find("PlayerBody").GetComponent<Transform>();
        }
        catch
        {
            throw new ArgumentException("Unable to obtain player body component", nameof(OfficeGhoulBehavior));
        }

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
        
        try
        {
            attackScript = GetComponentInChildren<OGAttacks>();
            attackScript.SetOrigin(originPoint, wanderExtents);
        }
        catch
        {
            throw new ArgumentException("Unable to assign OGAttacks component", nameof(OfficeGhoulBehavior));
        }

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

    private void CreateBehaviorTree()
    {
        //SECTION: BEHAVIOR TREE SETUP
        actions = new BehaviorTree(); //Define behavior tree

        Selector choices = new Selector("Pick Behavior"); //Selector for determining enemy behavior

        Sequence stun = new Sequence("Stunned-Cooldown"); //Sequence for the enemy stun
        Leaf stunCheck = new Leaf("Check-For-Stun", stunSequence.StunCheck); //Leaf 1: check if the enemy is stunned
        Leaf activeStun = new Leaf("Execute-Stun", stunSequence.ExecuteStun); //Leaf 2: the actual stun coroutine
        Leaf stunEnd = new Leaf("End-Stun", stunSequence.EndStun); //Leaf 3: End the stun of the enemy
        //Add leaves to the sequence
        stun.AddChild(stunCheck);
        stun.AddChild(activeStun);
        stun.AddChild(stunEnd);


        Sequence chase = new Sequence("Chase");
        Leaf look = new Leaf("Scan-for-Player", attackScript.Look);
        Leaf lunge = new Leaf("Lunge-at-Player", attackScript.Lunge);
        Selector lungeResult = new Selector("Grab-or-No");
        Leaf attackConfirmation = new Leaf("Grab-Check", attackScript.GrabCheck);
        Leaf attack = new Leaf("Attack-Player", attackScript.Attack);
        Sequence successfulLunge = new Sequence("Player-Grab");
        successfulLunge.AddChild(attackConfirmation);
        successfulLunge.AddChild(attack);
        Leaf failureConfirmation = new Leaf("Failed-Grab", attackScript.FailureConfirmation);
        Leaf attackFail = new Leaf("Attack-Failed", attackScript.GrabFail);
        Sequence failedLunge = new Sequence("Grab-Failure");
        failedLunge.AddChild(failureConfirmation);
        failedLunge.AddChild(attackFail);

        lungeResult.AddChild(successfulLunge);
        lungeResult.AddChild(failedLunge);
        //Add leaves to the sequence
        chase.AddChild(look);
        chase.AddChild(lunge);
        chase.AddChild(lungeResult);


        //Define the hearing sequence
        Sequence hear = new Sequence("Listen"); //Make the sequence
        Leaf listen = new Leaf("Audio-Input", hearingSequence.Hear); //Leaf 1: Conduct hearing check
        Leaf investigateSound = new Leaf("Investigate-Sound", hearingSequence.InvestigateSound); //Leaf 2: Investigate the origin point of the sound
        Leaf clearSound = new Leaf("Clear-Sound-Location", hearingSequence.DismissSound); //Leaf 3: After reaching the sound, clear the point of origin.
        //Add the leaves to the hear sequence.
        hear.AddChild(listen);
        hear.AddChild(investigateSound);
        hear.AddChild(clearSound);



        //Check if the enemy should retreat
        //If yes, execute the retreat sequence
        //If no, check if the enemy can wander
            //If yes, execute the wandering sequence
            //If no, stand still

        Sequence patrol = new Sequence("Patrol");
        Selector retreatCheck = new Selector("Check-Retreat");
        
        Sequence retreat = new Sequence("Back-to-Origin");
        Leaf verify = new Leaf("Should-this-Retreat", patrolScript.CheckPosition);
        Leaf flee = new Leaf("Return to origin", patrolScript.ReturnToOrigin);
        retreat.AddChild(verify);
        retreat.AddChild(flee);

        Sequence wander = new Sequence("Wander-Around");

        Selector wanderCheck = new Selector("Should-this-Wander");
        wander.AddChild(wanderCheck);

        Sequence roam = new Sequence("Wandering-Sequence");
        Leaf check = new Leaf("Check-To-Patrol", patrolScript.WanderCheck);
        Leaf move = new Leaf("Wander-To-Node", patrolScript.Wander);
        Leaf reset = new Leaf("ClearWander", patrolScript.ClearWander);
        roam.AddChild(check);
        roam.AddChild(move);
        roam.AddChild(reset);

        Sequence standStill = new Sequence("Idling");
        Leaf idle = new Leaf("Stand-Still", patrolScript.Idle);
        standStill.AddChild(idle);

        wanderCheck.AddChild(roam);
        wanderCheck.AddChild(idle);
        retreatCheck.AddChild(retreat);
        retreatCheck.AddChild(wanderCheck);
        patrol.AddChild(retreatCheck);

        choices.AddChild(stun);
        choices.AddChild(chase);
        choices.AddChild(hear);
        choices.AddChild(patrol);

        actions.AddChild(choices);
    }
}
