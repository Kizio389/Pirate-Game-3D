using UnityEngine;
using Photon.Pun;

public class Movement : MonoBehaviourPun
{
    private CharacterController characterController;
    private Animator animator_Player;

    [SerializeField] private float SpeedToWalk = 1f;
    [SerializeField] private float SpeedToRun = 3f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -9.81f;
    private Vector3 velocity;
    private Transform cameraTransform;

    private float currentStamina;
    private float maxStamina;
    [SerializeField] private float staminaRegenRate = 5f;
    [SerializeField] private float staminaDrainRate = 10f;

    private bool isRunning = false;

  
    bool wasMoving = false;

    void Awake()
    {
        // Lấy dữ liệu stamina từ PlayerPrefsManager
        currentStamina = PlayerPrefsManager.GetStamina();
        maxStamina = PlayerPrefsManager.GetMaxStamina();

        // Lấy các component cần thiết
        characterController = GetComponent<CharacterController>();
        animator_Player = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        bool isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
        if (isMoving != wasMoving)
        {
            photonView.RPC("SyncPlayerState", RpcTarget.Others, transform.position, transform.rotation);
            wasMoving = isMoving;
        }
        HandleJump();
        HandleMovement();
    }
    [PunRPC]
    void HandleMovement()
    {
        // Lấy input từ bàn phím
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Tính hướng di chuyển dựa trên camera
        Vector3 moveDirection = cameraTransform.right * horizontal + cameraTransform.forward * vertical;
        moveDirection.y = 0;

        // Kiểm tra điều kiện chạy
        bool isMoving = horizontal != 0 || vertical != 0;
        bool canRun = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0 && isMoving;

        // Cập nhật tốc độ và di chuyển
        float speed = canRun ? SpeedToRun : SpeedToWalk;
        characterController.Move(moveDirection.normalized * speed * Time.deltaTime);

        // Quản lý stamina
        if (canRun)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Max(currentStamina, 0); // Đảm bảo stamina không âm
            isRunning = true;
        }
        else
        {
            if (currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Min(currentStamina, maxStamina); // Đảm bảo stamina không vượt quá giới hạn
            }
            isRunning = false;
        }
        

        // Cập nhật trạng thái vào PlayerPrefs
        PlayerPrefsManager.SetStamina(currentStamina);

        // Cập nhật animator
        animator_Player.SetBool("IsRunning", isRunning);
        animator_Player.SetFloat("Horizontal", horizontal);
        animator_Player.SetFloat("Vertical", vertical);
    }
    void HandleJump()
    {
        // Kiểm tra xem nhân vật có đang chạm đất không
        if (characterController.isGrounded)
        {
            // Đặt vận tốc trục Y về 0 khi chạm đất
            velocity.y = 0;

            // Nếu trước đó nhân vật không ở trạng thái chạm đất, bật trigger "Lander"
            if (!animator_Player.GetBool("Lander"))
            {
                animator_Player.SetTrigger("Lander");
            }

            // Kiểm tra phím nhảy
            if (Input.GetButtonDown("Jump")) // Phím nhảy mặc định là Space
            {
                animator_Player.ResetTrigger("Lander"); // Đảm bảo không bị xung đột trigger
                animator_Player.SetTrigger("IsJump"); // Kích hoạt hoạt ảnh nhảy
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity); // Tính vận tốc nhảy
            }
        }
        else
        {
            // Nếu nhân vật đang rơi, không ở trạng thái chạm đất
            animator_Player.ResetTrigger("Lander");
        }

        // Thêm trọng lực
        velocity.y += gravity * Time.deltaTime;

        // Áp dụng di chuyển
        characterController.Move(velocity * Time.deltaTime);
    }
    [PunRPC]
    void SyncPlayerState(Vector3 position, Quaternion rotation)
    {
        // Đồng bộ vị trí và hướng quay với các người chơi khác
        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 10);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10);
    }
}
