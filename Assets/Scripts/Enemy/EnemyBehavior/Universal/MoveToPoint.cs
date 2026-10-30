using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MoveToPoint : MonoBehaviour
{
    [Tooltip("How close the enemy needs to be to a patrol point to clear it")]
    [SerializeField] private float clearDistance = 7; //Var for the distance at which the AI can clear a patrol point

    [Tooltip("Walking sound effect for this enemy")]
    [SerializeField] private AudioClip walkingSound;
    private IEnumerator sfxCoroutine = null; //Coroutine holder for the sfxCoroutine

    //Helper Func for Moving to a point of interest
    public Node.Status MoveToPOI(NavMeshAgent agent, Vector3 destination)
    {
        //If the agent isn't on the nav mesh fail the pass immediately
        if (!agent.isOnNavMesh)
        {
            Debug.LogError($"{this.gameObject} is not on a nav mesh!");
            return Node.Status.FAILURE;
        }

        if (agent.destination != destination)
        {
            agent.SetDestination(destination); //Set destination to the POI
        }

        if(sfxCoroutine == null)
        {
            sfxCoroutine = step(1f);
            StartCoroutine(sfxCoroutine);
        }

        //Calculate how far away the enemy is from the POI
        Vector3 offsetPOI = destination - transform.position;

        float distanceToNode = Vector3.SqrMagnitude(offsetPOI);

        if (distanceToNode > clearDistance)
        {
            //If the node is farther from the clear distance, keep the tree on this node
            return Node.Status.RUNNING;
        }
        else if (distanceToNode <= clearDistance)
        {
            //If not pass success
            return Node.Status.SUCCESS;
        }
        return Node.Status.FAILURE; //If something breaks fail the func
    }

    //Coroutine for playing a step sound effect on an interval
    private IEnumerator step(float stepInterval)
    {
        AudioManager.Instance.Play3DSFX(walkingSound, transform.position, 0.5f); //Play a sound using the audio manager
        yield return new WaitForSeconds(stepInterval); //Wait
        sfxCoroutine = null; //Free up the coroutine to play the sound again
    }

}
