using UnityEngine;

public class EnemySound : MonoBehaviour
{
    //Sound vars
    private float stepDelay;
    private float maxDelay = 0;
    [SerializeField] private float[] stepDelayType = new float[3];
    private float maxStepDelay;
    [SerializeField] private AudioSource soundGen;
    [SerializeField] private AudioClip step;

    private Transform playerTrans;

    [SerializeField] private float[] audioRanges = new float[3];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTrans = GameObject.Find("PlayerBody").GetComponent<Transform>();

        maxStepDelay = stepDelayType[0];
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 playerOffest = playerTrans.position - transform.position;
        float distanceToPlayer = Vector3.SqrMagnitude(playerOffest);

        if(distanceToPlayer <= audioRanges[0])
        {
            soundGen.volume = 1;
        }
        else if(distanceToPlayer <= audioRanges[1] )
        {
            soundGen.volume = 0.5f;
        }
        else
        {
            soundGen.volume = 0;
        }



        if (stepDelay <= 0)
        {
            stepDelay = maxStepDelay;
            soundGen.clip = step;
            soundGen.Play();
        }
        else
        {
            stepDelay -= Time.deltaTime;
        }

    }

    public void SetSound(int type)
    {
        maxDelay = stepDelayType[type];
    }
}
