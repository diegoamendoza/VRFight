using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    int maxHP;

    [SerializeField]
    Image healthBarImage;

    [PunRPC]
    public void UpdateHealthBar(int currentHealth)
    {
        healthBarImage.fillAmount = currentHealth / maxHP;
    }

    [PunRPC]
    public void UpdateMaxHP(int newMaxHP)
    {
        maxHP = newMaxHP;
    }
}
