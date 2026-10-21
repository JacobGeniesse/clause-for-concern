using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class StunSequence : MonoBehaviour
{
    private bool stunned;

    private NavMeshAgent agent;

    private IEnumerator stunCoroutine = null;

    [Tooltip("Length of the HR Rep's stun upon dying.")]
    [SerializeField]private float maxStunDuration = 5;

    private EnemyHealth enemyHealth;

    private void Start()
    {
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

    public Node.Status StunCheck()
    {
        if (enemyHealth.dead == true)
        {
            Debug.Log("Stunned");
            if (agent.isStopped != true)
            {
                agent.isStopped = true;
            }
            return Node.Status.SUCCESS;
        }
        else
        {
            return Node.Status.FAILURE;
        }
    }

    public Node.Status ExecuteStun()
    {
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
            stunCoroutine = null;
            return Node.Status.SUCCESS;
        }
    }

    private IEnumerator StunDelay()
    {
        stunned = true;
        yield return new WaitForSeconds(maxStunDuration);
        stunned = false;
    }

    public Node.Status EndStun()
    {
        Debug.Log("Returning to patrolling routine.");
        agent.isStopped = false;
        enemyHealth.ResetHealth(0);
        return Node.Status.SUCCESS;
    }
}
