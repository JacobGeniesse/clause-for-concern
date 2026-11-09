using System.Linq;
using System;
using UnityEngine;
using NUnit.Framework;

public class EnemyVLManager : MonoBehaviour, IObserver
{
    [Tooltip("Storage for enemy Voice Lines")]
    [SerializeField] private Voicelines voiceLines;

    private SoundObject activeSound;
    private int activePriority;

    public void Start()
    {
        voiceLines.Assemble();
    }


    public void PlayVoiceLine(string vlName)
    {
        if (voiceLines.categories.Contains(vlName))
        {
            int chosenCategory = voiceLines.categories.FindIndex(x => x == vlName);


            if(voiceLines.soundCollections[chosenCategory].voiceLineList.Count > 0)
            {
                int randomClip = UnityEngine.Random.Range(0, voiceLines.soundCollections[chosenCategory].voiceLineList.Count);

                if (voiceLines.soundCollections[chosenCategory].priority >= activePriority)
                {
                    if(activeSound != null)
                    {
                        activeSound.StopListening(this);
                    }

                    activeSound = AudioManager.Instance.Play3DSFX(voiceLines.soundCollections[chosenCategory].voiceLineList[randomClip], voiceLines.soundCollections[chosenCategory].voiceLineList[randomClip].length + 0.2f, transform.position);
                    if (activeSound != null)
                    {
                        activeSound.StartListening(this);
                        activePriority = voiceLines.soundCollections[chosenCategory].priority;
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning($"No category for voicelines called {vlName} found!");
        }
        
    }

    public void Trigger(ISource source)
    {
        source.StopListening(this);
        activeSound = null;
        activePriority = 0;
    }
}
