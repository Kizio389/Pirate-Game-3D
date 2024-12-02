using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstCameraController : MonoBehaviour
{
    [SerializeField] public float mouseSensitivity = 100f; // Độ nhạy của chuột
    [SerializeField] public Transform playerBody;         // Gắn Player Body để xoay theo trục Y
    private float xRotation = 0f;        // Biến lưu góc xoay theo trục X

    void Start()
    {
        // Khóa con trỏ chuột vào màn hình
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Lấy thông tin di chuyển chuột
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Tính toán góc xoay theo trục X
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Giới hạn góc nhìn lên xuống

        // Xoay camera theo trục X
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Xoay player theo trục Y
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
