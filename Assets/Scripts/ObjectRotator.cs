using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    public float speed = 45f;

    void Update()
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime, Space.Self);
    }
}
