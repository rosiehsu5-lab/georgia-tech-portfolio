using UnityEngine;

public class Fish : MonoBehaviour
{
    private bool floatAnimation = true;
    private float floatSpeed = 2f;
    private float floatHeight = 0.3f;
    
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (floatAnimation)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
        
    }
}