using System;
using UnityEngine;

public class Hearing : MonoBehaviour
{
    [SerializeField] private float lSoundRadius = 10f; //Radius that loud sounds can be heard from
    [SerializeField] private float qSoundRadius = 1f; //Radius that quiet sounds can be heard from
    [HideInInspector] public Vector3 importantSound; //Current Sound to seek out
    [HideInInspector] public bool unexaminedSound = false;
    private PlayerNoiseManager noiseManager; //Player's noise manager

    void Start()
    {
        //assign noiseManager component ref
        noiseManager = GameObject.Find("PlayerBody").GetComponent<PlayerNoiseManager>();   
    }

    //Func for performing a hearing check
    public bool HearingCheck(float playerDistance)
    {
        //Loud Noise check
        if(noiseManager.activeLoudNoise == true && playerDistance <= lSoundRadius)
        {
            importantSound = noiseManager.lastLoudNoise;
            unexaminedSound = true;
            return true;
        }
        else if(noiseManager.activeQuietNoise == true && playerDistance <= qSoundRadius)
        {
            //Quiet Noise check
            importantSound = noiseManager.lastQuietNoise;
            unexaminedSound = true;
            return true;
        }
        //If neither check passes return false
        return false;
    }
}
