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

    //[SerializeField] private bool aggressive = true; //Determines if the enemy is aggressive currently unused.

    #region hearing Vars
    private Vector3 interestingSound; //Sound for the enemy to investigate

    #endregion

    #region chase Vars
    private Vector3 lastKnownLocation; //Last spot the enemy spotted the player at

    private float attackDistance = 10f;

    #endregion

    private Transform playerTrans; //Point representing the player's position

    #region Helper Scripts
    private TraverseNodes patrolScript; //Helper script for patrolling between points
    private Sight sightScript; //Helper script for vision checks
    private Hearing hearingScript; //Helper script for hearing checks
    private MoveToPoint move; //Helper script for moving to a point
    #endregion


    void Start()
    {
        //Create the HR Rep's Behavior Tree
        CreateBehaviorTree();

        //Assign the necessary script references
        AssignReference();

        //Add patrols
        AddPatrols();
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
    }

    #region Chase Sequence Leaves
    private Node.Status Look()
    {
        //Do a sight check, if the player is spotted, move towards them
        if (sightScript.SightCheck(playerTrans))
        {
            lastKnownLocation = playerTrans.position; //Update the last known location
            Debug.Log("I see you!"); //Debug Message
            move.MoveToPOI(agent, playerTrans.position);
            if(DistanceCheck(playerTrans) <= attackDistance)
            {
                return Node.Status.SUCCESS;
            }
            else
            {
                return Node.Status.RUNNING;
            }
        }
        else
        {
            return Node.Status.FAILURE; //Otherwise, return failure
        }
    }

    private Node.Status Attack()
    {
        Debug.Log("Get a load of this!");
        //Attack Animation
        //if(Physics.Linecast(transform.position, playerTrans.position, out RaycastHit hit))
        //{
        //    if(hit.collider.gameObject.TryGetComponent<PlayerHealth>(out PlayerHealth healthComponent))
        //    {
        //        Debug.Log("Take this!");
        //    }
        //}
        return Node.Status.SUCCESS;
    }

    #endregion

    #region Lose Player Leaves
    private Node.Status LosePlayer()
    {
        //If there is a last known location, move towards it
        if (lastKnownLocation != Vector3.zero)
        {
            Debug.Log("I can't see or hear you. I will investigate where I saw you last");
            return move.MoveToPOI(agent, lastKnownLocation);
        }
        else
        {
            //If not fail the func
            return Node.Status.FAILURE;
        }
    }

    private Node.Status ResetChase()
    {
        //Reset the lastKnownLocation
        lastKnownLocation = Vector3.zero;
        return Node.Status.SUCCESS; //End the Sequence
    }
    #endregion

    #region Hear Sequence Leaves
    private Node.Status Hear()
    {
        float distance = DistanceCheck(playerTrans); //Do a distance check between the player and the HR Rep
        interestingSound = hearingScript.HearingCheck(distance); //Do a hearing check assigning the interesting sound point if successful
        if (interestingSound != Vector3.zero)
        {
            //If successful proceed to the next node
            Debug.Log("I can't see you, but I can hear you!");
            return Node.Status.SUCCESS;
        }
        else
        {
            //If not fail the node
            return Node.Status.FAILURE;
        }
    }

    private Node.Status InvestigateSound()
    {
        return move.MoveToPOI(agent, interestingSound); //Move towards the sound of interest
    }

    private Node.Status DismissSound()
    {
        hearingScript.clearSound(); //Mark that there is no longer an unexamined sound
        interestingSound = Vector3.zero; //Reset the intersting sound
        return Node.Status.SUCCESS; //End the sequence
    }

    #endregion

    #region Patrol Sequence Leaves
    private Node.Status Wander()
    {
        return patrolScript.Patrol(agent); //Return the result of the Patrol function.
    } 

    private Node.Status ResetPatrol()
    {
        Debug.Log("Reset"); //Debug Message indicating a reset
        patrolScript.IncrementPatrol(); //Move to the next patrol point
        return Node.Status.SUCCESS; //End the sequence
    }
    #endregion

    private float DistanceCheck(Transform playerTrans)
    {
        Vector3 playerOffest = playerTrans.position - transform.position;
        float distanceToPlayer = Vector3.SqrMagnitude(playerOffest);
        return distanceToPlayer;
    }


    #region Start Helpers
    //Function for adding the patrol points for the TraverseNodes script
    void AddPatrols()
    {
        //Grab all node game objects
        List<GameObject> fillerList = GameObject.FindGameObjectsWithTag("Patrol Node").ToList();

        //If there are patrol points in filler list
        if(fillerList.Count > 0)
        {
            //Convert all of the fillerList GameObjects into a transform point
            List<Vector3> conversionList = new List<Vector3>();

            foreach (GameObject GM in fillerList)
            {
                conversionList.Add(GM.transform.position);
            }

            //Assign the patrol points in conversionList to the patrol script
            if (patrolScript != null)
            {
                patrolScript.AssignPatrolPoints(conversionList);
            }
            else
            {
                Debug.LogError("Patrol Script not assigned!");
            }
        }
        else
        {
            Debug.LogError("No patrol nodes found!");
        }
    }

    //Function to calculate the distance away from the player using SqrMagnitude for slightly better performance

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

        //Get the Hearing component
        try
        {
            hearingScript = GetComponentInChildren<Hearing>();
        }
        catch
        {
            //Give an error if this fails and return false
            Debug.LogError($"No Hearing script attached to {this.gameObject}");
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

        //Define the patrol sequence
        Sequence patrol = new Sequence("Wander-Map"); //Create sequence for patrolling
        Leaf wander = new Leaf("Sensory-Input", Wander); //Leaf 1: Pick a node and travel to it
        Leaf clearPatrol = new Leaf("Clear-Patrol-Points", ResetPatrol); //Leaf 2: Clear the node and prep for the next one
        //Add leaves to the sequence
        patrol.AddChild(wander);
        patrol.AddChild(clearPatrol);


        //Define the chase sequence
        Sequence chase = new Sequence("Chase-Player"); //Make the sequence
        Leaf look = new Leaf("Sight-Check", Look); //Leaf 1: Conduct a sight check and if it passes, go to that point
        Leaf attack = new Leaf("Beat-Them-Up", Attack);
        //Add leaves to the sequence
        chase.AddChild(look);
        chase.AddChild(attack);
        

        //Define the losePlayer sequence
        Sequence losePlayer = new Sequence("Lose-Player"); //Make the sequence
        Leaf lose = new Leaf("Investigate-Last-Known-Location", LosePlayer); //Leaf 1: Investigate the last known location if it isn't 0
        Leaf endChase = new Leaf("End-Chase", ResetChase); //Leaf 2: Clear the last-known-location and prepare to start running through nodes again
        //Add the leaves to the sequence
        losePlayer.AddChild(lose);
        losePlayer.AddChild(endChase);

        //Define the hearing sequence
        Sequence hear = new Sequence("Listen"); //Make the sequence
        Leaf listen = new Leaf("Audio-Input", Hear); //Leaf 1: Conduct hearing check
        Leaf investigateSound = new Leaf("Investigate-Sound", InvestigateSound); //Leaf 2: Investigate the origin point of the sound
        Leaf clearSound = new Leaf("Clear-Sound-Location", DismissSound); //Leaf 3: After reaching the sound, clear the point of origin.
        //Add the leaves to the hear sequence.
        hear.AddChild(listen);
        hear.AddChild(investigateSound);
        hear.AddChild(clearSound);


        //Add the different behavior routes to the selector
        sensoryInput.AddChild(chase);
        sensoryInput.AddChild(hear);
        sensoryInput.AddChild(losePlayer);
        sensoryInput.AddChild(patrol);


        actions.AddChild(sensoryInput); //Add sensory input to the master behavior tree
    }
    #endregion
}