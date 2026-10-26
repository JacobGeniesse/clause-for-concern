using UnityEngine;

public class RoomTracker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
        {
        }
    }
}
