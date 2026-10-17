using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;

public class HearingSequence : MonoBehaviour
{
    private Vector3 interestingSound = Vector3.zero; //Sound for the enemy to investigate

    private Transform playerTrans; //reference to the player's transform

    private Hearing hearingScript; //Reference to the hearing script

    private NavMeshAgent agent; //Reference to the nav mesh agent of the enemy

    private MoveToPoint move; //Reference to the MoveToPoint script

    private void Start()
    {
        //Assign the various components

        //Assign navmeshagnet
        try
        {
            agent = GetComponentInParent<NavMeshAgent>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign NavMeshAgent component!", nameof(HearingSequence));
        }

        //Assign player transfrom
        try
        {
            //Get the player body component
            playerTrans = GameObject.Find("PlayerBody").GetComponent<Transform>();
        }
        catch
        {
            throw new ArgumentException("Unable to find PlayerBody component!", nameof(HearingSequence));
        }

        //Assign hearing script
        try
        {
            hearingScript = GetComponent<Hearing>();
        }
        catch
        {
            throw new ArgumentException("Unable to find Hearing component!", nameof(HearingSequence));
        }

        //Assign the MoveToPoint component
        try
        {
            move = GetComponentInParent<MoveToPoint>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign MoveToPoint component.", nameof(HearingSequence));
        }
    }

    //Node for a hearing check
    public Node.Status Hear()
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

    //Node for investigating an active sound
    public Node.Status InvestigateSound()
    {
        return move.MoveToPOI(agent, interestingSound); //Move towards the sound of interest
    }

    //Node for clearing an investigation point
    public Node.Status DismissSound()
    {
        //hearingScript.clearSound(); //Mark that there is no longer an unexamined sound
        interestingSound = Vector3.zero; //Reset the intersting sound
        return Node.Status.SUCCESS; //End the sequence
    }

    //Helper Func for calculating a distance check
    private float DistanceCheck(Transform playerTrans)
    {
        Vector3 playerOffest = playerTrans.position - transform.position;
        float distanceToPlayer = Vector3.SqrMagnitude(playerOffest);
        return distanceToPlayer;
    }
}
