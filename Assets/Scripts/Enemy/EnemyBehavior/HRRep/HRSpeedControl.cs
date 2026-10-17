using UnityEngine;
using UnityEngine.AI;
using System;

public class HRSpeedControl : MonoBehaviour
{
    [Tooltip("Speed values for the different enemy speed states."), SerializeField] private float[] moveSpeeds = 
    {
        3.5f,
        7f
    };

    [Tooltip("Acceleration values for the different enemy speed states."), SerializeField]
    private float[] accelerations =
    {
        120,
        240
    };

    private NavMeshAgent agent;

    private void Start()
    {
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

    //Func for changing the speed of the navmeshagent
    public void ChangeAgentSpeed(int speedType)
    {
        try
        {
            agent.speed = moveSpeeds[speedType]; //Set agent speed
            agent.acceleration = accelerations[speedType]; //Update acceleration to match
        }
        catch
        {
            throw new ArgumentException("Invalid Speed Type Entered!", nameof(HRSpeedControl));
        }
    }

    //Func for checking if the agent's speed should be changed
    public bool ShouldChangeSpeed(int speedType)
    {
        //Check if the desired new speed is already set
        if (agent.speed != moveSpeeds[speedType])
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
