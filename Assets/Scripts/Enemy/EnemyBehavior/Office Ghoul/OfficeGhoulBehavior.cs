using System;
using UnityEngine;
using UnityEngine.AI;

public class OfficeGhoulBehavior : MonoBehaviour
{
    private BehaviorTree actions;
    private Node.Status treeStatus = Node.Status.RUNNING;

    private Transform playerTrans;

    [SerializeField] private Collider attackCol;

    private NavMeshAgent agent;
    private MoveToPoint move;
    private Sight sight;
    private StunSequence stunSequence;
    private HearingSequence hearingSequence;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

    private Node.Status Idle()
    {
        //For having the Office Ghoul stand still in an area
        return Node.Status.SUCCESS;
    }

    private Node.Status Look()
    {
        if (sight.SightCheck(playerTrans))
        {
            move.MoveToPOI(agent, playerTrans.position); //Start moving towards the player's position
            if (attackCol.bounds.Contains(playerTrans.position))
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
            return Node.Status.FAILURE;
        }
    }

    private Node.Status Chase()
    {
        return Node.Status.SUCCESS;
        //If the player is within a certain range, continue to the next node for the attack sequence
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
            sight = GetComponentInChildren<Sight>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign Sight component", nameof(OfficeGhoulBehavior));
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


    }

    private void CreateBehaviorTree()
    {
        //SECTION: BEHAVIOR TREE SETUP
        actions = new BehaviorTree(); //Define behavior tree

        Selector choices = new Selector("Pick Behavior"); //Selector for determining enemy behavior


        Sequence chase = new Sequence("Chase");
        Leaf look = new Leaf("Scan-for-Player", Look);
        Leaf pursue = new Leaf("Pursue-Player", Chase);
        chase.AddChild(look);
        chase.AddChild(pursue);


        //Define the hearing sequence
        Sequence hear = new Sequence("Listen"); //Make the sequence
        Leaf listen = new Leaf("Audio-Input", hearingSequence.Hear); //Leaf 1: Conduct hearing check
        Leaf investigateSound = new Leaf("Investigate-Sound", hearingSequence.InvestigateSound); //Leaf 2: Investigate the origin point of the sound
        Leaf clearSound = new Leaf("Clear-Sound-Location", hearingSequence.DismissSound); //Leaf 3: After reaching the sound, clear the point of origin.
        //Add the leaves to the hear sequence.
        hear.AddChild(listen);
        hear.AddChild(investigateSound);
        hear.AddChild(clearSound);

        Leaf idle = new Leaf("Idling-Behavior", Idle);

        choices.AddChild(chase);
        choices.AddChild(hear);
        //choices.AddChild(idle);

        actions.AddChild(choices);
    }
}
