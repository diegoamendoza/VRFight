using TMPro;
using UnityEngine;

public class FightSceneTextManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statustext1;
    [SerializeField] private TextMeshProUGUI statustext2;

    public void SetStatusText(string text1, string text2)
    {
        if (statustext1 != null) statustext1.text = text1;
        if (statustext2 != null) statustext2.text = text2;
    }
}
