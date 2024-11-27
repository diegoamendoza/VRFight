using System.Collections;
using UnityEngine;
using TMPro; // Importa TextMeshPro
using UnityEngine.UI;
using Photon.Pun;

public class CombatManager : MonoBehaviourPunCallbacks
{
    public Button readyButton;
    public TMP_Text statusText, statusText2; // Cambiado a TMP_Text
    public TMP_Text playerScoreText, playerScoreText2; // Cambiado a TMP_Text
    public TMP_Text opponentScoreText, opponentScoreText2; // Cambiado a TMP_Text

    private float playerScore = 0;
    private float opponentScore = 0;
    private bool isPlayerReady = false;
    private bool isOpponentReady = false;
    [SerializeField] bool didPlayerWin;

    void Start()
    {
        readyButton.onClick.AddListener(OnReadyButtonPressed);
    }

    void OnReadyButtonPressed()
    {
        isPlayerReady = true;
        readyButton.interactable = false; // Desactiva el botón después de presionarlo
        photonView.RPC("UpdateReadyState", RpcTarget.Others, true); // Envía el estado listo al oponente
        CheckBothPlayersReady();
    }

    [PunRPC]
    void UpdateReadyState(bool opponentReady)
    {
        isOpponentReady = opponentReady;
        CheckBothPlayersReady();
    }

    void CheckBothPlayersReady()
    {
        if (isPlayerReady && isOpponentReady)
        {
            photonView.RPC("StartCombat", RpcTarget.All); // Sincroniza el inicio del combate
        }
        else
        {
            statusText.text = isPlayerReady ? "Esperando al oponente..." : "Presiona 'Listo' cuando estés preparado.";
            statusText2.text = statusText.text;
        }
    }

    [PunRPC]
    void StartCombat()
    {
        StartCoroutine(BeginCombat());
    }

    IEnumerator BeginCombat()
    {
        statusText.text = "¡Combate en progreso!";
        statusText2.text = statusText.text;

        yield return new WaitForSeconds(2); // Breve pausa antes de iniciar el combate

        // Inicia el combate de todos los robots
        foreach (var robot in FindObjectsOfType<RobotCombat>())
        {
            robot.StartCombat();
        }

        // Espera hasta que solo un jugador tenga robots en pie
        while (!CheckForWinner())
        {
            yield return null;
        }

        photonView.RPC("EndCombat", RpcTarget.All);
    }

    bool CheckForWinner()
    {
        int playerRobotsAlive = 0;
        int opponentRobotsAlive = 0;

        foreach (var robot in FindObjectsOfType<RobotCombat>())
        {
            if (robot.IsAlive)
            {
                if (robot.IsPlayerRobot) playerRobotsAlive++;
                else opponentRobotsAlive++;
            }
        }

        if(playerRobotsAlive == 0)
        {
            didPlayerWin = false;
        }
        if(opponentRobotsAlive == 0)
        {
            didPlayerWin = true;
        }
        //XDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD

        return playerRobotsAlive == 0 || opponentRobotsAlive == 0;
    }

    [PunRPC]
    void EndCombat()
    {
        bool playerWon = didPlayerWin;
        if (playerWon)
        {
            playerScore += 0.25f;
            statusText.text = "¡Has ganado la ronda!";
            statusText2.text = statusText.text;
        }
        else
        {
            opponentScore += 0.25f;
            statusText.text = "Has perdido la ronda.";
            statusText2.text = statusText.text;
        }

        // Actualiza la UI de puntaje
        playerScoreText.text = playerScore.ToString();
        playerScoreText2.text = playerScore.ToString();
        opponentScoreText.text = opponentScore.ToString();
        opponentScoreText2.text = opponentScore.ToString();

        // Reinicia el estado para una nueva ronda
        isPlayerReady = false;
        isOpponentReady = false;
        readyButton.interactable = true; // Reactiva el botón "Listo" para la siguiente ronda
    }
}
