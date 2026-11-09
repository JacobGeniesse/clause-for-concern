using System.Linq;
using System;
using UnityEngine;
using NUnit.Framework;

public class EnemyVLManager : MonoBehaviour
{
    [Tooltip("Storage for enemy Voice Lines")]
    [SerializeField] private Voicelines hrVoiceLines;

    private AudioClip activeSound;
    private int activePriority;

    public void Start()
    {
    }


    public void PlayVoiceLine(string vlName)
    {
        if (hrVoiceLines.categories.Contains(vlName))
        {
            int chosenCategory = hrVoiceLines.categories.FindIndex(x => x == vlName);

            if(hrVoiceLines.soundCollections[chosenCategory].voiceLineList.Count > 0)
            {
                int randomClip = UnityEngine.Random.Range(0, hrVoiceLines.soundCollections[chosenCategory].voiceLineList.Count);

                AudioManager.Instance.Play3DSFX(hrVoiceLines.soundCollections[chosenCategory].voiceLineList[randomClip], hrVoiceLines.soundCollections[chosenCategory].voiceLineList[randomClip].length + 0.2f, transform.position);
                //Debug.Log(hrVoiceLines.soundCollections[chosenCategory][randomClip].name);
            }
        }
        else
        {
            Debug.LogWarning($"No category for voicelines called {vlName} found!");
        }
        
    }
}
