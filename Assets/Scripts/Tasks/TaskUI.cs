using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskUI : MonoBehaviour
{
    [SerializeField] private Slider timerSlider;

    [SerializeField] private TextMeshProUGUI taskText;

    [SerializeField] private TaskManager taskManager;

    [SerializeField] private Animator clockAnim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (taskManager == null)
        {
            return;
        }

        if (timerSlider != null && taskManager.maxLimit > 0f)
        {
            timerSlider.value = taskManager.timeRemaining / taskManager.maxLimit;
        }

        if (clockAnim != null)
        {
            clockAnim.SetBool("Overtime", taskManager.overdue);
        }
    }

    public void SetTask(string taskName, string taskLocation)
    {
        if (taskText == null)
        {
            return;
        }

        taskText.text = $"Task: {taskName}\nLocation: {taskLocation}";
    }
}
