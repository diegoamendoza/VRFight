using UnityEngine;
using System.Collections;
using ZXing;
using UnityEngine.UI;
using System;
using Photon.Pun;  // Para usar Photon
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class ScanQRCode : MonoBehaviourPunCallbacks
{
    WebCamTexture webcamTexture;
    public string QrCode = string.Empty;
    private string errorMessage = string.Empty; // Mensaje de error
   // public Text statusText; // Texto de estado para mostrar el mensaje

    void Start()
    {
        // Comprobar permisos de cámara
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }

        // Inicializar la cámara
        var renderer = GetComponent<RawImage>();
        webcamTexture = new WebCamTexture(1000, 1000);
        renderer.texture = webcamTexture;

        // Comenzar la captura del QR
        StartCoroutine(GetQRCode());
    }

    IEnumerator GetQRCode()
    {
        IBarcodeReader barCodeReader = new BarcodeReader();
        webcamTexture.Play();
        var snap = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.ARGB32, false);

        while (string.IsNullOrEmpty(QrCode))
        {
            try
            {
                snap.SetPixels32(webcamTexture.GetPixels32());
                var result = barCodeReader.Decode(snap.GetRawTextureData(), webcamTexture.width, webcamTexture.height, RGBLuminanceSource.BitmapFormat.ARGB32);

                if (result != null)
                {
                    QrCode = result.Text;
                    if (!string.IsNullOrEmpty(QrCode))
                    {
                        Debug.Log("DECODED TEXT FROM QR: " + QrCode);

                        // Detener la cámara al encontrar un QR válido
                        webcamTexture.Stop();

                        // Conectarse a la sala de Photon con el nombre del QR
                        ConnectToRoom(QrCode);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }
            yield return null;
        }
        webcamTexture.Stop();
    }

    // Método para conectarse a la sala usando Photon
    void ConnectToRoom(string roomName)
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRoom(roomName);
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.NickName = "Player_" + UnityEngine.Random.Range(1000, 9999);
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRoom(QrCode);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        errorMessage = "Invalid Room: " + QrCode;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + QrCode);
        errorMessage = string.Empty;
      //  statusText.text = "Game found! Entering the room...";

       // StartCoroutine(EnterFightScene());
    }

    private IEnumerator EnterFightScene()
    {
        yield return new WaitForSeconds(1); // Esperar 1 segundo antes de cambiar de escena
        SceneManager.LoadScene("FightScene");
    }

    private void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();
        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 50;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);

        string text = string.IsNullOrEmpty(QrCode) ? "Scanning QR..." : "QR: " + QrCode;
        GUI.Label(rect, text, style);

        if (!string.IsNullOrEmpty(errorMessage))
        {
            Rect errorRect = new Rect(0, h / 10, w, h * 2 / 100);
            style.normal.textColor = Color.red;
            GUI.Label(errorRect, errorMessage, style);
        }
    }
}
