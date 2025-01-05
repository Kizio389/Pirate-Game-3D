using UnityEngine;
using Photon.Pun;

public class Movement : MonoBehaviourPun
{
    private CharacterController characterController;
    private Animator animator_Player;

    [SerializeField] private float SpeedToWalk = 1f;
    [SerializeField] private float SpeedToRun = 3f;
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

    [PunRPC]
    void SyncPlayerState(Vector3 position, Quaternion rotation)
    {
        // Đồng bộ vị trí và hướng quay với các người chơi khác
        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 10);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10);
    }
}
