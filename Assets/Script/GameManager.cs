using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantener el GameManager en todas las escenas
        }
        else
        {
            Destroy(gameObject);
        }

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        ConnectToPhoton(); // Intentar conectar a Photon al iniciar
    }

    private void ConnectToPhoton()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings(); // Conectar a Photon usando la configuración predeterminada
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado al servidor de Photon.");

        // Asignar un nombre aleatorio al jugador
        PhotonNetwork.NickName = "Player" + Random.Range(1, 1001);

        // Aquí puedes hacer que el jugador se una a una sala o crear una sala
        // Por ejemplo: PhotonNetwork.JoinRandomRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"Desconectado de Photon: {cause}");
    }
}
