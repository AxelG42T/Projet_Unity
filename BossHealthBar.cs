using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public Slider healthSlider; 

    
    public void Show()
    {
        gameObject.SetActive(true); 
    }

    
    public void Hide()
    {
        gameObject.SetActive(false); 
    }

    
    public void SetHealth(float currentHealth, float maxHealth)
    {
        healthSlider.value = currentHealth;
        healthSlider.maxValue = maxHealth;
    }
}