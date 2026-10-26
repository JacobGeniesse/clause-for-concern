using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class StunSequence : MonoBehaviour
{
    private bool stunned; //Is the enemy stunned

    private IEnumerator stunCoroutine = null; //Holding var for the stun coroutine

    [Tooltip("Length of the HR Rep's stun upon dying.")]
    [SerializeField]private float maxStunDuration = 5;

    private EnemyHealth enemyHealth; //Ref to the enemy health
    private NavMeshAgent agent; //Ref to the nav mesh agent


    private void Start()
    {
        //Get ref to enemy health
        try
        {
            enemyHealth = GetComponentInChildren<EnemyHealth>();
        }
        catch
        {
            throw new ArgumentException("Unable to assign EnemyHealth component", nameof(HRRepBehavior));
        }

        //Get reference to the nav mesh agent component
        if (TryGetComponent<NavMeshAgent>(out NavMeshAgent navAgent))
        {
            agent = navAgent;
        }
        else
        {
            //Give an error message if this fails and return false
            Debug.LogError($"No NavMeshAgent attached to {this.gameObject}!");
        }
    }

    //Stun Check Node
    public Node.Status StunCheck()
    {
        //If the enemy is dead
        if (enemyHealth.dead == true)
        {
            //Pass success and stop the enemy from moving
            Debug.Log("Stunned");
            if (agent.isStopped != true)
            {
                agent.isStopped = true;
            }
            return Node.Status.SUCCESS;
        }
        else
        {
            //If not pass failure
            return Node.Status.FAILURE;
        }
    }

    //Node for executing the stun delay
    public Node.Status ExecuteStun()
    {
        //Start coroutine if it doesn't already exist
        if (stunCoroutine == null)
        {
            stunCoroutine = StunDelay();
            StartCoroutine(stunCoroutine);
        }

        //Debug.Log(stunned);

        if (stunned == true)
        {
            return Node.Status.RUNNING;
        }
        else
        {
            //Once coroutine ends clear coroutine holder and pass success
            stunCoroutine = null;
            return Node.Status.SUCCESS;
        }
    }

    //Coroutine for stun delay
    private IEnumerator StunDelay()
    {
        //Is stunned
        stunned = true;
        yield return new WaitForSeconds(maxStunDuration); //Waiting period
        stunned = false; //No longer stunned
    }

    //Node for ending stun
    public Node.Status EndStun()
    {
        Debug.Log("Returning to patrolling routine."); //Debug Message
        agent.isStopped = false; //Free the agent
        enemyHealth.ResetHealth(0); //Reset enemy health
        return Node.Status.SUCCESS; //pass success
    }
}
