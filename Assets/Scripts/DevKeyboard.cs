using UnityEngine;
using UnityEngine.InputSystem;

public class DevKeyboard : MonoBehaviour
{
    [SerializeField] private GameObject player;
    void Update()
    {
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            player.transform.position = new Vector3(-1.6f,1f,-1.5f);
            player.transform.rotation = Quaternion.identity;
        }
    }
}
