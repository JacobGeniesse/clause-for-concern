using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms.Impl;

public class HRRepBehavior : MonoBehaviour
{
    private BehaviorTree actions;
    Node.Status treeStatus = Node.Status.RUNNING;

    private TraverseNodes patrolScript;

    void Start()
    {
        //Behavior tree definintion
        actions = new BehaviorTree();

        Sequence patrol = new Sequence("Wander-Map");

        Leaf wander = new Leaf("Look-For-Player", Wander);
        Leaf clearPatrol = new Leaf("Clear-Patrol-Points", ResetPatrol);

        actions.AddChild(patrol);
        patrol.AddChild(wander);
        patrol.AddChild(clearPatrol);

        if (TryGetComponent<TraverseNodes>(out TraverseNodes nodeScript))
        {
            patrolScript = nodeScript;
        }
        else
        {
            Debug.LogError($"No TraverseNodes script attached to {this.gameObject}!");
        }

        AddPatrols();
    }

    private void Update()
    {
        //If the treeStatus is not successful then set it equal to the process active under actions
        if (treeStatus != Node.Status.SUCCESS)
        {
            treeStatus = actions.Process();
        }

        //If treeStatus is successful then set it back to running so the process can continue
        if (treeStatus == Node.Status.SUCCESS)
        {
            treeStatus = Node.Status.RUNNING;
        }
    }

    private Node.Status Wander()
    {
        return Node.Status.SUCCESS;
    } 

    private Node.Status ResetPatrol()
    {
        patrolScript.IncrementPatrol();
        return Node.Status.SUCCESS;
    }

    void AddPatrols()
    {
        List<GameObject> fillerList = GameObject.FindGameObjectsWithTag("Patrol Node").ToList();

        if(fillerList.Count > 0)
        {
            List<Vector3> conversionList = new List<Vector3>();

            foreach (GameObject GM in fillerList)
            {
                conversionList.Add(GM.transform.position);
            }

            if (patrolScript != null)
            {
                patrolScript.AssignPatrolPoints(conversionList);
            }
            else
            {
                Debug.LogError("Patrol Script not assigned!");
            }
        }
        else
        {
            Debug.LogError("No patrol nodes found!");
        }

    }
}
