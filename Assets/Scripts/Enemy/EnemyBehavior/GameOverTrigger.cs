using UnityEngine;
using System;

public class GameOverTrigger : MonoBehaviour
{
    private GameOverManager gameOverManager;
    [SerializeField] private GameObject jumpscareCam;
    [SerializeField] private Animator anim;
    private GameObject playerCam;
    private bool isGamingOver = false;

    [SerializeField] EnemyBehavior enemyBehavior;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        try
        {
            gameOverManager = GameObject.Find("GameManager").GetComponent<GameOverManager>();
        }
        catch
        {
            throw new ArgumentException("Unable to find GameOverManager", nameof(GameOverTrigger));
        }

        try
        {
            playerCam = GameObject.Find("PlayerCamera");
        }
        catch
        {
            throw new ArgumentException("Unable to find player's camera!", nameof(GameOverManager));
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Player" && isGamingOver == false && enemyBehavior.aggressive == true)
        {
            isGamingOver = true;
            gameOverManager.GameOver(jumpscareCam, playerCam, anim);
        }
    }
}
