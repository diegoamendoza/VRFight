using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ArenaManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject arenaPrefab; // Arena prefab
    [SerializeField] public GameObject introAnimation, marcador ; // Objeto de la animación de introducción
    private FightSceneTextManager textManager; // Referencia al FightSceneTextManager
    private ARRaycastManager arRaycastManager;
    public ARPlaneManager arPlaneManager;
    private GameObject instantiatedArena;
    private bool arenaPlaced = false;
    private bool otherPlayerReady = false;

    private void Start()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        arPlaneManager = FindObjectOfType<ARPlaneManager>();
        textManager = FindObjectOfType<FightSceneTextManager>(); // Buscar el FightSceneTextManager
        // Inicializar el texto de estado
        textManager.SetStatusText("Tap on the plane to place the arena", "Tap on the plane to place the arena");

#if UNITY_EDITOR
        AutoSpawnArena();
#endif
    }

    private void Update()
    {
        if (arenaPlaced) return;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector2 touchPosition = Input.GetTouch(0).position;
            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            if (arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;

                if (instantiatedArena == null)
                {
                    instantiatedArena = Instantiate(arenaPrefab, hitPose.position, hitPose.rotation);
                    arenaPlaced = true;
                    arPlaneManager.enabled = false;
                    // Notificar a otros jugadores que este jugador está listo
                    photonView.RPC("PlayerReady", RpcTarget.All, PhotonNetwork.NickName);
                }
            }
        }
    }

    [PunRPC]
    void PlayerReady(string playerName)
    {
        if (playerName != PhotonNetwork.NickName)
        {
            otherPlayerReady = true;

            if (arenaPlaced)
            {
                textManager.SetStatusText("Both players are ready!", "Both players are ready!");
                StartCoroutine(ActivateBattleStarter());
            }
        }
        else
        {
            // Mensaje apropiado dependiendo del estado
            if (!arenaPlaced)
            {
                textManager.SetStatusText($"Waiting for {PhotonNetwork.PlayerListOthers[0].NickName} to place their arena...", $"Waiting for {PhotonNetwork.PlayerListOthers[0].NickName} to place their arena...");
            }
            else if (!otherPlayerReady)
            {
                // Ahora verifica si el otro jugador también está listo
                textManager.SetStatusText($"Waiting for {PhotonNetwork.PlayerListOthers[0].NickName} to be ready...", $"Waiting for {PhotonNetwork.PlayerListOthers[0].NickName} to be ready...");
            }
        }

        // Verifica si ambos jugadores están listos y la arena ha sido colocada
        CheckBothPlayersReady();
    }

    private void CheckBothPlayersReady()
    {
        if (arenaPlaced && otherPlayerReady)
        {
            textManager.SetStatusText("Both players are ready!", "Both players are ready!");
            StartCoroutine(ActivateBattleStarter());
        }
    }

    private IEnumerator ActivateBattleStarter()
    {
        yield return new WaitForSeconds(1); // Opcional retraso para el mensaje

        // Aquí activar la animación de introducción
        introAnimation.SetActive(true); // Asegúrate de que esté desactivado al inicio
        IntroManager introManager = FindObjectOfType<IntroManager>();
        string bluePlayerName = PhotonNetwork.NickName;
        string redPlayerName = PhotonNetwork.PlayerListOthers[0].NickName;
        introManager.SetPlayerNames(bluePlayerName, redPlayerName);

        Animator introAnimator = introManager.gameObject.GetComponent<Animator>();
        //introAnimator.SetTrigger("StartAnimation");

        // Esperar a que termine la animación de la introducción
        // Ajusta el tiempo según la duración de tu animación
        yield return new WaitForSeconds(introAnimator.GetCurrentAnimatorStateInfo(0).length);
        // Cambia esto a la duración de tu animación

        // Inicia la pelea o cualquier otra lógica que necesites aquí
        StartBattle();
    }

    private void StartBattle()
    {
        // Lógica para comenzar la batalla
        // Por ejemplo, ocultar la introducción y mostrar la arena
        marcador.SetActive(true);
        introAnimation.SetActive(false);
        Debug.Log("PELEAAA");
        // Inicia la pelea aquí
    }

    private void AutoSpawnArena()
    {
        // Método para auto-instanciar la arena en el editor
        if (instantiatedArena == null && arRaycastManager != null)
        {
            if (arenaPrefab != null)
            {
                instantiatedArena = Instantiate(arenaPrefab, Vector3.zero, Quaternion.identity);
                arenaPlaced = true;

                // Actualizar el texto de estado
                textManager.SetStatusText("Waiting for the other player to place their arena...", "Waiting for the other player to place their arena...");

                // Notificar a otros jugadores que este jugador está listo
                photonView.RPC("PlayerReady", RpcTarget.All, PhotonNetwork.NickName);
            }
            else
            {
                Debug.LogError("arenaPrefab is not assigned!");
            }
        }
        else
        {
            Debug.LogWarning("Arena has already been instantiated or AR Raycast Manager is null.");
        }
    }
}
