using System;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
   private PlayerController player;
   private void Start()
   {
      player = FindFirstObjectByType<PlayerController>();
   }

   private void OnTriggerEnter2D(Collider2D collision)
   {
      if (collision.CompareTag("Player"))
      {
         player = collision.GetComponent<PlayerController>();
         if (player != null)
         {
            player.TakeDamage(player.maxHealth); 
         }
      }
   }
}
