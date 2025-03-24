using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiDeathScreen : MonoBehaviour
{
    public CanvasGroup gameOverUI;
    public TextMeshProUGUI gameOverText;
    public GameObject deathScreen;

    private void Start()
    {
        gameOverUI.alpha = 0;
        gameOverUI.interactable = false;
        gameOverUI.blocksRaycasts = false;
    }

    public void ShowGameOver()
    {
        deathScreen.SetActive(true); 
        StartCoroutine(FadeInGameOver());
    }
    
    public void HideGameOver()
    {
        deathScreen.SetActive(false); 
    }

    private IEnumerator FadeInGameOver()
    {
        float duration = 5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            gameOverUI.alpha = Mathf.Lerp(0, 1, elapsed / duration);
            yield return null;
        }

        gameOverUI.interactable = true;
        gameOverUI.blocksRaycasts = true;
    }
}
