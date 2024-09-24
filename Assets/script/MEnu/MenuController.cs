using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // Hàm này sẽ được gọi khi nhấn nút "Play Game"
    public void PlayGame()
    {
        SceneManager.LoadScene("Lobby");
    }
}

