using System.Linq;
using System;
using UnityEngine;

public class EnemyVLManager : MonoBehaviour
{
    [Tooltip("Storage for enemy Voice Lines")]
    [SerializeField] private Voicelines hrVoiceLines;

    private AudioClip activeSound;
    private int activePriority;

    public void Start()
    {
        hrVoiceLines.Assemble();
    }


    public void PlayVoiceLine(string vlName)
    {
        if (hrVoiceLines.categories.Contains(vlName))
        {
            int chosenCategory = Array.FindIndex(hrVoiceLines.categories, x => x == vlName);

            if(hrVoiceLines.soundCollections[chosenCategory].Length > 0)
            {
                int randomClip = UnityEngine.Random.Range(0, hrVoiceLines.soundCollections[chosenCategory].Length);

                AudioManager.Instance.Play3DSFX(hrVoiceLines.soundCollections[chosenCategory][randomClip], hrVoiceLines.soundCollections[chosenCategory][randomClip].length + 0.2f, transform.position);
                //Debug.Log(hrVoiceLines.soundCollections[chosenCategory][randomClip].name);
            }
        }
        else
        {
            Debug.LogWarning($"No category for voicelines called {vlName} found!");
        }
        
    }
}
