using TMPro;
using UnityEngine;

// Fills HeldPhone tasks tab. Header and empty label live on the prefab; this script names the current task.
public class PhoneTasksUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameLabel;
    [SerializeField] TextMeshProUGUI locationLabel;
    [SerializeField] GameObject emptyLabel;

    TaskManager tasks;

    void OnEnable()
    {
        BindTasks();
        Refresh();
    }

    void OnDisable()
    {
        if (tasks != null)
        {
            tasks.TaskChanged -= Refresh;
        }
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
        bool hasTask = tasks != null && !string.IsNullOrEmpty(tasks.CurrentTaskName);
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
}
