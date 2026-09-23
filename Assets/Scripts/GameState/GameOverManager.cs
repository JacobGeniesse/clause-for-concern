using Unity.Cinemachine;
using UnityEngine;
using System;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverCanvas;
    private IEnumerator gameOverCoroutine;
    [SerializeField]private GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void GameOver(GameObject jumpscareCam, GameObject playerCam, Animator anim)
    {
        StartCoroutine(GameOverProcess(jumpscareCam, playerCam, anim));
    }

    private IEnumerator GameOverProcess(GameObject jumpscareCam, GameObject playerCam, Animator anim)
    {
        jumpscareCam.SetActive(true);
        playerCam.SetActive(false);
        anim.SetTrigger("Jumpscare");
        yield return new WaitForSeconds(0.45f);
        Cursor.lockState = CursorLockMode.None;
        gameOverCanvas.SetActive(true);
        gameManager.ModTime(0);
    }
}
