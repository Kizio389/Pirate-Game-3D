using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float rayLength = 5f;          // Độ dài tia Ray
    public LayerMask layerMask;          // Các Layer cần kiểm tra
    public float rotationSpeed = 30f;    // Tốc độ xoay (độ/giây)

    private bool isRotating = false;     // Trạng thái đối tượng đang xoay
    public float coneAngle = 45f;         // Góc mở của hình nón (tính từ tâm)
    public int rayCount = 10;            // Số tia phát ra
    
    public Color rayColor = Color.yellow;// Màu của tia

    [SerializeField] public float Heath = 200f;
    [SerializeField] public float Speed;

    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * Speed * Time.deltaTime;
        Rotate();
    }

    void Rotate()
    {
        for (int i = 0; i < rayCount; i++)
        {
            // Tính góc của tia hiện tại trong hình nón
            float currentAngle = Mathf.Lerp(-coneAngle / 2, coneAngle / 2, (float)i / (rayCount - 1));

            // Xoay hướng tia dựa trên góc
            Vector3 rayDirection = Quaternion.Euler(0, currentAngle, 0) * transform.forward;

            // Tạo Ray
            Ray ray = new Ray(transform.position, rayDirection);
            RaycastHit hit;

            // Nếu Raycast chạm Collider và không đang xoay
            if (Physics.Raycast(ray, out hit, rayLength, layerMask) && !isRotating)
            {
                //Debug.Log("Phát hiện va chạm với: " + hit.collider.name);
                isRotating = true; // Bắt đầu xoay
            }

            // Nếu đang xoay, kiểm tra lại
            if (isRotating)
            {
                // Xoay đối tượng
                transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

                // Kiểm tra lại xem đã thoát khỏi va chạm chưa
                if (!Physics.Raycast(ray, out hit, rayLength, layerMask))
                {
                    //Debug.Log("Không còn va chạm!");
                    isRotating = false; // Dừng xoay
                }
            }

            // Vẽ tia để debug
            Debug.DrawRay(transform.position, rayDirection * rayLength, rayColor);
        }
    }

    public void TakeDamege(float damege)
    {
        Heath -= damege;
        if(Heath<=0)
        {
            Heath = 0;
            isDeath();
        }
    }

    void isDeath()
    {
        animator.SetTrigger("isDeath");
    }
}