using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject settingsCanvas;

    public void PlayGame()
    {
        SceneManager.LoadScene("Lobby");
    }
    public void Settings()
    {
        settingsCanvas.SetActive(true); 
    }

    public void CloseSettings()
    {
        settingsCanvas.SetActive(false); 
    }
}

