using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObject : MonoBehaviour, ISource
{
    public AudioClip chosenClip; //Clip chosen by the AudioManager

    [Tooltip("Reference to the Sound Player on this Game Object")]
    [SerializeField] private AudioSource soundPlayer;
    private IEnumerator clipCoroutine = null; //Coroutine for playing an audio clip

    private List<IObserver> observers = new List<IObserver>();

    private void OnDisable()
    {
        Notify();
    }


    //Func for playing a clip
    public void PlayClip(AudioClip clip, float clipLength)
    {
        //Assign audio clip
        chosenClip = clip;
        //Play audio clip
        if (clipCoroutine == null)
        {
            clipCoroutine = playClip(clipLength);
            StartCoroutine(clipCoroutine);
        }
    }

    //Func for helping to play an audio clip
    IEnumerator playClip(float clipLength)
    {
        //Play audio clip
        soundPlayer.clip = chosenClip;
        soundPlayer.Play();
        yield return new WaitForSeconds(clipLength);
        
        //Delete self upon audio clip ending
        if (ObjectPool.instance != null && AudioManager.Instance != null)
        {
            clipCoroutine = null;
            //If there is an ObjectPool then return this object to the object pool
            ObjectPool.instance.Destroy(this.gameObject, AudioManager.Instance.soundObjectPool);
        }
        else
        {
            //If there is no object pool destroy this gameobject
            Destroy(this.gameObject);
        }
    }


    public void StartListening(IObserver observer)
    {
        observers.Add(observer);
    }

    public void StopListening(IObserver observer)
    {
        observers.Remove(observer);
    }

    public void Notify()
    {
        if(observers.Count > 0)
        {
            for(int i = 0; i < observers.Count; i++)
            {
                observers[i].Trigger(this);
            }
        }
        
    }

}