using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pauseForLobby : MonoBehaviour
{
    public GameObject PauseMenu;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenu.SetActive(!PauseMenu.activeSelf);
        }
    }
    public void ContinueBT()
    {
        PauseMenu?.SetActive(false);
    }

    public void QuitToMenu()
    {
        if (PhotonNetwork.InLobby) // Kiểm tra nếu đang ở trong lobby
        {
            PhotonNetwork.LeaveLobby(); // Thoát khỏi lobby trước
            StartCoroutine(WaitForLeaveLobbyAndDisconnect());
        }
        else if (PhotonNetwork.IsConnected) // Nếu không ở lobby nhưng vẫn kết nối server
        {
            PhotonNetwork.Disconnect(); // Ngắt kết nối
            StartCoroutine(WaitForDisconnect());
        }
        else
        {
            // Nếu chưa kết nối với Photon, chuyển ngay sang menu
            SceneManager.LoadScene("Menu");
        }
    }

    private IEnumerator WaitForLeaveLobbyAndDisconnect()
    {
        // Đợi thoát khỏi lobby hoàn tất
        while (PhotonNetwork.InLobby)
        {
            yield return null;
        }

        // Sau khi thoát lobby, ngắt kết nối
        PhotonNetwork.Disconnect();
        StartCoroutine(WaitForDisconnect());
    }

    private IEnumerator WaitForDisconnect()
    {
        // Đợi ngắt kết nối hoàn tất
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }

        // Khi đã ngắt kết nối, chuyển sang Menu Scene
        SceneManager.LoadScene("Menu");
    }
}
