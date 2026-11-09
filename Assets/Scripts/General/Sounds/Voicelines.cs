using UnityEngine;
using System.Collections.Generic;

/*
 * Scriptable object for handling different versions of enemy voiceline sets
 * Built this way for ease of testing and showcasing different versions of character voicelines.
*/
[CreateAssetMenu(fileName = "Voiceline", menuName = "Scriptable Objects/Voiceline")]
public class Voicelines : ScriptableObject
{
    [HideInInspector] public List<string> categories = new List<string>();

    [Tooltip("The different voicelines this enemy can have.")]
    public VoiceList[] soundCollections;

    public void Assemble()
    {
        categories.Clear();
        foreach(VoiceList category in soundCollections)
        {
            //Debug.Log(category.categoryName);
            categories.Add(category.categoryName);
        }
    }

}

//Class for storing voicelines and the categories they fall into
[System.Serializable]
public class VoiceList
{
    public string categoryName; //Category of voiceline

    public int priority = 0;

    public List<AudioClip> voiceLineList; //List of audio clips belonging to that category.

    public override string ToString()
    {
        return categoryName;
    }

}
