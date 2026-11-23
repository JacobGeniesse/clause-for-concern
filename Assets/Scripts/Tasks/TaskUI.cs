using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class TaskUI : MonoBehaviour
{
    [Tooltip("Reference to the slider component attached to the Timer UI")]
    [SerializeField] private Slider timerSlider;

    [Tooltip("Text component used for displaying the current task.")]
    [SerializeField] private TextMeshProUGUI taskText;

    private TaskManager taskManager; //Reference to the task manager

    [Tooltip("Reference to the clock attached to the timer display's animator component")]
    [SerializeField] private Animator clockAnim;

    [Tooltip("Reference to the background of the app's animator component")]
    [SerializeField] private Animator backgroundAnim;

    [Tooltip("Reference to the timer slider's fill component.")]
    [SerializeField] private Image sliderFill;

    void Start()
    {
        //Try to find the task manager in the scene
        try
        {
            taskManager = FindAnyObjectByType<TaskManager>();
        }
        catch
        {
            throw new ArgumentException("No task manager script found!", nameof(TaskUI));
        }
    }

    void Update()
    {
        //Set the timerSlider's value
        timerSlider.value = taskManager.timeRemaining / taskManager.maxLimit;

        //Change the slider's color based on time remaining
        if(timerSlider.value >= 0.5f)
        {
            sliderFill.color = Color.green; //Green for good
        }
        else if(timerSlider.value < 0.5f && timerSlider.value >= 0.2f)
        {
            sliderFill.color = Color.yellow; //Yellow for you're in trouble
        }
        else
        {
            sliderFill.color = Color.red; //Red for time has almost run out
        }

        //If a task is overdue
        if(taskManager.overdue == true)
        {
            clockAnim.SetBool("Overtime", true); //Make the clock change to angry mode
            backgroundAnim.speed = 2; //Speed up the pulsing of the background
        }
        else
        {
            clockAnim.SetBool("Overtime", false); //Make the clock change to calm mode
            backgroundAnim.speed = 1; //Set the speed of the animator back to normal
        }
    }

    //Func for making the task text
    public void SetTask(string taskName, string taskLocation)
    {
        taskText.text = $"Task: {taskName}\nLocation: {taskLocation}";
    }
}
