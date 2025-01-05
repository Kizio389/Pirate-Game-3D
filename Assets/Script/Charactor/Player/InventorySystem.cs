using UnityEngine;
using Photon.Pun; // Thư viện Photon

public class InventorySystem : MonoBehaviourPun
{
    [Header("UI Elements")]
    public GameObject inventoryScreenUI; // Giao diện Inventory

    [Header("Settings")]
    public KeyCode toggleKey1 = KeyCode.I; // Phím tắt chính
    public KeyCode toggleKey2 = KeyCode.Tab; // Phím tắt phụ

    private bool isOpen;

    private void Start()
    {
        // Chỉ xử lý Inventory cho nhân vật của người chơi hiện tại
        if (!photonView.IsMine)
        {
            enabled = false; // Vô hiệu hóa script nếu không phải của người chơi cục bộ
            return;
        }

        isOpen = false;

        // Tìm UI trong con của nhân vật nếu chưa được gán
        if (inventoryScreenUI == null)
        {
            inventoryScreenUI = transform.Find("InventoryScreenUI")?.gameObject; // Đảm bảo đúng tên của UI
        }

        if (inventoryScreenUI != null)
        {
            inventoryScreenUI.SetActive(false); // Ẩn Inventory ban đầu
        }
        else
        {
            Debug.LogError("Không tìm thấy InventoryScreenUI! Hãy gán thủ công trong Inspector.");
        }
    }

    private void Update()
    {
        // Chỉ xử lý phím tắt cho người chơi cục bộ
        if (photonView.IsMine && (Input.GetKeyDown(toggleKey1) || Input.GetKeyDown(toggleKey2)))
        {
            ToggleInventory();
        }
    }

    /// <summary>
    /// Bật hoặc tắt giao diện Inventory.
    /// </summary>
    public void ToggleInventory()
    {
        if (inventoryScreenUI != null)
        {
            isOpen = !isOpen; // Đảo trạng thái
            inventoryScreenUI.SetActive(isOpen);

            // Hiển thị/mở khóa chuột khi mở inventory
            Cursor.visible = isOpen;
            Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;

            Debug.Log($"Inventory is now {(isOpen ? "opened" : "closed")}.");
        }
        else
        {
            Debug.LogError("InventoryScreenUI không được gán!");
        }
    }

    /// <summary>
    /// Kiểm tra trạng thái mở/tắt của Inventory.
    /// </summary>
    /// <returns>True nếu Inventory đang mở, False nếu đóng.</returns>
    public bool IsInventoryOpen()
    {
        return isOpen;
    }
}
