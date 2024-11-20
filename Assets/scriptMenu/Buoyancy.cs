using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buoyancy : MonoBehaviour
{

    public float floatHeight = 2.0f;     // Chiều cao nổi của tàu
    public float bounceDamp = 0.05f;     // Giảm chấn dao động
    public float buoyancyFactor = 1.0f;  // Độ mạnh của sức nổi
    public Transform waterSurface; // Kéo đối tượng nước vào đây trong Inspector
    private float waterLevel;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (waterSurface != null)
        {
            waterLevel = waterSurface.position.y;
        }
        else
        {
            Debug.LogWarning("Water surface object not assigned.");
        }
    }

    void FixedUpdate()
    {
        Vector3 pos = transform.position;
        float waveHeight = GetWaveHeight(pos);

        if (pos.y < waveHeight) // Kiểm tra xem tàu có ở dưới mặt nước hay không
        {
            float buoyancy = (waveHeight - pos.y) * buoyancyFactor;
            Vector3 uplift = Vector3.up * (buoyancy - rb.velocity.y * bounceDamp);
            rb.AddForceAtPosition(uplift, transform.position, ForceMode.Acceleration);
        }
    }

    // Hàm tính độ cao của sóng nước tại vị trí của tàu
    float GetWaveHeight(Vector3 pos)
    {
        // Đây là hàm giả lập sóng, bạn có thể thay đổi cho phù hợp với shader nước của bạn
        return waterLevel + Mathf.Sin(Time.time + pos.x) * floatHeight;
    }
}
