using Unity.Cinemachine;
using UnityEngine;
using System;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Duplicate GameManager instance found in scene!");
            Destroy(this);
        }
    }


    [SerializeField] private AudioSource deathSound;
    [SerializeField] private AudioSource jumpscareSound;
    [SerializeField] private GameObject gameOverCanvas;
    private IEnumerator gameOverCoroutine;

    public void GameOver()
    {
        if(gameOverCanvas != null)
        {
            StartCoroutine(GameOverProcess());
        }
        else
        {
            Debug.LogError("Unable to start gameover sequence!");
        }
    }

    public void GameOver(GameObject jumpscareCam, GameObject playerCam, Animator anim)
    {
        StartCoroutine(GameOverProcess(jumpscareCam, playerCam, anim));
    }

    private IEnumerator GameOverProcess()
    {
        yield return new WaitForSeconds(0.2f);
        Cursor.lockState = CursorLockMode.None;
        gameOverCanvas.SetActive(true);
        GameManager.instance.ModTime(0);
    }
    private IEnumerator GameOverProcess(GameObject jumpscareCam, GameObject playerCam, Animator anim)
    {
        jumpscareSound.Play();
        jumpscareCam.SetActive(true);
        playerCam.SetActive(false);
        anim.SetTrigger("Jumpscare");
        yield return new WaitForSeconds(0.2f);
        deathSound.Play();
        yield return new WaitForSeconds(0.45f);
        jumpscareSound.Stop();
        Cursor.lockState = CursorLockMode.None;
        gameOverCanvas.SetActive(true);
        GameManager.instance.ModTime(0);
    }
}
