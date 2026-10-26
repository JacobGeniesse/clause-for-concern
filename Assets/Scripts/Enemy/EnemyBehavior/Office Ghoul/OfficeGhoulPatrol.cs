using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Rendering.CameraUI;

public class OfficeGhoulPatrol : MonoBehaviour
{
    private Vector3 originPoint;

    private NavMeshAgent agent;
    private MoveToPoint move;

    private bool wandering = false;

    [SerializeField] private string originRoomName;
    //private Room originRoom;

    private Vector3 wanderPoint = Vector3.zero;
    private float wanderExtents = 10f;

    private Bounds wanderRange;

    private RenderCheck renderCheck;

    private float minimumWait = 5f;
    private float maximumWait = 8f;

    private bool waiting = false;
    private IEnumerator waitingCoroutine = null;

    private void Start()
    {
        wanderRange = new Bounds(transform.position, new Vector3(wanderExtents, wanderExtents, wanderExtents));

        try
        {
            RoomManager.instance.FindRoom(originRoomName);
        }
        catch
        {
            throw new ArgumentException("Unable to assign originRoom component!", nameof(OfficeGhoulBehavior));
        }

        //Assign the movement script
        try
        {
            move = GetComponentInParent<MoveToPoint>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign MoveToPoint component", nameof(OfficeGhoulBehavior));
        }

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


    public Node.Status Wander()
    {
        if(wanderPoint == Vector3.zero)
        {
            wanderPoint = new Vector3(UnityEngine.Random.Range(originPoint.x - wanderExtents/2, originPoint.x + wanderExtents/2), originPoint.y, UnityEngine.Random.Range(originPoint.z - wanderExtents/2, originPoint.z + wanderExtents/2));
        }

        return move.MoveToPOI(agent, wanderPoint);
    }

    public Node.Status ClearWander()
    {
        if(waitingCoroutine == null)
        {
            waitingCoroutine = RefreshCooldown();
            StartCoroutine(waitingCoroutine);
        }

        if(waiting == false)
        {
            waitingCoroutine = null;
            return Node.Status.SUCCESS;
        }
        else
        {
            return Node.Status.RUNNING;
        }
    }

    private IEnumerator RefreshCooldown()
    {
        waiting = true;
        yield return new WaitForSeconds(UnityEngine.Random.Range(minimumWait, maximumWait));
        wanderPoint = Vector3.zero;
        waiting = false;
    }

    public Node.Status CheckPosition()
    {
        if (( wandering == true && wanderRange.Contains(transform.position)) || (wandering == false && transform.position == originPoint))
        {
            return Node.Status.FAILURE;
        }
        else
        {
            return Node.Status.SUCCESS;
        }
    }


    public Node.Status ReturnToOrigin()
    {
        //To be added once room culling is finished. If the ghoul is not in a room adjacent to the player teleport them back to their origin point
        //if (renderCheck.SightCheck())
        //{
            return move.MoveToPOI(agent, originPoint);
        //}
        //else
        //{
        //    agent.Warp(originPoint);
        //    return Node.Status.SUCCESS;
        //}
    }

    public Node.Status Idle()
    {
        return Node.Status.SUCCESS;
    }

    public void InitializeVars(Vector3 passedOrigin, float wanderBoundry, bool wander)
    {
        wandering = wander;
        originPoint = passedOrigin;
        wanderExtents = wanderBoundry;
    }

}
