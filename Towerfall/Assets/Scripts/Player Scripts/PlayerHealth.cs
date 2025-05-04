using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 100;
    public Transform respawnPoint; // Assign this in the Inspector
    public float respawnDelay = 2f; // Time before respawn

    private Animator animator;
    private MovementController controller;

    public bool isAlive = true;

    void Awake()
    {
        animator = GetComponent<Animator>(); // Ensure this is correctly set up
        controller = GetComponent<MovementController>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        //animator.SetTrigger("takeDamage");
        Debug.Log("Player took damage: " + damage);

        if (health <= 0)
        {
            Die();
        }

    }

    private void Die()
    {
        isAlive = false;
        animator.SetTrigger("die");
        if (controller != null)
            controller.enabled = false; // Disable player input
        Invoke(nameof(Respawn), respawnDelay);
    }

    private void Respawn()
    {
        health = 100;
        isAlive = true;
        animator.SetTrigger("respawn");
        transform.position = respawnPoint.position;

        if (controller != null)
            controller.enabled = true; // Re-enable input

        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        if (enemy != null && enemy.TryGetComponent<EnemyAI>(out var enemyAI))
        {
            enemyAI.ResetHealth();
            Debug.Log("Enemy health reset");
        }

        Debug.Log("Player respawned!");
    }
}
