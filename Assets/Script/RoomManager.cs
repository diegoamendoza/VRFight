using UnityEngine;
using Photon.Pun; // Aseg�rate de tener Photon.Pun en tus usings
using UnityEngine.SceneManagement; // Para cambiar de escena
using UnityEngine.UI; // Para interactuar con la UI
using QRCoder; // Aseg�rate de tener QRCoder en tus usings
using QRCoder.Unity; // Para usar UnityQRCode
using System.Collections;
using TMPro;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public string roomName;
    public Button createRoomButton; // Bot�n para crear la sala
    public GameObject waitingUI; // UI para "Esperando un jugador"
    public GameObject waitingUI2; // UI para "Esperando un jugador"
    public TMP_Text waitingText; // Texto para mostrar el mensaje de espera
    public TMP_Text waitingText2; // Texto para mostrar el mensaje de espera
    public Image qrCodeImage; // Imagen donde se mostrar� el QR
    public float waitTime = 2f; // Tiempo antes de cambiar a la escena de pelea

    void Start()
    {
        // Aseg�rate de que el bot�n est� vinculado
        if (createRoomButton != null)
        {
            createRoomButton.onClick.AddListener(CreateRoom);
        }
        else
        {
            Debug.LogError("El bot�n para crear la sala no est� asignado.");
        }

        // Desactivar la UI de espera y el QR al inicio
        waitingUI.SetActive(false);
        waitingUI2.SetActive(false);
        qrCodeImage.gameObject.SetActive(false);
    }

    void CreateRoom()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogError("No est�s conectado a Photon. Con�ctate primero.");
            return; // Salir del m�todo si no est�s conectado
        }
        roomName = "Sala_" + Random.Range(1000, 9999);
        // Crear una nueva sala con un nombre aleatorio
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2; // Establecer el n�mero m�ximo de jugadores en la sala

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }


    public override void OnCreatedRoom()
    {
        Debug.Log("Sala creada: " + roomName);
        // Activar la UI de "Esperando un jugador"
        waitingUI.SetActive(true);
        waitingUI2.SetActive(true);
        waitingText.text = "Waiting for other player...";
        waitingText2.text = "Waiting for other player...";

        // Generar y mostrar el QR
        GenerateQRCode(roomName);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        // Cuando un nuevo jugador entra a la sala
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2) // Si hay 2 jugadores en la sala
        {
            waitingText.text = "Rival found!";
            waitingText2.text = "Rival found!";
            StartCoroutine(StartFightAfterDelay());
        }
    }

    private IEnumerator StartFightAfterDelay()
    {
        yield return new WaitForSeconds(waitTime); // Esperar el tiempo especificado
                                                   // Cambiar a la escena de pelea usando PhotonNetwork para sincronizar a todos los jugadores
        PhotonNetwork.LoadLevel("FightScene"); // Aseg�rate de que esta escena est� a�adida en el Build Settings
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Error al crear la sala: " + message);
        // Podr�as manejar el error aqu�, tal vez intentar crear otra sala o mostrar un mensaje al usuario
    }

    private void GenerateQRCode(string roomName)
    {
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(roomName, QRCodeGenerator.ECCLevel.Q);
        UnityQRCode qrCode = new UnityQRCode(qrCodeData);
        Texture2D qrCodeAsTexture2D = qrCode.GetGraphic(20);

        // Asignar la textura generada a la imagen de la UI
        qrCodeImage.sprite = Sprite.Create(qrCodeAsTexture2D, new Rect(0, 0, qrCodeAsTexture2D.width, qrCodeAsTexture2D.height), new Vector2(0.5f, 0.5f));
        qrCodeImage.gameObject.SetActive(true); // Asegurarse de que la imagen sea visible
    }
}
