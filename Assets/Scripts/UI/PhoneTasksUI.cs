using TMPro;
using UnityEngine;

// Fills HeldPhone tasks tab. Header and empty label live on the prefab; this script names the current task.
public class PhoneTasksUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameLabel;
    [SerializeField] TextMeshProUGUI locationLabel;
    [SerializeField] GameObject emptyLabel;
    [SerializeField] TextMeshProUGUI timerLabel;

    static readonly Color timerColor = new Color(0.86f, 0.9f, 0.94f, 1f);
    static readonly Color overdueColor = new Color(0.85f, 0.35f, 0.32f, 1f);

    TaskManager tasks;
    string lastTimerText;

    void OnEnable()
    {
        BindTasks();
        Refresh();
        RefreshTimer();
    }

    void OnDisable()
    {
        if (tasks != null)
        {
            tasks.TaskChanged -= Refresh;
        }
    }

    void LateUpdate()
    {
        RefreshTimer();
    }

    void BindTasks()
    {
        if (tasks != null)
        {
            tasks.TaskChanged -= Refresh;
        }

        tasks = TaskManager.Instance;

        if (tasks != null)
        {
            tasks.TaskChanged += Refresh;
        }
    }

    void Refresh()
    {
        bool hasTask = HasTask();
        if (nameLabel != null)
        {
            nameLabel.gameObject.SetActive(hasTask);
        }

        if (locationLabel != null)
        {
            locationLabel.gameObject.SetActive(hasTask);
        }

        if (emptyLabel != null)
        {
            emptyLabel.SetActive(!hasTask);
        }

        if (!hasTask)
        {
            return;
        }

        nameLabel.text = tasks.CurrentTaskName;
        locationLabel.text = tasks.CurrentTaskLocation;
    }

    void RefreshTimer()
    {
        bool hasTask = HasTask();
        if (timerLabel != null)
        {
            timerLabel.gameObject.SetActive(hasTask);
        }

        if (!hasTask || timerLabel == null)
        {
            lastTimerText = null;
            return;
        }

        string text;
        Color color = timerColor;
        if (tasks.overdue)
        {
            text = "OVERDUE";
            color = overdueColor;
        }
        else
        {
            int seconds = Mathf.CeilToInt(Mathf.Max(0f, tasks.timeRemaining));
            text = "TIME  " + (seconds / 60).ToString("00") + ":" + (seconds % 60).ToString("00");
        }

        if (text == lastTimerText)
        {
            return;
        }

        lastTimerText = text;
        timerLabel.text = text;
        timerLabel.color = color;
    }

    bool HasTask()
    {
        return tasks != null && !string.IsNullOrEmpty(tasks.CurrentTaskName);
    }
}
