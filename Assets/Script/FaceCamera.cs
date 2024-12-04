using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // Obt�n la c�mara principal
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        // Asegura que el objeto mire hacia la c�mara
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                             mainCamera.transform.rotation * Vector3.up);
        }
    }
}
