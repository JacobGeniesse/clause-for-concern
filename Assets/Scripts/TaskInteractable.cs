using UnityEngine;
using System.Collections;

public class TaskInteractable : MonoBehaviour, IInteractable
{
    public bool currentTask = false;

    [SerializeField] private GameObject inputPopup;
    private Transform playerTrans;
    [SerializeField] private float minDistance = 10;

    private TaskManager taskManager;

    private CanvasGroup fadeScreen; // black fade screen
    [SerializeField] float fadeDuration = .5f; // fade duration 
    [SerializeField] float blackDuration = 1f; // how long the screen stays black
    private bool isFading = false;

    void Start()
    {
        if (taskManager == null)
        {
            taskManager = GameObject.Find("GameManager").GetComponent<TaskManager>();
        }

        if (playerTrans == null)
        {
            playerTrans = GameObject.Find("PlayerBody").GetComponent<Transform>();
        }
        if (fadeScreen == null)
        {
            playerTrans = GameObject.Find("FadeScreen").GetComponent<Transform>();
        }
    }


    void Update()
    {
        Vector3 playerOffest = playerTrans.position - transform.position;
        float distanceToPlayer = Vector3.SqrMagnitude(playerOffest);

        if (distanceToPlayer < minDistance && currentTask == true)
        {
            inputPopup.SetActive(true);
        }
        else
        {
            inputPopup.SetActive(false);
        }
    }


    public void Interact()
    {
        if (currentTask == true)
        {
            StartCoroutine(FadeTask());
        }
    }

    public void IncrementTask()
    {
        taskManager.CompleteTask();
    }
    private IEnumerator FadeTask()
    {
        isFading = true;
        GameManager.instance.ModTime(0); // freeze game time
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime; // adds the time since last frame, unscaled so it still counts while time is frozen
            fadeScreen.alpha = t / fadeDuration; // turns elapsed time into 0-1 so the screen gets darker each frame 
            yield return null; // waits one frame before looping again
        }
        fadeScreen.alpha = 1f;
        yield return new WaitForSecondsRealtime(blackDuration);
        IncrementTask();
        t = 0f;
        while(t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            fadeScreen.alpha = 1 - t / fadeDuration;
            yield return null;
        }
        fadeScreen.alpha = 0f;
        GameManager.instance.ModTime(1);
        isFading = false;
    }
}
