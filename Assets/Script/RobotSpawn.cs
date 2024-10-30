using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class RobotSpawn : MonoBehaviour
{
    public GameObject robotPrefab; // Prefab del robot
    public LayerMask platformLayerMask; // Capa de la plataforma para el Raycast
    public Vector3 gizmoSize = new Vector3(5f, 0.1f, 3f); // Tamaño del área de spawn
    public Color gizmoColor = Color.green; // Color del Gizmo
    public bool isPlayerOne = true; // Diferencia entre la zona del Jugador 1 y Jugador 2

    private Camera arCamera;
    private Vector3 gizmoCenter; // Centro del Gizmo

    void Start()
    {
        arCamera = Camera.main; // Cámara principal en AR
    }

    void Update()
    {
        // Detección de toque
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = arCamera.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            // Verifica si el toque fue en la plataforma
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, platformLayerMask))
            {
                // Verifica si está dentro del área de spawn del jugador
                if (IsWithinSpawnArea(hit.point))
                {
                    // Solo el jugador local puede instanciar su robot
                    SpawnRobot(hit.point);
                }
                else
                {
                    Debug.Log("No puedes colocar el robot fuera de tu área de spawn.");
                }
            }
        }
    }

    // Método para instanciar el robot en la posición tocada
    void SpawnRobot(Vector3 spawnPosition)
    {
        PhotonNetwork.Instantiate("RobotPrefab", spawnPosition, Quaternion.identity);
    }

    // Verifica si el punto de toque está dentro del área de spawn del jugador
    bool IsWithinSpawnArea(Vector3 touchPosition)
    {
        float halfWidth = gizmoSize.x / 2;
        return isPlayerOne ? (touchPosition.x < gizmoCenter.x + halfWidth) : (touchPosition.x > gizmoCenter.x - halfWidth);
    }

    // Método para dibujar el Gizmo en la escena
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        // Calcula el centro del área de spawn basado en si es el Jugador 1 o 2
        gizmoCenter = transform.position;
        if (isPlayerOne)
        {
            gizmoCenter.x -= gizmoSize.x / 2; // El Jugador 1 tiene la mitad izquierda
        }
        else
        {
            gizmoCenter.x += gizmoSize.x / 2; // El Jugador 2 tiene la mitad derecha
        }

        // Dibuja el área de spawn en la escena
        Gizmos.DrawCube(gizmoCenter, gizmoSize);
    }
}
