using UnityEngine;

public class ParkingSpot : MonoBehaviour
{
    [SerializeField] private GameObject painel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CarMovement>(out _))
            painel.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<CarMovement>(out _))
            painel.SetActive(false);
    }
}
