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
    [SerializeField] private Animator backgroundAnim;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        try
        {
            taskManager = FindAnyObjectByType<TaskManager>();
        }
        catch
        {
            throw new ArgumentException("No task manager script found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        timerSlider.value = taskManager.timeRemaining / taskManager.maxLimit;

        if(taskManager.overdue == true)
        {
            clockAnim.SetBool("Overtime", true);
            backgroundAnim.speed = 2;
        }
        else
        {
            clockAnim.SetBool("Overtime", false);
            backgroundAnim.speed = 1;
        }
    }

    public void SetTask(string taskName, string taskLocation)
    {
        taskText.text = $"Task: {taskName}\nLocation: {taskLocation}";
    }
}
