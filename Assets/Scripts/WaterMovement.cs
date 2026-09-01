using UnityEngine;

public class WaterMovement : MonoBehaviour
{
    [SerializeField] float mais = 0.38f;
    
    void Update()
    {
        
        float wave = (Mathf.Sin(Time.time * 0.7f) * 0.03f) + mais;

        transform.position = new Vector3(
            transform.position.x,
            wave,
            transform.position.z
        );
    }
}