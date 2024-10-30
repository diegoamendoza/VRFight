using UnityEngine;
using Photon.Pun;

public class RobotSync : MonoBehaviourPun
{
    [PunRPC] // Asegúrate de que este método esté marcado como RPC
    public void SetRobotLocalPosition(Vector3 localPosition)
    {
        // Cambiar la posición local

        // Establecer como hijo del objeto Arena en este dispositivo
        Transform arenaTransform = GameObject.FindGameObjectWithTag("Arena").transform;
        transform.SetParent(arenaTransform, true);
        transform.localPosition = localPosition;
    }
}
