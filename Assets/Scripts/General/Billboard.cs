using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform playerTrans;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(playerTrans == null)
        {
            playerTrans = GameObject.Find("PlayerBody").GetComponent<Transform>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(playerTrans);
    }
}
