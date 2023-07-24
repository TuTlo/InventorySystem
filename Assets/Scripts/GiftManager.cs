using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiftManager : MonoBehaviour
{
    public Image thisItemImage;
    public Text thisItemName;
    public Text thisItemDescription;
    public GameObject giveGiftButton;

    private Slot selectedSlot;


    void Start()
    {
        Button btn = giveGiftButton.GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            OnBtnClick();
        });

    }


    public void Display()
    {
        selectedSlot = ButtonListener.selectedItem.GetComponent<Slot>();

        thisItemName.text = selectedSlot.slotName.text;
        thisItemImage.sprite = selectedSlot.slotImage.sprite;
        thisItemDescription.text = selectedSlot.slotInfo.text;giveGiftButton.SetActive(true);


    }

    void OnBtnClick()
    {


        string selectedItemName = selectedSlot.slotName.text;

        if (selectedItemName == "")
        {
            return;
        }


        InventoryManager.removeItem(selectedItemName, 1);
        Debug.Log("送出: " + selectedItemName);


    }
}
