using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UISettingsManager : MonoBehaviour
{
    public GameObject ReturnButton;
    public EventSystem EventSystem;
    public GameManager GM;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventSystem = EventSystem.current;
        GM = FindFirstObjectByType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.currentSelectedGameObject == null)
        {
            EventSystem.SetSelectedGameObject(ReturnButton);
        }
    }

    public void OnClickReturnButton()
    {
        SceneManager.LoadScene("Menu");
    }
    
    /*public void OnVolumeChange(float volume)
    {
        GM.SetAudioVolume(volume);
    }*/
    
}