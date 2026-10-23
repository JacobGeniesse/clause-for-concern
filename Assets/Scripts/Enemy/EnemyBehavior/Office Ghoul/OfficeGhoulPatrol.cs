using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OfficeGhoulPatrol : MonoBehaviour
{
    private Vector3 originPoint;

    private NavMeshAgent agent;
    private MoveToPoint move;

    [SerializeField] private string originRoomName;
    private Room originRoom;

    private Vector3 wanderPoint = Vector3.zero;
    [SerializeField] private float wanderExtents = 10f;

    private Bounds wanderRange;

    private RenderCheck renderCheck;

    private void Start()
    {
        originPoint = transform.position;

        wanderRange = new Bounds(transform.position, new Vector3(wanderExtents, 1, wanderExtents));

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
    }

    public Node.Status Wander()
    {
        if(wanderPoint == Vector3.zero)
        {
            wanderPoint = new Vector3(UnityEngine.Random.Range(originPoint.x - wanderExtents, originPoint.x + wanderExtents), originPoint.y, UnityEngine.Random.Range(originPoint.z - wanderExtents, originPoint.z + wanderExtents));
        }

        return move.MoveToPOI(agent, wanderPoint);
    }

    public Node.Status ReturnToOrigin()
    {
        if (renderCheck.SightCheck())
        {
            return move.MoveToPOI(agent, originPoint);
        }
        else
        {
            agent.Warp(originPoint);
            return Node.Status.SUCCESS;
        }
    }

}
