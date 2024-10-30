using System.Collections;
using UnityEngine;

public class RobotCombat : MonoBehaviour
{
    public bool IsPlayerRobot; // Determina si el robot es del jugador o del oponente
    public bool IsAlive = true;
    private int health = 100;

    public void StartCombat()
    {
        if (IsAlive)
        {
            StartCoroutine(CombatRoutine());
        }
    }

    IEnumerator CombatRoutine()
    {
        while (IsAlive)
        {
            yield return new WaitForSeconds(1); // Simula ataques cada segundo
            TakeDamage(Random.Range(10, 20)); // Daño simulado
        }
    }

    void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            IsAlive = false;
            StopAllCoroutines(); // Detiene el combate del robot
        }
    }
}
