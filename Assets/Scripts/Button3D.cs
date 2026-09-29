using UnityEngine;
using UnityEngine.InputSystem;

public class Button3D : MonoBehaviour
{
    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.TryGetComponent(out TaskHolder holder))
                    holder.AcceptTask();
            }
        }
    }
}
