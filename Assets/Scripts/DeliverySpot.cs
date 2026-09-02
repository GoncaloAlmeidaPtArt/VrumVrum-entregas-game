using UnityEngine;

public class DeliverySpot : MonoBehaviour
{
    private TaskManager taskManager;

    void Start()
    {
        taskManager = TaskManager.Instance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CarMovement>(out _) & taskManager.CurrentTask.GetDeliveryPoint == gameObject)
        {
            taskManager.CompleteCurrentTask();
            gameObject.SetActive(false);
        }
    }
}
