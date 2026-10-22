using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class OfficeGhoulBehavior : MonoBehaviour
{
    private BehaviorTree actions;
    private Node.Status treeStatus = Node.Status.RUNNING;

    private Transform playerTrans;

    [SerializeField] private Collider attackCol;

    #region Attacking Scripts
    private bool attacking = false;
    private IEnumerator attackingCoroutine = null;
    #endregion

    private NavMeshAgent agent;
    private MoveToPoint move;
    private Sight sight;
    private PlayerMovement playerMove;
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

    private Node.Status Attack()
    {
        if (attackingCoroutine == null)
        {
            Debug.Log("Get a load of this!"); //Debug message showing that the attack worked
            attackingCoroutine = AttackCoroutine(); //Set the active grab attack
            StartCoroutine(attackingCoroutine); //Start the grab attack coroutine
        }

        //Check for if the attack sequence is still happening
        if (attacking == false)
        {
            attackingCoroutine = null;
            return Node.Status.SUCCESS; //After the attack is done, end the sequence
        }
        else
        {
            return Node.Status.RUNNING; //If the attack is still underway, keep running this node
        }

    }

    private IEnumerator AttackCoroutine()
    {
        //Debug.Log("Violence");
        agent.destination = transform.position;
        attacking = true;

        playerTrans.parent = attackCol.transform;
        playerTrans.position = attackCol.transform.position;

        playerMove.AlterMoveState(false);

        yield return new WaitForSeconds(1.5f);
        playerTrans.parent = null;
        playerMove.AlterMoveState(true);

        yield return new WaitForSeconds(1.5f);
        attacking = false;
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

        try
        {
            playerMove = playerTrans.gameObject.GetComponent<PlayerMovement>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign PlayerMovement component", nameof(OfficeGhoulBehavior));
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
        Leaf look = new Leaf("Scan-for-Player", Look);
        Leaf attack = new Leaf("Attack-Player", Attack);
        //Add leaves to the sequence
        chase.AddChild(look);
        chase.AddChild(attack);


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

        choices.AddChild(stun);
        choices.AddChild(chase);
        choices.AddChild(hear);
        //choices.AddChild(idle);

        actions.AddChild(choices);
    }
}
