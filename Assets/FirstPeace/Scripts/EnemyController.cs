using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private float health;
    private EnemiesManager _enemiesManager;

    public void Initialize(float health, EnemiesManager enemiesManager)
    {
        this.health = health;
        _enemiesManager = enemiesManager;
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Particle Collision!");
        if (other.GetComponent<WeaponScript>() != null)
        {
            TakeDamage(other.GetComponent<WeaponScript>().damage);
        }
    }

    private void TakeDamage(float damage)
    {
        Debug.Log("Damage taken!");
        health -= damage;

        if (health <= 0)
        {
            KillEnemy();
        }
    }

    private void KillEnemy()
    {
        _enemiesManager.KilledEnemy(gameObject);
    }
}