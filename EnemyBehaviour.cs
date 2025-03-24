using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBehaviour : MonoBehaviour
{
    public float maxHealth = 50f;
    public float currentHealth;
    public float speed = 5f;
    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer graphics;
    public PlayerController player;
    public int scoreValue = 10;
    public float detectionRange = 30f; 
    public float attackRange = 7f; 
    public float attackDamage = 10f; 
    public float attackCooldown = 1.5f; 
    private bool isPlayerDetected = false;
    private bool canAttack = true;
    
    [SerializeField] private BossHealthBar bossHealthBar;
    void Start()
    {
        bossHealthBar = FindFirstObjectByType<BossHealthBar>();
        if (bossHealthBar != null)
        {
            bossHealthBar.Hide();
        }
        
        currentHealth = maxHealth;
        player = FindFirstObjectByType<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        Debug.Log("Distance au joueur: " + distanceToPlayer);
        
        if (distanceToPlayer <= detectionRange)
        {
            isPlayerDetected = true;
            
            if (bossHealthBar != null && !bossHealthBar.gameObject.activeSelf)
            {
                bossHealthBar.Show(); 
                bossHealthBar.SetHealth(currentHealth, maxHealth);
            }

            if (distanceToPlayer <= attackRange)
            {
                rb.linearVelocity = Vector2.zero;
                animator.SetBool("IsWalking", false);
                if (canAttack)
                {
                    Attack();
                }
            }
            else
            {
                Vector2 direction = (player.transform.position - transform.position).normalized;
                rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocityY);
                animator.SetBool("IsWalking", true);
                graphics.flipX = direction.x < 0;
            }
        }
        else
        {
            isPlayerDetected = false;
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsWalking", false);
        }
    }
    private void Attack()
    {
        animator.SetTrigger("Attack");
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= attackRange)
        {
            if (player != null)
            {
                player.TakeDamage(attackDamage);
            }
        }
        canAttack = false;
        StartCoroutine(ResetAttackCooldown());
    }
    
    private IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        if (bossHealthBar != null)
        {
            bossHealthBar.SetHealth(currentHealth, maxHealth);
        }
        
        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Die()
    {
        if (player != null)
        {
            if (bossHealthBar != null)
            {
                bossHealthBar.Hide(); 
            }
            
            player.AddScore(scoreValue);
            animator.SetBool("IsDead", true);
            GetComponent<Collider2D>().enabled = false; 
            rb.simulated = false;  
            this.enabled = false; 

            StartCoroutine(DestroyAfterAnimation());
        }
    }
    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Death"));
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        gameObject.SetActive(false);
    }
    
    public void ResetEnemy()
    {
        currentHealth = maxHealth; 
        animator.SetBool("IsDead", false);  
        GetComponent<Collider2D>().enabled = true;  
        rb.simulated = true;  
        animator.SetBool("IsWalking", false);  
        canAttack = true;  
    }
}