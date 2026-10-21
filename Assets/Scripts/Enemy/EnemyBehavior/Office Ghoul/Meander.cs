using UnityEngine;

public class Meander : MonoBehaviour
{
    private Vector3 originPoint;
    private Bounds meanderRange;
    private Vector3 wanderRange;

    private float meanderCooldown;

    private Transform playerTrans;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originPoint = transform.position;
        meanderRange = new Bounds(originPoint, wanderRange);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
