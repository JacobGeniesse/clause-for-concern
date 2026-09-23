using System;
using UnityEngine;
using UnityEngine.UI;

public class TaskUI : MonoBehaviour
{
    [SerializeField] private Slider timerSlider;
    [SerializeField] private TaskManager taskManager;

    [SerializeField] private Animator clockAnim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timerSlider.value = taskManager.timeRemaining / taskManager.maxLimit;

        if(taskManager.overdue == true)
        {
            clockAnim.SetBool("Overtime", true);
        }
        else
        {
            clockAnim.SetBool("Overtime", false);
        }
    }
}
