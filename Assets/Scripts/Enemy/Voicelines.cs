using UnityEngine;

[CreateAssetMenu(fileName = "Voiceline", menuName = "Scriptable Objects/Voiceline")]
public class Voicelines : ScriptableObject
{
    [Tooltip("Voice lines for when the enemy takes damage.")]
    [SerializeField] private AudioClip[] takeDamage;

    [Tooltip("Voice lines for when the enemy gets stunned.")]
    [SerializeField] private AudioClip[] stunStart;

    [Tooltip("Voice lines for when the enemy is actively stunned.")]
    [SerializeField] private AudioClip[] stunIdle;

    [Tooltip("Voice lines for when the enemy recovers from being stunned.")]
    [SerializeField] private AudioClip[] stunRecover;

    [Tooltip("Voice lines for when the enemy begins chasing the player.")]
    [SerializeField] private AudioClip[] startChase;

    [Tooltip("Voices lines intermittenly said while actively in chase.")]
    [SerializeField] private AudioClip[] activeChase;

    [Tooltip("Voice lines for when the enemy loses the player while in case.")]
    [SerializeField] private AudioClip[] loseChase;

    [Tooltip("Voice lines for the enemy attempting to attack the player.")]
    [SerializeField] private AudioClip[] attackAttempt;

    [Tooltip("Voice lines for the enemy successfully attacking and throwing the player.")]
    [SerializeField] private AudioClip[] attackSuccess;

    [Tooltip("Voice lines for the enemy failing their attack.")]
    [SerializeField] private AudioClip[] attackFailure;
}
