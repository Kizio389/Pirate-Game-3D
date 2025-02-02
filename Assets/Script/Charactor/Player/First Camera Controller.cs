﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class FirstCameraController : MonoBehaviourPun
{
    [SerializeField] public float mouseSensitivity = 100f; // Độ nhạy chuột
    [SerializeField] public Transform playerBody;         // Player Body để xoay
    private float xRotation = 0f;
    private LogicEscToPauseGame logicEscToPauseGame;

    void Start()
    {
        if (!photonView.IsMine)
        {
            gameObject.SetActive(false); // Vô hiệu hóa camera cho các người chơi khác
            return;
        }

        Cursor.lockState = CursorLockMode.Locked; // Khóa con trỏ chuột vào màn hình
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (PauseMenuActive()) return; // Dừng xoay camera khi PauseMenu đang bật
        if(!InventorySystem2.Instance.isOpen )
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }

        
    }

    // Hàm kiểm tra trạng thái PauseMenu
     private bool PauseMenuActive()
    {
        return logicEscToPauseGame != null && logicEscToPauseGame.PauseMenu.activeSelf;
    }
}