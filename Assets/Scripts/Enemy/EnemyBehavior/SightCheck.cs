using UnityEngine;
using System;

public class SightCheck : MonoBehaviour
{
    [SerializeField, Range(0, 180f)] private float visionRadius;
    [SerializeField, Range(0f, 30f)] private float visionRange;

    private Vector3 coneOrigin;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        try
        {
            coneOrigin = transform.position;
        }
        catch
        {
            throw new ArgumentException("Unable to assign vision cone origin!", nameof(SightCheck));
        }
    }

    public bool SeePlayer(Transform target)
    {
        //Do a line cast to see if the player is visible
        if(Physics.Linecast(transform.position, target.position, out RaycastHit hit))
        {
            //First vector is the hit object's position
            Vector3 playerPos = hit.transform.position - transform.position;

            //Second vector is the forward direction of the enemy
            Vector3 forward = transform.forward;

            //Calculate the angle between the forward direction between the two points
            float angleDif = Mathf.Abs(Vector3.SignedAngle(playerPos, forward, Vector3.up));

            //If the object hit is the player, and the angle is within the vision radius, and the hit is within a certain range
            if (hit.collider.tag == "Player" && angleDif <= visionRadius && hit.distance <= visionRange)
            {
                //Return true for the player being seen
                return true;
            }
            else
            {
                //Otherwise return false
                return false;
            }
        }
        else
        {
            return false; //Return false if the linecast fails
        }
    }
}
