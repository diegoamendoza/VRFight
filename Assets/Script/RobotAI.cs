using Photon.Pun;
using System.Collections;
using UnityEngine;

public class RobotAI : MonoBehaviourPunCallbacks, IPunObservable
{
    public RobotStats stats;
    public LayerMask enemyLayer;

    private Transform target;
    private float currentHealth;

    private PhotonView photonView;

    private void Start()
    {
        currentHealth = stats.health;
        photonView = GetComponent<PhotonView>();

        if (photonView.IsMine)
        {
            StartCoroutine(SearchForTarget());
        }
    }

    //private void Update()
    //{
    //    if (!photonView.IsMine) return; // Solo controlar los robots que son del jugador local

    //    if (target != null)
    //    {
    //        float distanceToTarget = Vector3.Distance(transform.position, target.position);

    //        if (distanceToTarget <= stats.attackRange)
    //        {
    //            Attack(target.GetComponent<RobotAI>());
    //        }
    //        else
    //        {
    //            MoveTowardsTarget();
    //        }
    //    }
    //}

    private IEnumerator SearchForTarget()
    {
        while (true)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 10f, enemyLayer);
            float closestDistance = Mathf.Infinity;
            Transform closestTarget = null;

            foreach (Collider hit in hits)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance && hit.transform != this.transform)
                {
                    closestDistance = distance;
                    closestTarget = hit.transform;
                }
            }

            target = closestTarget;
            yield return new WaitForSeconds(1f);
        }
    }

    //private void MoveTowardsTarget()
    //{
    //    Vector3 direction = (target.position - transform.position).normalized;
    //    transform.position += direction * stats.moveSpeed * Time.deltaTime;
    //}

    //private void Attack(RobotAI enemy)
    //{
    //    bool isCritical = Random.Range(0f, 100f) < stats.criticalChance;
    //    int damage = isCritical ? Mathf.RoundToInt(stats.attack * stats.criticalDamageMultiplier) : stats.attack;

    //    int finalDamage = Mathf.Max(0, damage - enemy.stats.defense);
    //    enemy.TakeDamage(finalDamage);

    //    photonView.RPC("TakeDamage", RpcTarget.AllBuffered, finalDamage); // Sincroniza el daño en todos los jugadores
    //}

    [PunRPC]
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(stats.robotName + " recibió " + damage + " de daño. Vida restante: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(stats.robotName + " ha sido destruido.");
        PhotonNetwork.Destroy(gameObject); // Sincroniza la destrucción del robot
    }

    // Sincronizar la posición del robot y su salud con la red
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(currentHealth);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            currentHealth = (float)stream.ReceiveNext();
        }
    }
}
