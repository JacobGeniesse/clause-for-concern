using UnityEngine;
using System.Collections.Generic;

/*
 * Scriptable object for handling different versions of enemy voiceline sets
 * Built this way for ease of testing and showcasing different versions of character voicelines.
*/
[CreateAssetMenu(fileName = "Voiceline", menuName = "Scriptable Objects/Voiceline")]
public class Voicelines : ScriptableObject
{
    [HideInInspector] public string[] categories = new string[]
    {
        "takeDamage",
        "stunStart",
        "stunIdle",
        "stunRecover",
        "startChase",
        "activeChase",
        "loseChase",
        "attackAttempt",
        "attackSuccess",
        "attackFailure"
    };



    [Tooltip("Voice lines for when the enemy takes damage.")]
    public AudioClip[] takeDamage;
    //private int damagePriority = 0;


    [Tooltip("Voice lines for when the enemy gets stunned.")]
    public AudioClip[] stunStart;
    //private int stunSPiority = 0;


    [Tooltip("Voice lines for when the enemy is actively stunned.")]
    public AudioClip[] stunIdle;
    //private int stunIPriority = 0;

    [Tooltip("Voice lines for when the enemy recovers from being stunned.")]
    public AudioClip[] stunRecover;

    [Tooltip("Voice lines for when the enemy begins chasing the player.")]
    public AudioClip[] startChase;

    [Tooltip("Voices lines intermittenly said while actively in chase.")]
    public AudioClip[] activeChase;

    [Tooltip("Voice lines for when the enemy loses the player while in case.")]
    public AudioClip[] loseChase;

    [Tooltip("Voice lines for the enemy attempting to attack the player.")]
    public AudioClip[] attackAttempt;

    [Tooltip("Voice lines for the enemy successfully attacking and throwing the player.")]
    public AudioClip[] attackSuccess;

    [Tooltip("Voice lines for the enemy failing their attack.")]
    public AudioClip[] attackFailure;

    [HideInInspector] public AudioClip[][] soundCollections;


    public void Assemble()
    {
        soundCollections = new AudioClip[][]
        {
            takeDamage,
            stunStart,
            stunIdle,
            stunRecover,
            startChase,
            activeChase,
            loseChase,
            attackAttempt,
            attackSuccess,
            attackFailure
        };
    }
}
