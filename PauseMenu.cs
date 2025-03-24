using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool Ispaused = false;
    
    public GameObject pauseMenuUI;
    private PlayerController playerController; 
    private Animator playerAnimator;
    
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        playerAnimator = player.GetComponent<Animator>();
        pauseMenuUI.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Ispaused)
            {
                Resume();
            }
            else
            {
                Paused();
            }
        }
    }

    void Paused()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Ispaused = true;
        
        InputBlocker.BlockInputs();
        playerController.enabled = false;
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Ispaused = false;
        
        InputBlocker.UnblockInputs();
        playerController.enabled = true;
    }
    
    public static class InputBlocker
    {
        private static bool inputsBlocked = false;
    
        public static bool AreInputsBlocked()
        {
            return inputsBlocked;
        }
    
        public static void BlockInputs()
        {
            inputsBlocked = true;
        }
    
        public static void UnblockInputs()
        {
            inputsBlocked = false;
        }
    }

    public void LoadMainMenu()
    {
        Resume();
        SceneManager.LoadScene("Menu");
    }
    
    public void LoadSettings()
    {
        Resume();
        SceneManager.LoadScene("Settings");
    }
    
}
