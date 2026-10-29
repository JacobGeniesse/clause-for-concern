using UnityEngine;
using UnityEngine.AI;
public class Hearing : MonoBehaviour
{
    [Tooltip("How far away the enemy can hear loud sounds from.")]
    [SerializeField] private float loudRadius = 10f; //Radius that loud sounds can be heard from
    [Tooltip("How far away the enemy can hear the player walking from.")]
    [SerializeField] private float quietRadius = 1f; //Radius that quiet sounds can be heard from

    [HideInInspector] public Vector3 targetSound; //Current sound that is the target of interest

    //private bool unexaminedSound = false; //bool for if there is an unexamined sound

    private PlayerNoiseManager noiseManager; //Player's noise manager

    void Start()
    {
        //assign noiseManager component ref
        noiseManager = GameObject.Find("PlayerBody").GetComponent<PlayerNoiseManager>();
    }

    //Func for performing a hearing check
    public Vector3 HearingCheck(float distance)
    {
        //Loud Noise check
        if (noiseManager.activeLoudNoise == true && distance <= loudRadius)
        {
            targetSound = noiseManager.lastLoudNoise; //Set the important sound's vector3
            //unexaminedSound = true; //Indicate there is sound to investigate
            return targetSound; //Say that there is a noise to hear
        }
        else if (noiseManager.activeQuietNoise == true && distance <= quietRadius)
        {
            //Quiet Noise check
            targetSound = noiseManager.lastQuietNoise;
            //unexaminedSound = true;
            return targetSound;
        }
        //If neither check passes return false
        return Vector3.zero;
    }

    //Func for resetting the unexamined sound var
    public void clearSound()
    {
        //unexaminedSound = false;
    }
}
