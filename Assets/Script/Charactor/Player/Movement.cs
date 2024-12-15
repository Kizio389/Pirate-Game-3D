using Photon.Pun;
using UnityEngine;

public class Movement : MonoBehaviourPun
{
    private CharacterController characterController;
    private Animator animator_Player;

    [SerializeField] private float SpeedToWalk = 1f;
    [SerializeField] private float SpeedToRun = 3f;
    private bool isRun = false;

    private Transform cameraTransform; // Tham chiếu đến camera chính

    SingletonIndexPlayer DataPlayer;

    private void Awake()
    {
        animator_Player = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        DataPlayer = SingletonIndexPlayer.Instance;

        // Lấy tham chiếu đến camera chính
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (!photonView.IsMine) return; // Chỉ điều khiển nhân vật sở hữu

        HandleCameraRotation(); // Xoay camera theo chuột
        HandleMovement();       // Di chuyển nhân vật

        if (!isRun && DataPlayer.Stamina < DataPlayer.Max_Stamina)
        {
            DataPlayer.Stamina += 0.2f * Time.deltaTime;
            photonView.RPC("SyncStamina", RpcTarget.All, DataPlayer.Stamina);
        }
    }

    void HandleMovement()
    {
        animator_Player.SetBool("ToWalk", false);
        animator_Player.SetBool("ToRun", false);

        // Lấy input từ bàn phím
        float horizontal = Input.GetAxis("Horizontal"); // A (-1) và D (+1)
        float vertical = Input.GetAxis("Vertical");     // W (+1) và S (-1)

        // Tạo hướng di chuyển dựa trên camera
        Vector3 moveDirection = cameraTransform.right * horizontal + cameraTransform.forward * vertical;

        // Loại bỏ chuyển động theo trục Y
        moveDirection.y = 0;

        if (moveDirection.magnitude >= 0.1f)
        {
            float _Speed = SpeedToWalk;

            // Kiểm tra nếu người chơi nhấn Shift để chạy
            if (Input.GetKey(KeyCode.LeftShift) && DataPlayer.Stamina > 0)
            {
                isRun = true;
                DataPlayer.Stamina -= 0.5f * Time.deltaTime;
                _Speed = SpeedToRun;

                photonView.RPC("SetAnimation", RpcTarget.All, "ToRun", true);
            }
            else
            {
                isRun = false;
                photonView.RPC("SetAnimation", RpcTarget.All, "ToWalk", true);
            }

            // Di chuyển nhân vật
            characterController.Move(moveDirection.normalized * _Speed * Time.deltaTime);
        }
    }

    void HandleCameraRotation()
    {
        // Xử lý xoay camera theo chuột
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Giới hạn góc quay của camera (để không bị lộn ngược)
        Vector3 cameraRotation = cameraTransform.localEulerAngles;
        cameraRotation.x -= mouseY;
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, -60f, 60f);

        // Xoay camera theo trục Y
        cameraTransform.localEulerAngles = new Vector3(cameraRotation.x, cameraRotation.y + mouseX, cameraRotation.z);

        // Xoay nhân vật theo camera
        transform.rotation = Quaternion.Euler(0f, cameraTransform.eulerAngles.y, 0f);
    }

    [PunRPC]
    void SyncStamina(float stamina)
    {
        DataPlayer.Stamina = stamina;
    }

    [PunRPC]
    void SetAnimation(string parameter, bool state)
    {
        animator_Player.SetBool(parameter, state);
    }
}
