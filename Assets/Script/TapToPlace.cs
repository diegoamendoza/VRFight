using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Photon.Pun;

public class TapToPlace : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public GameObject prefab;
    public GameObject prefabObject;
    public bool isReadyToPlace = false;
    public bool objectReady = false;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private MecanicaImage mecanicaImage;
    public Camera arCamera;
    public RobotStats currentRobotStats; // Para almacenar las estadísticas del robot
    
    void Awake()
    {
        currentRobotStats = GetComponent<RobotStats>();
        mecanicaImage = FindObjectOfType<MecanicaImage>();
    }

    void Update()
    {
        if (Input.touchCount > 0 && isReadyToPlace)
        {
            Touch touch = Input.GetTouch(0);
            PlaceObjectOnArena(touch);
        }

        if(Input.touchCount > 0 && objectReady)
        {
            Touch touch2 = Input.GetTouch(0);
            PlaceObjectOnRobot(touch2);
        }
    }

    public void PlaceObjectOnRobot(Touch touch)
    {
        Ray ray = arCamera.ScreenPointToRay(touch.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if(hit.collider.CompareTag("Robot"))
            {
                PhotonView photonView = hit.collider.gameObject.GetComponent<PhotonView>();
                GameObject spawnObject = PhotonNetwork.Instantiate(prefabObject.name, hit.point, Quaternion.identity);
                RobotStats objectStats = spawnObject.GetComponent<RobotStats>();
                int[] stats = new int[7];
                stats[0] = objectStats.attack;
                stats[1] = objectStats.defense;
                stats[2] = objectStats.health;
                stats[3] = objectStats.shield;

                string pasive = spawnObject.GetComponent<ObjectPasive>().pasive;
                photonView.RPC("SyncRobotStats", RpcTarget.All, stats);
                photonView.RPC("SyncPasive", RpcTarget.All, pasive);
                PhotonNetwork.Destroy(spawnObject);
                objectReady = false;
                mecanicaImage.cardsUIAnimator.SetTrigger("DesactivateCardD");
                Debug.Log("Se agrego el objeto " + prefabObject.name);
            }
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
                // Instanciar el robot
                GameObject spawnedRobot = PhotonNetwork.Instantiate(prefab.name, hit.point, Quaternion.identity);
                spawnedRobot.GetComponent<RobotCombat>().robotStats = currentRobotStats;

                int[] stats = new int[7];
                stats[0] = currentRobotStats.attack;
                stats[1] = currentRobotStats.defense;
                stats[2] = currentRobotStats.health;
               
                
               
                Transform arenaTransform = hit.collider.transform;
                spawnedRobot.transform.SetParent(arenaTransform, true); 
                
              
                Vector3 localPosition = spawnedRobot.transform.localPosition;
                Debug.Log(localPosition);
           
                PhotonView photonView = spawnedRobot.GetComponent<PhotonView>();
                photonView.RPC("SetRobotLocalPosition", RpcTarget.Others, localPosition);
                photonView.RPC("SyncRobotStats", RpcTarget.All, stats);
                

                // Restablece la mecánica de selección de piezas
                isReadyToPlace = false;
                mecanicaImage.ResetParts();
            }
        }
    }
}
