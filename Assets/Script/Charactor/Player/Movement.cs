using Photon.Pun; // Thư viện Photon để hỗ trợ multiplayer
using UnityEngine;

public class Movement : MonoBehaviourPun // Lớp này xử lý việc di chuyển nhân vật trong môi trường multiplayer
{
    private CharacterController characterController; // Quản lý vật lý và di chuyển của nhân vật
    private Animator animator_Player; // Điều khiển hoạt ảnh của nhân vật

    [SerializeField] private float SpeedToWalk = 1f; // Tốc độ khi đi bộ
    [SerializeField] private float SpeedToRun = 3f; // Tốc độ khi chạy
    private bool isRun = false; // Trạng thái xác định nhân vật đang chạy

    private Transform cameraTransform; // Tham chiếu tới camera chính để định hướng di chuyển
    private SingletonIndexPlayer DataPlayer; // Lấy dữ liệu người chơi từ Singleton (các thông tin như stamina, máu, v.v.)

    private float syncRate = 0.1f; // Tần suất gửi dữ liệu mạng (mỗi 0.1 giây)
    private float syncTimer = 0f; // Bộ đếm thời gian để gửi dữ liệu mạng

    private void Awake()
    {
        // Khởi tạo các thành phần quan trọng
        characterController = GetComponent<CharacterController>(); // Gắn CharacterController để xử lý di chuyển
        animator_Player = GetComponent<Animator>(); // Gắn Animator để điều khiển hoạt ảnh
        DataPlayer = SingletonIndexPlayer.Instance; // Lấy dữ liệu của người chơi từ Singleton
        cameraTransform = Camera.main.transform; // Lấy tham chiếu camera chính để xoay hướng theo góc nhìn
    }

    private void Update()
    {
        if (!photonView.IsMine) return; // Kiểm tra quyền sở hữu, chỉ điều khiển nhân vật thuộc về người chơi hiện tại

        HandleCameraRotation(); // Xử lý xoay camera
        HandleMovement();       // Xử lý di chuyển nhân vật

        // Gửi dữ liệu vị trí và hướng của nhân vật lên mạng
        syncTimer += Time.deltaTime; // Tăng bộ đếm thời gian
        if (syncTimer >= syncRate) // Nếu đã đạt đến tần suất cập nhật
        {
            photonView.RPC("SyncPlayerState", RpcTarget.Others, transform.position, transform.rotation); // Đồng bộ trạng thái
            syncTimer = 0f; // Reset bộ đếm
        }
    }

    void HandleMovement()
    {
        // Lấy hướng di chuyển từ bàn phím
        float horizontal = Input.GetAxis("Horizontal"); // Nhấn A/D hoặc phím tương ứng để di chuyển ngang
        float vertical = Input.GetAxis("Vertical"); // Nhấn W/S hoặc phím tương ứng để di chuyển thẳng

        Vector3 moveDirection = cameraTransform.right * horizontal + cameraTransform.forward * vertical; // Xác định hướng dựa theo góc nhìn camera
        moveDirection.y = 0; // Loại bỏ di chuyển trên trục Y (để không bay lên)

        if (moveDirection.magnitude >= 0.1f) // Nếu có chuyển động từ người chơi
        {
            float speed = Input.GetKey(KeyCode.LeftShift) ? SpeedToRun : SpeedToWalk; // Chuyển giữa đi bộ và chạy dựa trên phím Shift
            characterController.Move(moveDirection.normalized * speed * Time.deltaTime); // Di chuyển nhân vật theo hướng đã tính
        }
    }

    void HandleCameraRotation()
    {
        // Lấy input chuột để điều khiển góc nhìn
        float mouseX = Input.GetAxis("Mouse X"); // Xoay nhân vật theo trục Y khi di chuột ngang
        float mouseY = Input.GetAxis("Mouse Y"); // Xoay camera theo trục X khi di chuột dọc

        Vector3 cameraRotation = cameraTransform.localEulerAngles; // Lấy góc quay hiện tại của camera
        cameraRotation.x = Mathf.Clamp(cameraRotation.x - mouseY, -60f, 60f); // Giới hạn góc nhìn theo trục X để không lật camera
        cameraTransform.localEulerAngles = new Vector3(cameraRotation.x, cameraRotation.y + mouseX, 0); // Cập nhật góc nhìn mới cho camera
        transform.rotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0); // Cập nhật góc quay cho nhân vật theo góc nhìn
    }

    [PunRPC]
    void SyncPlayerState(Vector3 position, Quaternion rotation)
    {
        // Đồng bộ trạng thái vị trí và góc quay từ người chơi khác
        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 10); // Chuyển vị trí nhân vật dần đến vị trí nhận được
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10); // Xoay nhân vật dần đến hướng nhận được
    }
}
