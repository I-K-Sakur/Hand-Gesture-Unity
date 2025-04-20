using UnityEngine;

public class MountRotation : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 20f;
    private int rotationDirection = 0; // 0 = stop, 1 = forward, -1 = reverse

    void Update()
    {
        if (rotationDirection != 0)
        {
            transform.Rotate(Vector3.right * _rotationSpeed * rotationDirection * Time.deltaTime);
        }
    }

    public void StartForwardRotation()
    {
        rotationDirection = 1;
    }

    public void StartReverseRotation()
    {
        rotationDirection = -1;
    }

    public void StopRotation()
    {
        rotationDirection = 0;
    }
}