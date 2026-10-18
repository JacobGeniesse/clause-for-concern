using System;
using UnityEngine;

public class CullRooms : MonoBehaviour
{
    private Transform playerTrans;

    [SerializeField] private float cullDistance;
    [SerializeField] private GameObject objectToHide;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTrans = GameObject.Find("PlayerBody").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerOffest = playerTrans.position - transform.position;
        float distanceToPlayer = Vector3.SqrMagnitude(playerOffest);
        
        if(distanceToPlayer >= cullDistance )
        {
            if(objectToHide.activeSelf == true)
            {
                objectToHide.SetActive(false);
            }
        }
        else
        {
            if (objectToHide.activeSelf == false)
            {
                objectToHide.SetActive(true);
            }
        }
    }
}
