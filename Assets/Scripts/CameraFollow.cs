using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float height;
    [SerializeField] private float distance;
    [SerializeField] private float followResponsiveness;
    [SerializeField] private float angleResponsiveness;
    [SerializeField] private float collisionBuffer = 0.2f;
    [SerializeField] private LayerMask obstacleMask = ~0;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPivot = target.position + Vector3.up * height;

        Vector3 desiredPosition = targetPivot - target.forward * -distance;

        Vector3 direction = (desiredPosition - targetPivot).normalized;
        float desiredDistance = distance;

        Vector3 finalPosition;

        if (Physics.Raycast(targetPivot, direction, out RaycastHit hit, desiredDistance, obstacleMask))
        {
            float safeDistance = Mathf.Max(hit.distance - collisionBuffer, 0f);
            finalPosition = targetPivot + direction * safeDistance;
        }
        else
        {
            finalPosition = targetPivot + direction * desiredDistance;
        }

        transform.position = Vector3.Lerp(
            transform.position,
            finalPosition,
            Time.deltaTime * followResponsiveness
        );

        Quaternion desiredRotation = Quaternion.LookRotation(targetPivot - transform.position);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            Time.deltaTime * angleResponsiveness
        );
    }
}