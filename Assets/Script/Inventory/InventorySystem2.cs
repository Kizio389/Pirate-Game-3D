using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySystem2 : MonoBehaviourPun
{
    public static InventorySystem2 Instance { get; set; }

    public GameObject ItemInfoUI;

    [Header("UI Elements")]
    public GameObject inventoryScreenUI; // Giao diện Inventory

    [Header("Settings")]
    public KeyCode toggleKey1 = KeyCode.I; // Phím tắt chính
    public KeyCode toggleKey2 = KeyCode.Tab; // Phím tắt phụ
    public bool isOpen;

    


    public List<GameObject> slotList = new List<GameObject>();

    public List<string> itemList = new List<string>();

    private GameObject itemToAdd; 

    private GameObject whatSlotToEquip;

    public bool isFull;
    private CanvasGroup canvasGroup;

    [Header("Pickup Alert Settings")]
    public float fadeDuration = 0.5f; // Thời gian fade in/out
    public float displayDuration = 2f; // Thời gian hiển thị trước khi fade out
    private Coroutine fadeCoroutine; // Để quản lý coroutine đang chạy
    private CanvasGroup pickupAlertCanvasGroup; // Quản lý độ trong suốt
    public GameObject pickupAlert;
    public TextMeshProUGUI pickupName;
    public Image pickupImage;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
       
    }


    private void Start()
    {
        Cursor.visible = false;
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
        PopulateSlotList();
        isFull = false;
        pickupAlertCanvasGroup = pickupAlert.GetComponent<CanvasGroup>();
        if (pickupAlertCanvasGroup == null)
        {
            pickupAlertCanvasGroup = pickupAlert.AddComponent<CanvasGroup>();
        }
        pickupAlertCanvasGroup.alpha = 0; // Đảm bảo ban đầu popup ẩn
        pickupAlert.SetActive(false);
      
    }


    void TriggerPickupPopUp(string itemName, Sprite itemSprite)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine); // Dừng hiệu ứng cũ nếu đang chạy
        }

        pickupAlert.SetActive(true);
        pickupName.text = itemName;
        pickupImage.sprite = itemSprite;

        fadeCoroutine = StartCoroutine(FadePickupAlert());
    }
    private IEnumerator FadePickupAlert()
    {
        // Fade in
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            pickupAlertCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        // Hiển thị trong một khoảng thời gian
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            pickupAlertCanvasGroup.alpha = Mathf.Clamp01(1 - (elapsedTime / fadeDuration));
            yield return null;
        }

        pickupAlert.SetActive(false);
        fadeCoroutine = null; // Reset coroutine
    }


    private void PopulateSlotList()
    {
       foreach(Transform child in  inventoryScreenUI.transform)
        {
            if(child.CompareTag("Slot"))
            {
                slotList.Add(child.gameObject);
            }
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

    public void AddToInventory(string itemName)
    {
        
            whatSlotToEquip = FindNextEmtySlot();
            itemToAdd= Instantiate(Resources.Load<GameObject>(itemName),whatSlotToEquip.transform.position,whatSlotToEquip.transform.rotation);
            itemToAdd.transform.SetParent(whatSlotToEquip.transform);
            itemList.Add(itemName);
         
        TriggerPickupPopUp(itemName , itemToAdd.GetComponent<Image>().sprite);

        ReCalculeList();

    }

   

    private GameObject FindNextEmtySlot()
    {
       
        foreach(GameObject slot in slotList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        return new GameObject();
    }

    public bool CheckIfFull()
    {
        int counter = 0;
        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount > 0)
            {
                counter += 1;
            }
           
        }
        if (counter == 21)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public void RemoveItem(string nameToRemove, int amountToRemove)
    {
        int counter = amountToRemove; // Số lượng cần xóa

        // Duyệt qua danh sách slot từ cuối về đầu để tránh lỗi khi xóa phần tử
        for (var i = slotList.Count - 1; i >= 0; i--)
        {
            // Kiểm tra nếu slot hiện tại có ít nhất một item
            if (slotList[i].transform.childCount > 0)
            {
                // Kiểm tra tên của item trong slot, bỏ qua "(Clone)" nếu có
                if (slotList[i].transform.GetChild(0).name == nameToRemove + "(Clone)" && counter > 0)
                {
                    // Xóa item
                    DestroyImmediate(slotList[i].transform.GetChild(0).gameObject);
                    counter--; // Giảm số lượng cần xóa
                }
            }

            // Dừng vòng lặp nếu đã xóa đủ số lượng cần thiết
            if (counter == 0)
            {
                break;
            }
        }

        // Cập nhật lại danh sách item sau khi xóa
        ReCalculeList();

        
    } 
    public void ReCalculeList()
    {
       itemList.Clear();
        foreach(GameObject slot in slotList)
        {
            if(slot.transform.childCount > 0)
            {
                string name = slot.transform.GetChild(0).name;
                string str2 = "(Clone)";
                string result = name.Replace(str2, "");
                itemList.Add(result);
            }
        
        }
    }
}
