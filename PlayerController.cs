using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    private PlayerController player;
    public float maxHealth = 100f;
    public float currentHealth;
    public float maxMagic = 100f;
    public float currentMagic;
    public float maxStamina = 100f;
    public float currentStamina;
    public HealthBar healthBar;
    public MagicBar magicBar;
    public StaminaBar staminaBar;
    [SerializeField] float speed = 10f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] private Rigidbody2D rb;
    public Transform feetLeft;
    public Transform feetRight;
    [SerializeField] private LayerMask groundLayer;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public bool isGrounded;
    public bool isJumping;
    public Vector2 movement;
    public InputActionAsset actions;
    public GameManager GM;
    public UIManager UIManager;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public float attackRate = 2f;
    private float nextAttackTime = 0f;
    public LayerMask enemyLayer;
    public GameObject PanelParent;
    public TextMeshProUGUI scoreText;
    public UiDeathScreen uiDeathScreen;
    public AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip run1Sound;
    public AudioClip run2Sound;
    public AudioClip run3Sound;
    public AudioClip run4Sound;
    private Coroutine runningSoundCoroutine;
    public AudioClip landingSound;
    public AudioClip[] attackSounds;
    public AudioClip deathSound;
    public int maxPotions = 3;
    public int currentPotions;
    public float healAmount = 30f;
    public AudioClip drinkPotionSound;
    public Transform lastCheckpoint; 
    public float rollSpeed = 15f;  
    public float rollDuration = 0.3f;  
    public float rollCost = 20f; 
    private bool isRolling = false;
    [SerializeField] private float staminaRegenRate = 5f;  
    [SerializeField] private float staminaRegenDelay = 2f; 
    private bool isRegeneratingStamina = false;
    public int score = 0;
    void Start()
    {
        PanelParent.SetActive(false);
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        
        if (staminaBar == null)
            staminaBar = FindFirstObjectByType<StaminaBar>();

        if (staminaBar != null)
            staminaBar.SetMaxStamina(maxStamina);
        
        if (healthBar == null)
            healthBar = FindFirstObjectByType<HealthBar>();

        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);
        
        player = FindFirstObjectByType<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        GM = FindFirstObjectByType<GameManager>();
        UIManager = FindFirstObjectByType<UIManager>();
        scoreText = GetComponentInChildren<TextMeshProUGUI>();
        currentPotions = maxPotions;
        actions.Enable();
        InputActionMap m = actions.FindActionMap("Gameplay");
        InputAction move = m.FindAction("Move");
        InputAction jump = m.FindAction("Jump");
        InputAction attack = m.FindAction("Attack");
        InputAction roll = m.FindAction("Roll");

        move.started += MoveCallback;
        move.performed += MoveCallback;
        move.canceled += MoveCallback;
        
        jump.started += JumpCallback;
        
        attack.started += AttackCallback;
        
        roll.started += RollCallback;
    }

    public void TakeDamage(float damage)
    {
        if (isRolling) return;
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            DeathAnimation();
        }
    }
    
    public void AddScore(int points)
    {
        score += points;
        UIManager.refreshScore();
    }
    
    
    private void TakeStamina(float stamina)
    {
        currentStamina -= stamina;
        staminaBar.SetStamina(currentStamina);
        
        if (isRegeneratingStamina)
        {
            StopCoroutine(RegenerateStamina());
            isRegeneratingStamina = false;
        }

        StartCoroutine(StartStaminaRegenTimer());
    }
    
    private IEnumerator StartStaminaRegenTimer()
    {
        yield return new WaitForSeconds(staminaRegenDelay);
        if (!isRolling) 
        {
            StartCoroutine(RegenerateStamina());
        }
    }
    
    private IEnumerator RegenerateStamina()
    {
        isRegeneratingStamina = true;

        while (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            staminaBar.SetStamina(currentStamina);
            yield return null;
        }

        currentStamina = maxStamina;
        staminaBar.SetStamina(currentStamina);
        isRegeneratingStamina = false;
    }
    
    private void AttackCallback(InputAction.CallbackContext obj)
    {

        if (Time.time >= nextAttackTime && isGrounded)
        {
            animator.SetBool("IsAttacking", true);
            
            if (attackSounds.Length > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, attackSounds.Length);
                audioSource.PlayOneShot(attackSounds[randomIndex]);
            }

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<EnemyBehaviour>().TakeDamage(20f);
            }

            StartCoroutine(ResetAttackTrigger());

            nextAttackTime = Time.time + 1f / attackRate;
        }
    }


    public void UsePotion()
    {
        if (currentPotions > 0)
        {
            currentPotions--;
            currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
            healthBar.SetHealth(currentHealth);
            
            if (drinkPotionSound != null)
                audioSource.PlayOneShot(drinkPotionSound);

            Debug.Log($"Potion utilisée, potions restantes : {currentPotions}");
        }
        else
        {
            Debug.Log("Plus de potions");
        }
    }
    
    private void RollCallback(InputAction.CallbackContext obj)
    {
        Roll();
    }


    
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }


    private IEnumerator ResetAttackTrigger()
    {
        yield return new WaitForSeconds(0.55f);
        animator.SetBool("IsAttacking", false);
    }

    
    
    private void JumpCallback(InputAction.CallbackContext obj)
    {
        if (rb == null)
        {
            Debug.LogError("🚨 Rigidbody2D est NULL au moment du saut !");
            return;
        }
        if (isGrounded)
        {
            audioSource.PlayOneShot(jumpSound);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            animator.SetBool("IsJumping", true);
            animator.SetBool("IsFalling", false);
        }
            
    }
    
    private void Roll()
    {
        if (isRolling || currentStamina < rollCost || !isGrounded)
            return;

        TakeStamina(rollCost);
        isRolling = true;
        animator.SetTrigger("Roll");
        
        gameObject.layer = LayerMask.NameToLayer("PlayerRolling");

        Vector2 rollDirection = movement.x != 0 ? new Vector2(movement.x, 0).normalized : (spriteRenderer.flipX ? Vector2.left : Vector2.right);
        rb.linearVelocity = rollDirection * rollSpeed;

        StartCoroutine(EndRoll());
    }

    
    private IEnumerator EndRoll()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
    
        isRolling = false;
        rb.linearVelocity = Vector2.zero; 
        animator.ResetTrigger("Roll"); 
        animator.SetTrigger("Respawn"); 
        
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    private void Flip(float _velocity)
    {
        if (_velocity > 0.1f)
        {
            spriteRenderer.flipX = false;
        }
        else if (_velocity < -0.1f)
        {
            spriteRenderer.flipX = true;
        }
    }

    public void DeathAnimation()
    {
        if (currentHealth <= 0)
        {
            animator.SetTrigger("Die");
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
            actions.Disable();
            this.enabled = false;
            uiDeathScreen.ShowGameOver();
            audioSource.PlayOneShot(deathSound);
            
            Invoke(nameof(Respawn), 7f);
        }
    }
    
    private void Respawn()
    {
        if (lastCheckpoint != null)
        {
            uiDeathScreen.HideGameOver();
            transform.position = lastCheckpoint.position;
            currentHealth = maxHealth;
            currentPotions = maxPotions;
            healthBar.SetHealth(currentHealth);

            rb.simulated = true;
            actions.Enable();
            this.enabled = true;
            animator.SetTrigger("Respawn");
            animator.SetFloat("Speed", 0);
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false);

            Debug.Log("Réapparition au dernier checkpoint");
        }
        else
        {
            Debug.Log("Aucun checkpoint défini");
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (!isGrounded)
            {
                audioSource.PlayOneShot(landingSound);
            }
            
            isGrounded = true;
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false);
        }
    }


    private void MoveCallback(InputAction.CallbackContext obj)
    {
        movement = obj.ReadValue<Vector2>();

        if (movement.x != 0 && isGrounded)
        {
            if (runningSoundCoroutine == null)
            {
                runningSoundCoroutine = StartCoroutine(PlayRunningSounds());
            }
        }
        else
        {
            if (runningSoundCoroutine != null)
            {
                StopCoroutine(runningSoundCoroutine);
                runningSoundCoroutine = null;
            }
        }
    }

    private IEnumerator PlayRunningSounds()
    {
        AudioClip[] runSounds = { run1Sound, run2Sound, run3Sound, run4Sound };
        int index = 0;

        while (movement.x != 0 && isGrounded)
        {
            audioSource.PlayOneShot(runSounds[index]);
            index = (index + 1) % runSounds.Length;
            yield return new WaitForSeconds(0.3f); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (isRolling) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            UsePotion();
        }


        rb.linearVelocity = new Vector2(movement.x * speed, rb.linearVelocity.y);
              
    }


    
    void FixedUpdate()
    {
        Flip(rb.linearVelocityX);
        float characterVelocity = Mathf.Abs(rb.linearVelocityX);
        animator.SetFloat("Speed", characterVelocity);
        
        if (rb.linearVelocityY > 0.1f)
        {
            animator.SetBool("IsJumping", true);
            animator.SetBool("IsFalling", false);
        }
        else if (rb.linearVelocityY < -0.1f)
        {
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", true);
            
        }
        else if (isGrounded)
        {
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false);
        }
        
        if (!isGrounded && runningSoundCoroutine != null)
        {
            StopCoroutine(runningSoundCoroutine);
            runningSoundCoroutine = null;
        }
    }

}