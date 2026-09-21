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

    public void GameOver(GameObject jumpscareCam, GameObject playerCam)
    {
        StartCoroutine(GameOverProcess(jumpscareCam, playerCam));
    }

    private IEnumerator GameOverProcess(GameObject jumpscareCam, GameObject playerCam)
    {
        jumpscareCam.SetActive(true);
        playerCam.SetActive(false);
        yield return new WaitForSeconds(0.45f);
        Cursor.lockState = CursorLockMode.None;
        gameOverCanvas.SetActive(true);
        gameManager.ModTime(0);
    }
}
