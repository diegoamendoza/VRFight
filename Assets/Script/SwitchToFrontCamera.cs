using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SwitchToFrontCamera : MonoBehaviour
{
    private ARCameraManager arCameraManager;

    public void SwitchCamera()
    {
        arCameraManager = GetComponent<ARCameraManager>();

        if (arCameraManager != null)
        {
            if (arCameraManager.requestedFacingDirection != CameraFacingDirection.User)
            {
                // Cambiar a la cámara frontal
                arCameraManager.requestedFacingDirection = CameraFacingDirection.User;
            }
			else
			{
                arCameraManager.requestedFacingDirection = CameraFacingDirection.World;
            }
        }
    }
}