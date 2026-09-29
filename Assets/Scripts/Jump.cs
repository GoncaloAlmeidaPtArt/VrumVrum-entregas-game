using UnityEngine;

public class Jump : MonoBehaviour
{
    [SerializeField] private float jumpForce = 2;
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        //Vector3 dir = -(other.transform.position - transform.position).normalized;
        rb.AddForce(other.transform.up*jumpForce, ForceMode.Force);
    }
}
