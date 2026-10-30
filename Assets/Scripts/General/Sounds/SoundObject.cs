using System.Collections;
using UnityEngine;

public class SoundObject : MonoBehaviour
{
    public AudioClip chosenClip; //Clip chosen by the AudioManager

    [Tooltip("Reference to the Sound Player on this Game Object")]
    [SerializeField] private AudioSource soundPlayer;
    private IEnumerator clipCoroutine = null; //Coroutine for playing an audio clip

    //Func for playing a clip
    public void PlayClip(AudioClip clip)
    {
        //Assign audio clip
        chosenClip = clip;
        //Play audio clip
        if (clipCoroutine == null)
        {
            clipCoroutine = playClip();
            StartCoroutine(clipCoroutine);
        }
    }

    //Func for helping to play an audio clip
    IEnumerator playClip()
    {
        //Play audio clip
        soundPlayer.clip = chosenClip;
        soundPlayer.Play();
        yield return new WaitForSeconds(chosenClip.length);
        
        //Delete self upon audio clip ending
        if (ObjectPool.instance != null && AudioManager.Instance != null)
        {
            //If there is an ObjectPool then return this object to the object pool
            ObjectPool.instance.Destroy(this.gameObject, AudioManager.Instance.soundObjectPool);
        }
        else
        {
            //If there is no object pool destroy this gameobject
            Destroy(this.gameObject);
        }
    }
}