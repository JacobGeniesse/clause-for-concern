using UnityEngine;
using UnityEngine.UIElements;

public class HearingSequence : Sequence
{
    //private bool aggressive;

    //private Transform playerTrans;
    //private Transform originPoint;

    //private Vector3 interestingSound;

    //HearingSequence(ref bool agg, ref Transform pt, ref Transform ori)
    //{
    //    aggressive = agg;
    //    playerTrans = pt;
    //    originPoint = ori;

    //    //Define the hearing sequence
    //    Sequence hear = new Sequence("Listen"); //Make the sequence
    //    Leaf listen = new Leaf("Audio-Input", Hear); //Leaf 1: Conduct hearing check
    //    Leaf investigateSound = new Leaf("Investigate-Sound", InvestigateSound); //Leaf 2: Investigate the origin point of the sound
    //    Leaf clearSound = new Leaf("Clear-Sound-Location", DismissSound); //Leaf 3: After reaching the sound, clear the point of origin.
    //    //Add the leaves to the hear sequence.
    //    hear.AddChild(listen);
    //    hear.AddChild(investigateSound);
    //    hear.AddChild(clearSound);

    //}

    //private Node.Status Hear()
    //{
    //    if (aggressive)
    //    {
    //        float distance = DistanceCheck(playerTrans); //Do a distance check between the player and the HR Rep
    //        interestingSound = hearingScript.HearingCheck(distance); //Do a hearing check assigning the interesting sound point if successful
    //        if (interestingSound != Vector3.zero)
    //        {
    //            //If successful proceed to the next node
    //            Debug.Log("I can't see you, but I can hear you!");
    //            return Node.Status.SUCCESS;
    //        }
    //        else
    //        {
    //            //If not fail the node
    //            return Node.Status.FAILURE;
    //        }
    //    }
    //    else
    //    {
    //        return Node.Status.FAILURE;
    //    }

    //}

    //private Node.Status InvestigateSound()
    //{
    //    return move.MoveToPOI(agent, interestingSound); //Move towards the sound of interest
    //}

    //private Node.Status DismissSound()
    //{
    //    hearingScript.clearSound(); //Mark that there is no longer an unexamined sound
    //    interestingSound = Vector3.zero; //Reset the intersting sound
    //    return Node.Status.SUCCESS; //End the sequence
    //}

    //private float DistanceCheck(Transform playerTrans)
    //{
    //    Vector3 playerOffest = playerTrans.position - originPoint.position;
    //    float distanceToPlayer = Vector3.SqrMagnitude(playerOffest);
    //    return distanceToPlayer;
    //}


}
