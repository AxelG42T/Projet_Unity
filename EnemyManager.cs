using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private List<EnemyBehaviour> enemies = new List<EnemyBehaviour>();

    
    public void RespawnEnemies()
    {
        
        foreach (EnemyBehaviour enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.gameObject.SetActive(true);  
                enemy.ResetEnemy();  
            }
        }
    }
    
    private void Start()
    {
        //enemies.AddRange(FindObjectsOfType<EnemyBehaviour>());
    }
}