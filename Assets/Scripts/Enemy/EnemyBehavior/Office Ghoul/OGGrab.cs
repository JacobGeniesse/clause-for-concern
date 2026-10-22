using UnityEngine;

public class OGGrab : MonoBehaviour
{
    [Tooltip("How far away the enemy can see.")]
    [SerializeField] private float range;

    [Tooltip("How wide the enemy's FOV is.")]
    [SerializeField, Range(0, 180)] private float fovRadius;

    [SerializeField, HideInInspector] private bool detection; //Bool for if the enemy has spotted the target

    private int layerMask;

    private void Start()
    {
        layerMask = 1 << 2;
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
                detection = true;
                return detection;
            }
            else
            {
                //Otherwise return false
                detection = false;
                return detection;
            }
        }
        else
        {
            detection = false;
            return false; //Return false if the linecast fails
        }
    }




}
