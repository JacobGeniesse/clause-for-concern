using UnityEngine;

public class PlayerNoiseManager : MonoBehaviour
{
    [Tooltip("How long sounds are audible to the enemies for.")]
    [SerializeField, Range(0, 3f)] private float maxNoiseTimer = 0.1f; //Max time noises are audible for
    [HideInInspector] public Vector3 lastLoudNoise; //storage for the location of the last loud sound
    [HideInInspector] public Vector3 lastQuietNoise; //storage for the location of the last quiet sound
    private float loudNoiseExpiration = 0; //timer float for loud noises
    private float quietNoiseExpiration = 0; //timer float for quiet noises

     public bool activeLoudNoise = false; //Is there an active loud sound?
     public bool activeQuietNoise = false; //Is there an active quiet sound?
    
    void Update()
    {
        //If the counters are above zero decrease them and say there is an active loud sound
        if(loudNoiseExpiration > 0)
        {
            loudNoiseExpiration -= Time.deltaTime;
            activeLoudNoise = true;
        }
        else
        {
            //If the counter is below zero stop counting down and say there isn't a loud sound active
            activeLoudNoise = false;
        }

        //Same thing as the group but for quiet sounds
        if (quietNoiseExpiration > 0)
        {
            quietNoiseExpiration -= Time.deltaTime;
            activeQuietNoise = true;
        }
        else
        {
            activeQuietNoise = false;
        }
    }

    //Function for updating the current loud noise
    public void UpdateLoudNoise(Vector3 noisePos)
    {
        lastLoudNoise = noisePos;
        loudNoiseExpiration = maxNoiseTimer;
    }

    //Function for updating the current quiet noise
    public void UpdateQuietNoise(Vector3 noisePos)
    {
        lastQuietNoise = noisePos;
        quietNoiseExpiration = maxNoiseTimer;
    }
}
