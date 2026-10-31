using UnityEngine;

public class OGGrab : MonoBehaviour
{
    [Tooltip("How far away the player can be before the enemy starts a lunge.")]
    [SerializeField] private float range;

    [Tooltip("How wide the FOV is for starting a lunge.")]
    [SerializeField, Range(0, 180)] private float fovRadius;

    [SerializeField] private bool showGuides; //Bool for if the enemy has spotted the target

    private int layerMask; //Layermask to ignore the attackbox's collider

    private void Start()
    {
        layerMask = 1 << 2; //Set up layermask
    }

    public bool GrabCheck(Transform target)
    {
        //Do a line cast to see if the player is visible
        if (Physics.Raycast(transform.position, target.position - transform.position, out RaycastHit hit, range, ~layerMask))
        {
            //First vector is the hit object's position
            Vector3 playerPos = hit.transform.position - transform.position;

            //Second vector is the forward direction of the enemy
            Vector3 forward = transform.forward;

            //Calculate the angle between the forward direction between the two points
            float angleDif = Mathf.Abs(Vector3.SignedAngle(playerPos, forward, Vector3.up));

            //If the object hit is the player, and the angle is within the vision radius, and the hit is within a certain range
            if (hit.collider.tag == "Player" && angleDif <= fovRadius)
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
