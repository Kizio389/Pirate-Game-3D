using UnityEngine;
public class SunRotation : MonoBehaviour
{
    public float realTimeMinuteDuration = 60f; // Thời gian 1 phút trong game (giây ngoài đời thực)
    private float gameTime = 0f; // Thời gian trong game (tính bằng phút)

    private float xAngle = 0f; // Góc hiện tại của trục X
    private float yAngle = 0f; // Góc hiện tại của trục Y

    public float xSpeed = 0f;

    private bool increasing = true;
    void Update()
    {

        // Tăng góc X theo tốc độ
        if (increasing)
        {
            xAngle += xSpeed;
            if (xAngle >= 90f) // Khi đạt 90 độ, đổi hướng
            {
                xAngle = 90f;
                increasing = false;
            }
        }
        else
        {
            xAngle -= xSpeed;
            if (xAngle <= 0f) // Khi đạt 0 độ, đổi hướng
            {
                xAngle = 0f;
                increasing = true;
            }
        }
        // Tăng góc Y theo tốc độ
        yAngle = (gameTime / 1440f) * 360f;
        if (yAngle >= 360f)
        {
            yAngle = 0f; // Đặt về 0 khi đạt 360
        }
        gameTime += (Time.deltaTime / realTimeMinuteDuration) * 60f; // Chuyển đổi giây thực sang phút trong game
        if (gameTime >= 1440f) // 1440 phút = 24 giờ
        {
            gameTime = 0f; // Đặt lại về 0 khi qua ngày mới
        }

        int hours = Mathf.FloorToInt(gameTime / 60f);
        int minutes = Mathf.FloorToInt(gameTime % 60f);

        Debug.Log("Game Time: " + $"{hours:00}:{minutes:00}");

        // Cập nhật góc xoay đối tượng
        transform.rotation = Quaternion.Euler(xAngle, yAngle, 0f);
    }
}
