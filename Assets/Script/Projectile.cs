using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    private Transform target;
    private int damage;

    public void SetTarget(Transform target, int damage)
    {
        this.target = target;
        this.damage = damage;
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); // Destruye el proyectil si no hay objetivo
            return;
        }

        // Mueve el proyectil hacia el objetivo
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Comprueba si el proyectil alcanza el objetivo
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        RobotCombat enemy = target.GetComponent<RobotCombat>();
        if (enemy != null)
        {
            PhotonView enemyView = enemy.GetComponent<PhotonView>();
            if (enemyView != null && enemyView.IsMine)
            {
                enemy.TakeDamage(damage);
            }
        }

        Destroy(gameObject); // Destruye el proyectil después del impacto
    }
}
