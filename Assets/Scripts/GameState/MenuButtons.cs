using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    [SerializeField] private string sceneName = "SampleScene";
    [SerializeField] private GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(gameManager == null)
        {
            Debug.LogError("Game Manager component for " + nameof(MenuButtons) + " is not assigned!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Resume()
    {
        gameManager.UnPause();
    }

    public void Retry()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quitting Game");
    }
}
