using UnityEngine;

public class TaskHolder : MonoBehaviour
{
    private TaskManager _taskManager;
    [SerializeField] private Task task;

    private void Start()
    {
        _taskManager = TaskManager.Instance;
    }

    public void AcceptTask()
    {
        _taskManager.AcceptTask(task);
    }
}
