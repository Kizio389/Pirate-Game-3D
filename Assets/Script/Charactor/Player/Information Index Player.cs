using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class InformationIndexPlayer : MonoBehaviour
{
    SingletonIndexPlayer DataPlayer; // Lấy thông tin chung của người chơi (Health, Stamina)

    [SerializeField] private Image Hp_bar;          // Thanh hiển thị máu
    [SerializeField] private Image Stamina_bar;     // Thanh hiển thị stamina

    private PhotonView photonView; // Đối tượng PhotonView để đồng bộ logic giữa các người chơi

    private float maxStamina = 100f;                // Giá trị stamina tối đa
    private float currentStamina = 100f;            // Giá trị stamina hiện tại
    private float staminaDrainRate = 10f;           // Tốc độ giảm stamina khi chạy
    private float staminaRegenRate = 5f;            // Tốc độ hồi stamina khi đi bộ hoặc đứng yên
    private bool isRunning = false;                 // Trạng thái chạy
    private bool isWalking = false;                 // Trạng thái đi bộ

    void Start()
    {
        DataPlayer = SingletonIndexPlayer.Instance; // Lấy thông tin từ Singleton
        photonView = GetComponent<PhotonView>();   // Lấy PhotonView của nhân vật

        // Chỉ hiển thị giao diện UI cho người chơi local
        if (!photonView.IsMine)
        {
            Hp_bar.gameObject.SetActive(false);    // Ẩn thanh máu
            Stamina_bar.gameObject.SetActive(false); // Ẩn thanh stamina
        }
    }

    void Update()
    {
        // Chỉ xử lý logic stamina và giao diện UI cho người chơi local
        if (photonView.IsMine)
        {
            HandleStamina(); // Xử lý logic stamina
            UpdateUI();      // Cập nhật giao diện thanh máu và stamina
        }
    }

    // Cập nhật giao diện thanh máu và stamina
    void UpdateUI()
    {
        Hp_bar.fillAmount = DataPlayer.Health / DataPlayer.Max_Health; // Hiển thị mức máu
        Stamina_bar.fillAmount = currentStamina / maxStamina;          // Hiển thị mức stamina
    }

    // Xử lý logic stamina (giảm khi chạy, hồi khi đi bộ hoặc đứng yên)
    void HandleStamina()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool isMoving = horizontal != 0 || vertical != 0;
        bool canRun = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0;

        if (canRun)
        {
            isRunning = true;
            currentStamina -= staminaDrainRate * Time.deltaTime;
            if (currentStamina < 0)
                currentStamina = 0;
        }
        else
        {
            isRunning = false;
            currentStamina += staminaRegenRate * Time.deltaTime;
            if (currentStamina > maxStamina)
                currentStamina = maxStamina;
        }

        GetComponent<Animator>().SetBool("Run", isRunning);
        GetComponent<Animator>().SetBool("ToWalk", isMoving && !isRunning);
    }


}
