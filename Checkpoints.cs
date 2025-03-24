using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    public Transform respawnPoint; 
    public GameManager gameManager; 
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && Vector2.Distance(transform.position, respawnPoint.position) < 2f)
        {
            Rest();
        }
    }
    private void Rest()
    {
        Debug.Log("🛌 Repos au feu de camp !");
        PlayerController player = FindFirstObjectByType<PlayerController>();

        if (player != null)
        {
            player.currentHealth = player.maxHealth;
            player.healthBar.SetHealth(player.currentHealth);
            player.currentPotions = player.maxPotions;
            Debug.Log("✨ Vie et potions restaurées !");
        }
        
        if (gameManager != null)
        {
            gameManager.RespawnEnemies();
        }
    }
    
    public void OnPlayerRespawn()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.transform.position = player.lastCheckpoint.position;
            if (gameManager != null)
            {
                gameManager.RespawnEnemies();
            }
        }
    }
}
