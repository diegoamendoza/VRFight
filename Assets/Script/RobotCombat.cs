using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RobotCombat : MonoBehaviourPunCallbacks
{
    public bool IsPlayerRobot;
    public bool IsAlive = true;
    public int baseHealth = 100;
    public int baseAttack = 10;
    public GameObject projectilePrefab;
    public GameObject shield;
    public Transform firePoint;
    public float fireRate = 1f;
    public float detectionRange = 10f;

    public Material enemyMaterialBlue;
    public Material enemyMaterialPurple;

    public RobotStats robotStats;
    public HealthBar healthBar;
    private int currentHealth;
    private int currentShield;
    private Transform targetEnemy;
    private bool isInCombat = false;

    private void Start()
    {
        if (photonView.IsMine)
        {
            IsPlayerRobot = true;
        }
        else
        {
            IsPlayerRobot = false;
            Renderer[] renderers = GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
            {
                if(renderer.material.name == "Blue")
                {
                    renderer.material = enemyMaterialBlue;
                }
                else
                {
                    renderer.material = enemyMaterialPurple;
                }
            }
        }
        robotStats = GetComponent<RobotStats>();
        
        currentHealth = baseHealth + robotStats.health;
        currentShield = robotStats.shield;
    }

    private void Update()
    {
        if (photonView.IsMine && IsAlive && isInCombat)
        {
            FindClosestEnemy();
            if (targetEnemy != null)
            {
                AttackTarget();
            }
        }
    }


    public void StartCombat()
    {
        isInCombat = true;
    }

    void FindClosestEnemy()
    {
        // Encuentra todos los robots en la escena
        RobotCombat[] robots = FindObjectsOfType<RobotCombat>();
        float closestDistance = Mathf.Infinity;
        targetEnemy = null;

        foreach (RobotCombat robot in robots)
        {
            // Ignora robots que no estén vivos o que sean del mismo equipo
            if (!robot.IsAlive || robot.IsPlayerRobot == IsPlayerRobot)
                continue;

            float distance = Vector3.Distance(transform.position, robot.transform.position);
            if (distance < closestDistance && distance <= detectionRange)
            {
                closestDistance = distance;
                targetEnemy = robot.transform;
            }
        }
    }

    void AttackTarget()
    {
        if (targetEnemy == null)
            return;

        // Dispara proyectiles a intervalos definidos
        if (!IsInvoking("ShootProjectile"))
        {
            InvokeRepeating("ShootProjectile", 0f, fireRate);
        }
    }

    void ShootProjectile()
    {
        if (targetEnemy == null || !targetEnemy.GetComponent<RobotCombat>().IsAlive)
        {
            CancelInvoke("ShootProjectile");
            return;
        }

        // Instancia proyectiles y envía daño aumentado por las estadísticas
        int totalAttack = baseAttack + robotStats.attack;
        photonView.RPC("ShootProjectileRPC", RpcTarget.All, firePoint.position, targetEnemy.GetComponent<PhotonView>().ViewID, totalAttack);
    }

    [PunRPC]
    void ShootProjectileRPC(Vector3 firePosition, int targetViewID, int attackDamage)
    {
        GameObject projectile = Instantiate(projectilePrefab, firePosition, Quaternion.identity);
        Projectile projectileScript = projectile.GetComponent<Projectile>();

        PhotonView targetPhotonView = PhotonView.Find(targetViewID);
        if (targetPhotonView != null)
        {
            Transform targetTransform = targetPhotonView.transform;
            projectileScript.SetTarget(targetTransform, attackDamage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (!photonView.IsMine) return;

        if (robotStats.shield <= 0)
        {
            int reducedDamage = Mathf.Max(damage - robotStats.defense, 1);
            currentHealth -= reducedDamage;
        }
        else
        {
            currentShield -= damage;
        }

        photonView.RPC("UpdateHealthRPC", RpcTarget.All, currentHealth);
        photonView.RPC("UpdateShield", RpcTarget.All, currentShield);

        if (currentHealth <= 0 && IsAlive)
        {
            IsAlive = false;

            CancelInvoke("ShootProjectile");
            photonView.RPC("OnDeathRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    void UpdateHealthRPC(int updatedHealth)
    {

        currentHealth = updatedHealth;
        healthBar.UpdateHealthBar(currentHealth);
        if (currentHealth <= 0 && IsAlive)
        {
            IsAlive = false;
        }
    }
    [PunRPC]
    void UpdateShield(int updatedShield)
    {
        currentShield = updatedShield;
        if (currentShield <= 0)
        {
            shield.SetActive(false);
        }
    }


    public void StartShield()
    {
        shield.SetActive(true);
    }

    [PunRPC]
    void OnDeathRPC()
    {
        IsAlive = false;
        CancelInvoke("ShootProjectile");
        // Opcional: Agregar efectos de muerte
    }
}
