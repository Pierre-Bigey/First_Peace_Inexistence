using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    public int health = 100;

    void OnParticleCollision(GameObject other)
    {
        Debug.Log("Particle Collision!");
        if (other.GetComponent<WeaponScript>() != null)
        {
            TakeDamage(10);
        }
    }

    void TakeDamage(int damage)
    {
        Debug.Log("Damage taken!");
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}