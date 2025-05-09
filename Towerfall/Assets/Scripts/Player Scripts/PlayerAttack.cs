using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 1.5f;  // How far the attack reaches
    public int attackDamage = 15;    // Damage per attack
    public float attackCooldown = 0.5f; // Time between attacks
    public LayerMask enemyLayer;      // Detects enemies

    private float lastAttackTime;
    private Animator animator;
    private PlayAudio playAudio;

    void Awake()
    {
        animator = GetComponent<Animator>();
        playAudio = GetComponent<PlayAudio>();
    }

    void Update()
    {
        // 1. Detect left mouse button click (left-click is Input.GetMouseButtonDown(0))
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();  // Calls the Attack function when left-click is pressed
            lastAttackTime = Time.time;  // Sets cooldown to prevent spamming
        }
    }

    private void Attack()
    {
        // Play an attack animation (Optional, you can use an Animator if you have one)
        animator.SetTrigger("attack");
        playAudio.PlaySpell();

        // Check for enemies in attack range
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.GetComponentInParent<BreakablePlatform>() is BreakablePlatform platform)
            {
                Debug.Log("Hit enemy");
                platform.TakeDamage(attackDamage);
            }
            if (enemy.GetComponentInParent<EnemyAI>() is EnemyAI health)
            {
                Debug.Log("Hit enemy");
                health.TakeDamage(attackDamage);
            }
        }
    }

    public void Victory()
    {
        animator.SetTrigger("victory");
    }

    // Draw attack range in Scene view (for debugging)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}