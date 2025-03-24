using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private PlayerController player;
    public GameObject PlayButton;
    public GameObject SettingsButton;
    public GameObject QuitButton;
    public EventSystem EventSystem;
    public GameManager GameManager;
    public TextMeshProUGUI scoreText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        scoreText = GetComponentInChildren<TextMeshProUGUI>();
        EventSystem = EventSystem.current;
        GameManager = FindFirstObjectByType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.currentSelectedGameObject == null)
        {
            EventSystem.SetSelectedGameObject(PlayButton);
        }
        scoreText.text = "Souls: " + player.score;
    }

    public void OnClickPlayButton()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnClickSettingsButton()
    {
        SceneManager.LoadScene("Settings");
    }

    public void OnClickQuitButton()
    {
        Application.Quit();
    }
    
    public void refreshScore()
    {
        scoreText.text = "Souls: " + player.score;
    }

    
}
