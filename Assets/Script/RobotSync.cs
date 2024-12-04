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
        Debug.Log("Sincronizar");
    }

    [PunRPC]
    public void SyncRobotStats(int[] stats)
    {
        RobotStats actualStats = GetComponent<RobotStats>();

        actualStats.attack += stats[0];
        actualStats.defense += stats[1];
        actualStats.health += stats[2];
        actualStats.shield += stats[3];

        if(actualStats.shield >0)
        {
            GetComponent<RobotCombat>().StartShield();
        }

    }

    [PunRPC]
    public void SyncPasive(string pasive)
    {
        if (pasive == null) return;
    }


}
