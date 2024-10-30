using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class RobotSpawn : MonoBehaviour
{
    public GameObject robotPrefab; // Prefab del robot
    public LayerMask platformLayerMask; // Capa de la plataforma para el Raycast
    public Vector3 gizmoSize = new Vector3(5f, 0.1f, 3f); // Tama�o del �rea de spawn
    public Color gizmoColor = Color.green; // Color del Gizmo
    public bool isPlayerOne = true; // Diferencia entre la zona del Jugador 1 y Jugador 2

    private Camera arCamera;
    private Vector3 gizmoCenter; // Centro del Gizmo

    void Start()
    {
        arCamera = Camera.main; // C�mara principal en AR
    }

    void Update()
    {
        // Detecci�n de toque
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = arCamera.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            // Verifica si el toque fue en la plataforma
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, platformLayerMask))
            {
                // Verifica si est� dentro del �rea de spawn del jugador
                if (IsWithinSpawnArea(hit.point))
                {
                    // Solo el jugador local puede instanciar su robot
                    SpawnRobot(hit.point);
                }
                else
                {
                    Debug.Log("No puedes colocar el robot fuera de tu �rea de spawn.");
                }
            }
        }
    }

    // M�todo para instanciar el robot en la posici�n tocada
    void SpawnRobot(Vector3 spawnPosition)
    {
        PhotonNetwork.Instantiate("RobotPrefab", spawnPosition, Quaternion.identity);
    }

    // Verifica si el punto de toque est� dentro del �rea de spawn del jugador
    bool IsWithinSpawnArea(Vector3 touchPosition)
    {
        float halfWidth = gizmoSize.x / 2;
        return isPlayerOne ? (touchPosition.x < gizmoCenter.x + halfWidth) : (touchPosition.x > gizmoCenter.x - halfWidth);
    }

    // M�todo para dibujar el Gizmo en la escena
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        // Calcula el centro del �rea de spawn basado en si es el Jugador 1 o 2
        gizmoCenter = transform.position;
        if (isPlayerOne)
        {
            gizmoCenter.x -= gizmoSize.x / 2; // El Jugador 1 tiene la mitad izquierda
        }
        else
        {
            gizmoCenter.x += gizmoSize.x / 2; // El Jugador 2 tiene la mitad derecha
        }

        // Dibuja el �rea de spawn en la escena
        Gizmos.DrawCube(gizmoCenter, gizmoSize);
    }
}
