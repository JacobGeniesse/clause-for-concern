using System.Collections.Generic;
using System;
using UnityEngine;

public class HRTeleport : MonoBehaviour
{
    [SerializeField] private List<Vector3> warpPoints;
    private Transform playerTrans;

    [SerializeField] private float minDistance = 25;
    [SerializeField] private float maxDistance = 150;

    public void AddWarps(List<Vector3> nodes)
    {
        Debug.Log(nodes.Count);
        try
        {
            warpPoints.AddRange(nodes);
        }
        catch
        {
            throw new ArgumentException("Unable to assign warp nodes!", nameof(HRTeleport));
        }
    }

    public Vector3 ChooseNode()
    {
        float closenessScore = 9999;
        Vector3 currentNode = Vector3.zero;

        for(int i = 0; i < warpPoints.Count; i++)
        {
            float localClose = 0;
            float disToPlay = DistanceCheck(playerTrans);
            if (disToPlay > minDistance && disToPlay < maxDistance)
            {
                localClose += DistanceCheck(playerTrans);
            }
            else
            {
                localClose = 9999;
            }

            if (localClose < closenessScore)
            {
                currentNode = warpPoints[i];
                closenessScore = localClose;
            }
        }

        return currentNode;
    }

    public Node.Status Teleport()
    {
        Vector3 chosenNode = ChooseNode();

        if(chosenNode != Vector3.zero)
        {
            transform.position = chosenNode;
            return Node.Status.SUCCESS;
        }
        else
        {
            return Node.Status.FAILURE;
        }
    }

    private float DistanceCheck(Transform playerTrans)
    {
        Vector3 playerOffest = playerTrans.position - transform.position;
        float distanceToPlayer = Vector3.SqrMagnitude(playerOffest);
        return distanceToPlayer;
    }
}
