using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Photon.Pun;

public class TapToPlace : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public GameObject prefab;
    public bool isReadyToPlace = false;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private MecanicaImage mecanicaImage;
    public Camera arCamera;
    public RobotStats currentRobotStats; // Para almacenar las estadísticas del robot

    void Awake()
    {
        mecanicaImage = FindObjectOfType<MecanicaImage>();
    }

    void Update()
    {
        if (Input.touchCount > 0 && isReadyToPlace)
        {
            Touch touch = Input.GetTouch(0);
            PlaceObjectOnArena(touch);
        }
    }

    private void PlaceObjectOnArena(Touch touch)
    {
        Ray ray = arCamera.ScreenPointToRay(touch.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Arena"))
            {
                RobotStats newStats = prefab.GetComponent<RobotStats>();
                newStats.robotName = currentRobotStats.robotName;
                newStats.attack = currentRobotStats.attack;
                newStats.defense = currentRobotStats.defense;
                newStats.health = currentRobotStats.health;
                newStats.criticalChance = currentRobotStats.criticalChance;
                newStats.criticalDamageMultiplier = currentRobotStats.criticalDamageMultiplier;
                newStats.attackRange = currentRobotStats.attackRange;
                newStats.moveSpeed = currentRobotStats.moveSpeed;
                // Instanciar el robot
                GameObject spawnedRobot = PhotonNetwork.Instantiate(prefab.name, hit.point, Quaternion.identity);
                spawnedRobot.GetComponent<RobotCombat>().robotStats = currentRobotStats;
                // Hacer que el robot sea hijo del objeto Arena
                Transform arenaTransform = hit.collider.transform; // Obtiene la referencia del objeto Arena
                spawnedRobot.transform.SetParent(arenaTransform, true); // Asigna el robot como hijo de la Arena
                
                // Guarda la posición local del robot respecto a la Arena
                Vector3 localPosition = spawnedRobot.transform.localPosition;
                Debug.Log(localPosition);
                // Sincroniza la posición local del robot
                PhotonView photonView = spawnedRobot.GetComponent<PhotonView>();
                photonView.RPC("SetRobotLocalPosition", RpcTarget.Others, localPosition);
                

                // Restablece la mecánica de selección de piezas
                isReadyToPlace = false;
                mecanicaImage.ResetParts();
            }
        }
    }
}
