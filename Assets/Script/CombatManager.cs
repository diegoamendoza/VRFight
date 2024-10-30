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

    private int playerScore = 0;
    private int opponentScore = 0;
    private bool isPlayerReady = false;
    private bool isOpponentReady = false;

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
            StartCoroutine(BeginCombat());
        }
        else
        {
            // statusText.text = isPlayerReady ? "Esperando al oponente..." : "Presiona 'Listo' cuando estés preparado.";
        }
    }

    IEnumerator BeginCombat()
    {
        // statusText.text = "¡Combate en progreso!";
        yield return new WaitForSeconds(2); // Breve pausa antes de iniciar el combate

        // Llama a los métodos de combate en los robots de ambos jugadores (por ejemplo, inicia sus ataques)
        foreach (var robot in FindObjectsOfType<RobotCombat>())
        {
            robot.StartCombat();
        }

        // Espera hasta que solo un jugador tenga robots en pie
        while (!CheckForWinner())
        {
            yield return null;
        }

        EndCombat();
    }



    bool CheckForWinner()
    {
        // Comprueba si solo quedan robots de un jugador
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

        return playerRobotsAlive == 0 || opponentRobotsAlive == 0;
    }

    void EndCombat()
    {
        bool playerWon = CheckForWinner();
        if (playerWon)
        {
            playerScore++;
            statusText.text = "¡Has ganado la ronda!";
            statusText2.text = "¡Has ganado la ronda!";
        }
        else
        {
            opponentScore++;
            statusText.text = "Has perdido la ronda.";
            statusText2.text = "Has perdido la ronda.";
        }

        // Actualiza la UI de puntaje
        playerScoreText.text = playerScore.ToString();
        playerScoreText2.text = playerScore.ToString();
        opponentScoreText.text =  opponentScore.ToString();
        opponentScoreText2.text =  opponentScore.ToString();

        // Reinicia el estado
        isPlayerReady = false;
        isOpponentReady = false;
        readyButton.interactable = true; // Reactiva el botón "Listo" para la siguiente ronda
    }
}
