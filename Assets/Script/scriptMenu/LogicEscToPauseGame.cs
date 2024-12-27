using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class LogicEscToPauseGame : MonoBehaviour
{
    public GameObject PauseMenu;

    private void Start()
    {
        // Đảm bảo chuột được khóa và ẩn khi bắt đầu
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Bật hoặc tắt PauseMenu
            PauseMenu.SetActive(!PauseMenu.activeSelf);

            // Kiểm soát trạng thái chuột dựa trên trạng thái của PauseMenu
            UpdateCursorState();
        }
    }

    public void ContinueBT()
    {
        PauseMenu?.SetActive(false);
        UpdateCursorState(); // Cập nhật trạng thái chuột khi tiếp tục
    }

    public void QuitToMenu()
    {
        if (PhotonNetwork.InRoom) // Kiểm tra nếu đang trong phòng
        {
            PhotonNetwork.LeaveRoom(); // Rời phòng trước
            StartCoroutine(WaitForLeaveRoomAndDisconnect());
        }
        else if (PhotonNetwork.InLobby) // Kiểm tra nếu đang ở lobby
        {
            PhotonNetwork.LeaveLobby(); // Rời lobby trước
            StartCoroutine(WaitForLeaveLobbyAndDisconnect());
        }
        else if (PhotonNetwork.IsConnected) // Nếu chỉ kết nối server
        {
            PhotonNetwork.Disconnect(); // Ngắt kết nối ngay
            StartCoroutine(WaitForDisconnect());
        }
        else
        {
            // Nếu không kết nối với Photon, chuyển ngay sang Menu
            SceneManager.LoadScene("Menu");
        }
    }

    private IEnumerator WaitForLeaveRoomAndDisconnect()
    {
        // Đợi thoát phòng hoàn tất
        while (PhotonNetwork.InRoom)
        {
            yield return null;
        }

        // Ngắt kết nối sau khi rời phòng
        PhotonNetwork.Disconnect();
        StartCoroutine(WaitForDisconnect());
    }

    private IEnumerator WaitForLeaveLobbyAndDisconnect()
    {
        // Đợi thoát lobby hoàn tất
        while (PhotonNetwork.InLobby)
        {
            yield return null;
        }

        // Ngắt kết nối sau khi rời lobby
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

        // Chuyển sang Menu Scene
        SceneManager.LoadScene("Menu");
    }

    // Cập nhật trạng thái chuột dựa trên trạng thái PauseMenu
    private void UpdateCursorState()
    {
        if (PauseMenu.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None; // Bỏ khóa chuột
            Cursor.visible = true;                 // Hiển thị chuột
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // Khóa chuột
            Cursor.visible = false;                  // Ẩn chuột
        }
    }
}
