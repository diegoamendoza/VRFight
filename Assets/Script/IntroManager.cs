using UnityEngine;
using TMPro; // O usando UnityEngine.UI si no usas TextMeshPro

public class IntroManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bluePlayerNameText; // Para TextMeshPro
    [SerializeField] private TextMeshProUGUI bluePlayerNameText2; // Para TextMeshPro
    [SerializeField] private TextMeshProUGUI redPlayerNameText; // Para TextMeshPro
    [SerializeField] private TextMeshProUGUI redPlayerNameText2; // Para TextMeshPro

    // Método para establecer los nombres de los jugadores
    public void SetPlayerNames(string bluePlayerName, string redPlayerName)
    {
        bluePlayerNameText.text = bluePlayerName;
        bluePlayerNameText2.text = bluePlayerName;
        redPlayerNameText.text = redPlayerName;
        redPlayerNameText2.text = redPlayerName;
    }
}
