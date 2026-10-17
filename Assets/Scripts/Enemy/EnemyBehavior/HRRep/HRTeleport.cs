using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class HRTeleport : MonoBehaviour
{
    [SerializeField, HideInInspector] private List<Vector3> warpPoints; //List of points that the enemy can teleport to.
    private Transform playerTrans; //Reference to the player's position

    [Tooltip("The maximum distance that the player can be away from a warp point for that warp point to be able to be warped to.")]
    [SerializeField] private float minDistance = 25;

    [Tooltip("The maximum distance that the player can be away from a warp point for that warp point to be able to be warped to.")]
    [SerializeField] private float maxDistance = 150;
    private NavMeshAgent agent; //reference to the nav mesh agent

    private Vector3 chosenNode = Vector3.zero;

    private void Start()
    {
        //Assign the variables

        //Agent assignment
        try
        {
            agent = GetComponent<NavMeshAgent>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign NavMeshAgent component!", nameof(HearingSequence));
        }

        //Player reference assignment
        try
        {
            //Get the player body component
            playerTrans = GameObject.Find("PlayerBody").GetComponent<Transform>();
        }
        catch
        {
            throw new ArgumentException("Unable to find PlayerBody component!", nameof(HearingSequence));
        }
    }

    //Function for adding in the warp points for the enemy to teleport to
    public void AddWarps(List<Vector3> nodes)
    {
        //Debug.Log(nodes.Count); //Debug func for determine the number of nodes for the enemy to warp to
        try
        {
            warpPoints.AddRange(nodes); //Try to add the nodes passed by HRRepBehavior
        }
        catch
        {
            throw new ArgumentException("Unable to assign warp nodes!", nameof(HRTeleport));
        }
    }

    //Helper function for picking a node to teleport to
    public Vector3 ChooseNode()
    {
        float closenessScore = 9999; //Base closeness score, used if no point is assigned
        Vector3 currentNode = Vector3.zero; //Point to be ignored if passed

        //For each warp point, calculate their closeness score and then pick a point to warp to
        for(int i = 0; i < warpPoints.Count; i++)
        {
            float localClose = 0; //this node's closeness score
            float disToPlay = DistanceCheck(playerTrans, warpPoints[i]); //Calc the distance to the player
            
            //If the distance to the player is within the range to be warped to, calculate the closeness score for it
            if (disToPlay > minDistance && disToPlay < maxDistance)
            {
                localClose += disToPlay;
            }
            else
            {
                //If not, set the value to max voiding it from the running.
                localClose = closenessScore;
            }

            //If there is a new warp node to assign update the closness score and currentNode for future use
            if (localClose < closenessScore)
            {
                currentNode = warpPoints[i];
                closenessScore = localClose;
            }
        }

        return currentNode; //Return the current node
    }

    //Function for checking if the enemy should warp
    public Node.Status WarpCheck()
    {
        chosenNode = ChooseNode(); //Pick a node
        //If a valid node is returned, pass success. If not pass failure
        if (chosenNode != Vector3.zero)
        {
            return Node.Status.SUCCESS;
        }
        else
        {
            return Node.Status.FAILURE;
        }
    }

    //Function for executing a teleport
    public Node.Status Teleport()
    {
        //If the chosen node is not zero, warp to that node
        if(chosenNode != Vector3.zero)
        {
            agent.Warp(chosenNode); //Warp to the node
            chosenNode = Vector3.zero; //Reset the chosenNode to prevent infinite teleporting
            return Node.Status.SUCCESS;
        }
        else
        {
            return Node.Status.FAILURE;
        }
    }

    //Helper func for performing a distance check
    private float DistanceCheck(Transform playerTrans, Vector3 nodePos)
    {
        Vector3 playerOffest = playerTrans.position - nodePos;
        float distanceToPlayer = Vector3.SqrMagnitude(playerOffest);
        return distanceToPlayer;
    }
}
