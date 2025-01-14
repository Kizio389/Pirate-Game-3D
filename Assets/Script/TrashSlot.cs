using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TrashSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject trashAlertUI;

    private TextMeshProUGUI textToModify;

    public Sprite trash_closed;
    public Sprite trash_opened;

    private Image imageComponent;

    Button YesBTN, NoBTN;

    GameObject draggedItem
    {
        get
        {
            return DragDrop.itemBeingDragged;
        }
    }

    GameObject itemToBeDeleted;



    public string itemName
    {
        get
        {
            string name = itemToBeDeleted.name;
            string toRemove = "(Clone)";
            string result = name.Replace(toRemove, "");
            return result;
        }
    }



    void Start()
    {
        imageComponent = transform.Find("background")?.GetComponent<Image>();
        if (imageComponent == null)
        {
            Debug.LogError("Background image component not found!");
        }

        if (trashAlertUI != null)
        {
            textToModify = trashAlertUI.transform.Find("Text")?.GetComponent<TextMeshProUGUI>();
            YesBTN = trashAlertUI.transform.Find("yes")?.GetComponent<Button>();
            NoBTN = trashAlertUI.transform.Find("no")?.GetComponent<Button>();

            if (YesBTN != null)
            {
                YesBTN.onClick.AddListener(delegate { DeleteItem(); });
            }
            else
            {
                Debug.LogError("Yes button not found!");
            }

            if (NoBTN != null)
            {
                NoBTN.onClick.AddListener(delegate { CancelDeletion(); });
            }
            else
            {
                Debug.LogError("No button not found!");
            }
        }
        else
        {
            Debug.LogError("trashAlertUI is not assigned in the Inspector.");
        }
    }


    public void OnDrop(PointerEventData eventData)
    {
        //itemToBeDeleted = DragDrop.itemBeingDragged.gameObject;
        if (draggedItem.GetComponent<InventoryItem>().isTrashable == true)
        {
            itemToBeDeleted = draggedItem.gameObject;
            StartCoroutine(notifyBeforeDeletion());
        }

    }

    IEnumerator notifyBeforeDeletion()
    {
        trashAlertUI.SetActive(true);
        textToModify.text = "Throw away this " + itemName + "?";
        yield return new WaitForSeconds(1f);
    }

    private void CancelDeletion()
    {
        imageComponent.sprite = trash_closed;
        trashAlertUI.SetActive(false);
    }

    private void DeleteItem()
    {
        imageComponent.sprite = trash_closed;
        DestroyImmediate(itemToBeDeleted.gameObject);
        InventorySystem2.Instance.ReCalculeList();
        /*CraftingSystem.Instance.RefreshNeededItems();*/
        trashAlertUI.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        if (draggedItem != null && draggedItem.GetComponent<InventoryItem>().isTrashable == true)
        {
            imageComponent.sprite = trash_opened;
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (draggedItem != null && draggedItem.GetComponent<InventoryItem>().isTrashable == true)
        {
            imageComponent.sprite = trash_closed;
        }
    }

}

