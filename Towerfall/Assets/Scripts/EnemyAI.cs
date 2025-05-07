using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask groundLayer, playerLayer;

    public float health;
    public int damageAmount = 20;

    // Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public Animator playerAnimator;

    // Animator
    private Animator animator;
    private bool hasGoneMad = false;
    private PlayerHealth playerHealth;

    private PlayAudio playAudio;
    private bool isStarting = true;

    private void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        playAudio = GetComponent<PlayAudio>();
    }

    private void Start()
    {
        StartCoroutine(WaitForIntroAnimation());
        animator.SetTrigger("IdleBegin");
        playAudio.PlayRoarOne();
    }

    private IEnumerator WaitForIntroAnimation()
    {
        // Wait until "IdleBegin" state is no longer playing
        while (animator.GetCurrentAnimatorStateInfo(0).IsTag("IdleBegin"))
        {
            yield return null; // wait for next frame
        }

        // Wait until transition out finishes (optional but safer)
        while (animator.IsInTransition(0))
        {
            yield return null;
        }

        isStarting = false; // Enemy can now start behavior
    }

    private void Update()
    {
        if (isStarting)
            return;
        // Block all state changes if attack animation is playing
        // Get current state info
        var state = animator.GetCurrentAnimatorStateInfo(0);

        // Stop movement during attack or death animations
        bool isInAttackAnim = state.IsTag("Attack");
        bool isInDieAnim = state.IsTag("Die");
        bool isMadAnim = state.IsTag("IsMad");
        bool isBegin = state.IsTag("IdleBegin");

        agent.isStopped = isInAttackAnim || isInDieAnim || isMadAnim || isBegin;

        // If in attack or die animation, stop further AI logic
        if (agent.isStopped)
            return;

        if (!hasGoneMad && health <= 125)
        {
            animator.SetTrigger("isMad");
            playAudio.PlayRoarTwo();
            hasGoneMad = true;
        }

        if (playerHealth != null && !playerHealth.isAlive)
        {
            return;
        }

        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInSightRange && !playerInAttackRange)
            Patrolling();
        else if (playerInSightRange && !playerInAttackRange)
            ChasePlayer();
        else if (playerInAttackRange && playerInSightRange)
            AttackPlayer();
        else
            SetIdle(); // fallback
    }

    private void Patrolling()
    {
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", false);

        if (hasGoneMad)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", true);
        }

        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        // Only set animations if not attacking
        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            if (health <= 125)
            {
                animator.SetBool("isRunning", true);
                animator.SetBool("isWalking", false);
            }
            else
            {
                animator.SetBool("isWalking", true);
                animator.SetBool("isRunning", false);
            }
        }

        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetTrigger("Attack");
        playAudio.PlayAttack();

        agent.isStopped = true;
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks); // Reset cooldown
        }
    }

    public void DealDamage()
    {
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, attackRange, playerLayer);
        foreach (Collider col in hitPlayers)
        {
            if (col.GetComponentInParent<PlayerHealth>() is PlayerHealth health)
            {
                health.TakeDamage(damageAmount);
                playerAnimator.SetTrigger("takeDamage");
                Debug.Log("Hit " + col.name);
            }
            else
            {
                Debug.LogWarning("PlayerHealth not found on " + col.name);
            }
        }
    }

    public void ResetHealth()
    {
        health = 250; // assuming you have a maxHealth variable
        Debug.Log($"Enemy health reset!");
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetTrigger("takeDamage"); // changed to Trigger
        playAudio.PlayHurt();
        Debug.Log("Enemy took damage: " + damage);
        Debug.Log("Enemy health: " + health);

        if (health <= 0)
        {
            animator.SetTrigger("die");
            playAudio.PlayDeath();
            agent.isStopped = true;
            agent.ResetPath();
            //agent.enabled = false;
            //GetComponent<Collider>().enabled = false;
            //Invoke(nameof(DestroyEnemy), 2f);
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && player.TryGetComponent<PlayerAttack>(out var playerAttack))
            {
                playerAttack.Victory();
            }
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void SetIdle()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}