using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] allEnemies; 
    private bool enemiesRespawned = false;

    
    public void RespawnEnemies()
    {
        StartCoroutine(ResetEnemiesCoroutine());
    }

    private IEnumerator ResetEnemiesCoroutine()
    {
        
        yield return new WaitForSeconds(1f);
        foreach (GameObject enemy in allEnemies)
        {
            if (enemy != null)
            {
                enemy.SetActive(true);
                
                EnemyBehaviour enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();
                if (enemyBehaviour != null)
                {
                    enemyBehaviour.currentHealth = enemyBehaviour.maxHealth; 
                    enemyBehaviour.animator.SetBool("IsDead", false);
                    enemyBehaviour.rb.simulated = true; 
                    enemyBehaviour.enabled = true; 
                }
            }
        }

        enemiesRespawned = true;
    }
    
    public void DisableAllEnemies()
    {
        foreach (GameObject enemy in allEnemies)
        {
            if (enemy != null)
            {
                enemy.SetActive(false);
            }
        }
    }
    
    public void KillAllEnemies()
    {
        foreach (GameObject enemy in allEnemies)
        {
            if (enemy != null)
            {
                EnemyBehaviour enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();
                if (enemyBehaviour != null)
                {
                    enemyBehaviour.Die();
                }
            }
        }
    }

    private void Start()
    {
        // R
    }
}