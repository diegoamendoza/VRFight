using MoreMountains.Feedbacks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab; // Prefab del jugador
    public GameObject intro, marcador;
    public GameObject uiHead, uiBody, uiLegs;
    public ARTrackedImageManager aRTrackedImageManager;
    public Animator CardsUIAnimator;

    private void Start()
    {
        SpawnPlayer();

        playerPrefab.GetComponent<ArenaManager>().introAnimation = intro;
        playerPrefab.GetComponent<ArenaManager>().marcador = marcador;

        

    }

    private void SpawnPlayer()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            // Instanciar el prefab del jugador en la posición (0,0,0) con rotación predeterminada
            GameObject playerPrefab2 = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity, 0);

            playerPrefab2.GetComponent<ArenaManager>().introAnimation = intro;
            playerPrefab2.GetComponent<ArenaManager>().marcador = marcador;
 

            //playerPrefab2.GetComponent<MecanicaImage>().arTrackedImageManager = aRTrackedImageManager;
        }
    }

    public override void OnJoinedRoom()
    {
        SpawnPlayer(); // Asegúrate de que el jugador se instancie al unirse a la sala
    }
}
