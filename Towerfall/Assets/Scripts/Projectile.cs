using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifeTime = 2f;
    public int damage = 20;

    void OnEnable()
    {
        Invoke(nameof(Deactivate), lifeTime);
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        CancelInvoke(); // stop leftover invokes
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<EnemyAI>(out var enemy))
        {
            enemy.TakeDamage(damage);
        }

        // Optional: also damage other types of objects

        Deactivate(); // Deactivate after hitting something
    }
}
