using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Rendering.CameraUI;

public class OfficeGhoulPatrol : MonoBehaviour
{
    private Vector3 originPoint; //Point where the office ghoul should be
    private NavMeshAgent agent; //Reference to the nav mesh agent
    private MoveToPoint move; //Reference to the movetopoint script
    private bool wandering = false; //Does this enemy wander?

    //Variables related to the enemy's room of origin
    //[SerializeField] private string originRoomName;
    //private Room originRoom;

    private Vector3 wanderPoint = Vector3.zero; //point the enemy wanders towards
    private float wanderExtents = 10f; //How far can the enemy travel
    private Bounds wanderRange; //Physcial range the enemy wanders in

    [Tooltip("Minimum amount of time before the enemy starts moving again after wandering.")]
    [SerializeField] private float minimumWait = 5f;

    [Tooltip("Maximum amount of time before the enemy starts moving again after wandering.")]
    [SerializeField]private float maximumWait = 8f;

    private bool waiting = false; //Is the enemy mid cooldown right now?
    private IEnumerator waitingCoroutine = null; //holding var for the waiting coroutine

    private RenderCheck renderCheck; //Helper script for determining if the enemy is visible

    private void Start()
    {
        //Create the range for the enemy to wander in
        wanderRange = new Bounds(transform.position, new Vector3(wanderExtents, wanderExtents, wanderExtents));

        /* //To be implemented once room culling is finished
        try
        {
            RoomManager.instance.FindRoom(originRoomName);
        }
        catch
        {
            throw new ArgumentException("Unable to assign originRoom component!", nameof(OfficeGhoulBehavior));
        }
        */

        //Assign the movement script
        try
        {
            move = GetComponentInParent<MoveToPoint>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign MoveToPoint component", nameof(OfficeGhoulBehavior));
        }

        //Assign the render check script
        try
        {
            renderCheck = GetComponent<RenderCheck>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign RenderCheck component", nameof(OfficeGhoulBehavior));
        }

        //Assign the nav mesh agent script
        try
        {
            agent = GetComponentInParent<NavMeshAgent>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign NavMeshAgent script.", nameof(OfficeGhoulBehavior));
        }
    }

    //Node to check if the enemy should wander
    public Node.Status WanderCheck()
    {
        if(wandering == true)
        {
            return Node.Status.SUCCESS;
        }
        else
        {
            return Node.Status.FAILURE;
        }
    }

    //Node for conducting a wander
    public Node.Status Wander()
    {
        if(wanderPoint == Vector3.zero)
        {
            wanderPoint = new Vector3(UnityEngine.Random.Range(originPoint.x - wanderExtents/2, originPoint.x + wanderExtents/2), originPoint.y, UnityEngine.Random.Range(originPoint.z - wanderExtents/2, originPoint.z + wanderExtents/2));
        }

        return move.MoveToPOI(agent, wanderPoint);
    }

    //Clearing the previous wandering point after waiting
    public Node.Status ClearWander()
    {
        //If the coroutine isn't currently running start running it
        if(waitingCoroutine == null)
        {
            waitingCoroutine = RefreshCooldown();
            StartCoroutine(waitingCoroutine);
        }

        if(waiting == false)
        {
            //If the coroutine has finished clear the coroutine and succeed the func
            waitingCoroutine = null;
            return Node.Status.SUCCESS;
        }
        else
        {
            //Otherwise keep on this node
            return Node.Status.RUNNING;
        }
    }

    //Coroutine for refreshing the cooldown
    private IEnumerator RefreshCooldown()
    {
        waiting = true; //set waiting to true
        yield return new WaitForSeconds(UnityEngine.Random.Range(minimumWait, maximumWait)); //Pick a random length to wait for
        wanderPoint = Vector3.zero; //Reset the wandering point
        waiting = false; //Stop waiting
    }

    //Check if the enemy should retreat
    public Node.Status CheckPosition()
    {
        //If the enemy is a wanderer, check if it is beyond its range. If it is an idle enemy check if it isn't at its origin
        if (( wandering == true && wanderRange.Contains(transform.position)) || (wandering == false && transform.position == originPoint))
        {
            return Node.Status.FAILURE; //If it is within where it should be pass failure
        }
        else
        {
            return Node.Status.SUCCESS; //If not start the retreat coroutine
        }
    }

    //Node for returning to the origin point
    public Node.Status ReturnToOrigin()
    {
        //To be added once room culling is finished. If the ghoul is not in a room adjacent to the player teleport them back to their origin point
        //if (renderCheck.SightCheck())
        //{
            return move.MoveToPOI(agent, originPoint); //Walk back to the origin point
        //}
        //else
        //{
        //    agent.Warp(originPoint);
        //    return Node.Status.SUCCESS;
        //}
    }

    //Node for idling
    public Node.Status Idle()
    {
        return Node.Status.SUCCESS;
    }

    //Func used by the main script to initialize variables in here
    public void InitializeVars(Vector3 passedOrigin, float wanderBoundry, bool wander)
    {
        wandering = wander;
        originPoint = passedOrigin;
        wanderExtents = wanderBoundry;
    }

}
