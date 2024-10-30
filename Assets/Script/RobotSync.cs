using UnityEngine;
using Photon.Pun;

public class RobotSync : MonoBehaviourPun
{
    [PunRPC] // Aseg�rate de que este m�todo est� marcado como RPC
    public void SetRobotLocalPosition(Vector3 localPosition)
    {
        // Cambiar la posici�n local

        // Establecer como hijo del objeto Arena en este dispositivo
        Transform arenaTransform = GameObject.FindGameObjectWithTag("Arena").transform;
        transform.SetParent(arenaTransform, true);
        transform.localPosition = localPosition;
    }
}
