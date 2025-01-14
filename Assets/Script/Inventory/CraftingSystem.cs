using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSystem : MonoBehaviour
{
    private GameObject craftingScreenUI; // Giao di?n chính c?a Crafting
    private GameObject toolsScreenUI;    // Giao di?n Tools
    public List<string> inventoryItemList = new List<string>();

    // Nút chuy?n ??i gi?a các danh m?c
    Button toolsBTN;

    // Nút th?c hi?n craft item (ví d?: Axe)
    Button craftAxeBTN;

    // Yêu c?u ?? craft item
    Text AxeReq1, AxeReq2;
    private bool isOpen;

    private Blueprint AxeBLP= new Blueprint("AXE",2,"Stone",3,"Stick",3);
    public static CraftingSystem Instance { get; set; }
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
        isOpen = false; // Ban ??u Crafting UI ?óng

        // Gán nút tools
        /*toolsBTN = craftingScreenUI.transform.Find("ToolsButton").GetComponent<Button>();*/
        /*toolsBTN.onClick.AddListener(delegate { OpenToolsCategory(); });*/

        // Gán yêu c?u và nút craft Axe
      /*  AxeReq1 = toolsScreenUI.transform.Find("Axe").transform.Find("req1").GetComponent<Text>();
        AxeReq2 = toolsScreenUI.transform.Find("Axe").transform.Find("req2").GetComponent<Text>();

        craftAxeBTN = toolsScreenUI.transform.Find("Axe").transform.Find("Button").GetComponent<Button>();
        craftAxeBTN.onClick.AddListener(delegate { CraftAnyItem(AxeBLP); });*/
    }
    
    private void Update()
    {
        /*RefreshNeededItems();*/
        if (Input.GetKeyDown(KeyCode.C) && !isOpen)
        {
            craftingScreenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.C) && isOpen)
        {
            craftingScreenUI.SetActive(false);
            toolsScreenUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isOpen = false;
        }
    }
    /*void OpenToolsCategory()
    {
        craftingScreenUI.SetActive(false); // T?t giao di?n chính
        toolsScreenUI.SetActive(true);    // B?t giao di?n Tools
    }*/
   /* public void RefreshNeededItems()
    {
        // Kh?i t?o b? ??m cho các nguyên li?u c?n thi?t
        int stone_count = 0;
        int stick_count = 0;

        // L?y danh sách các item t? InventorySystem
        inventoryItemList = InventorySystem2.Instance.itemList;

        // Duy?t qua danh sách các item trong inventory
        foreach (string itemName in inventoryItemList)
        {
            switch (itemName)
            {
                case "Stone":
                    stone_count += 1; // T?ng s? l??ng ?á
                    break;

                case "Stick":
                    stick_count += 1; // T?ng s? l??ng que
                    break;

                default:
                    break;
            }
        }
        AxeReq1.text = $"3 Stone [{stone_count}]"; // 3 là s? l??ng yêu c?u
        AxeReq2.text = $"3 Stick [{stick_count}]"; // 3 là s? l??ng yêu c?u
                                                   // Ki?m tra n?u ?? nguyên li?u ?? craft Axe
        if (stone_count >= 3 && stick_count >= 3)
        {
            craftAxeBTN.gameObject.SetActive(true); // Kích ho?t nút ch? t?o
        }
        else
        {
            craftAxeBTN.gameObject.SetActive(false); // Vô hi?u hóa nút ch? t?o
        }
    }*/

    public IEnumerator calculate()
    {
        // Tạm dừng trong 1 giây
        yield return new WaitForSeconds(1f);

        // Cập nhật lại danh sách inventory
        InventorySystem2.Instance.ReCalculeList();
    }

    /*void CraftAnyItem(Blueprint blueprintToCraft)
    {
        // Thêm item chế tạo vào inventory
        InventorySystem2.Instance.AddToInventory(blueprintToCraft.itemName);

        // Kiểm tra số lượng loại nguyên liệu cần thiết
        if (blueprintToCraft.numOfRequirements == 1)
        {
            // Nếu chỉ yêu cầu 1 loại nguyên liệu, xóa nguyên liệu đó khỏi inventory
            InventorySystem2.Instance.RemoveItem(blueprintToCraft.Req1, blueprintToCraft.Req1amount);
        }
        else if (blueprintToCraft.numOfRequirements == 2)
        {
            // Nếu yêu cầu 2 loại nguyên liệu, xóa cả hai khỏi inventory
            InventorySystem2.Instance.RemoveItem(blueprintToCraft.Req1, blueprintToCraft.Req1amount);
            InventorySystem2.Instance.RemoveItem(blueprintToCraft.Req2, blueprintToCraft.Req2amount);
        }

        StartCoroutine(calculate());

        // Làm mới thông tin yêu cầu nguyên liệu
        *//*RefreshNeededItems();*//*
    }*/

}
