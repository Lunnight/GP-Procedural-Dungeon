using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    //global initialisation
    Item[] inventory;
    int inventoryIndex;

    //displaying inventory
    GameObject canvas;
    TextMeshProUGUI invenDisplay;
    TextMeshProUGUI fullInventoryDisplay;
    TextMeshProUGUI inventoryDelete;
    bool displaying;

    //main
    void Start()
    {
        inventory = new Item[3];   //change number of inventory slots here
        inventoryIndex = -1;
        displaying = false;

        canvas = GameObject.Find("Canvas");
        invenDisplay = GameObject.Find("Inventory").GetComponent<TextMeshProUGUI>();

        fullInventoryDisplay = GameObject.Find("FullInventory").GetComponent<TextMeshProUGUI>();
        fullInventoryDisplay.CrossFadeAlpha(0, 0, true);
        inventoryDelete = GameObject.Find("InventoryDelete").GetComponent<TextMeshProUGUI>();
        inventoryDelete.CrossFadeAlpha(0, 0, true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            DisplayInventory();
        }
        //deleting item in slot 1, 2 or 3
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DeleteItem(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DeleteItem(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            DeleteItem(3);
        }
    }

    //functions

    public void DeleteItem(int index)
    {
        index--;    //originally num between 1 and 3, - to make into array index

        if (inventoryIndex == -1)
        {
            inventoryDelete.text = "Inventory Empty";
        }
        else if (inventory[index] == null)
        {
            inventoryDelete.text = "Slot Empty";
        }
        else
        {
            Item[] tempInventory = new Item[3];
            int counter = -1;

            inventory[index] = null;

            for (int i = 0; i <= inventoryIndex; i++)
            {
                if (inventory[i] != null)
                {
                    counter++;
                    tempInventory[counter] = inventory[i];
                }
            }

            inventoryIndex = counter;
            inventory = tempInventory;

            UpdateDisplay();
            inventoryDelete.text = "Item Removed";
        }

        inventoryDelete.CrossFadeAlpha(1, 0, true);
        inventoryDelete.CrossFadeAlpha(0, 2f, true);
    }
    public void UpdateDisplay()
    {
        invenDisplay.text = null;
        if (displaying)
        {
            if (inventoryIndex > -1)
            {
                for (int i = 0; i <= inventoryIndex; i++)
                {
                    invenDisplay.text = invenDisplay.text + inventory[i].ItemType + ": " + inventory[i].Quantity + "\n";
                }
            }
            else
            {
                invenDisplay.text = "Inventory empty";
            }
        }
    }
    public void DisplayInventory()
    {
        if (!displaying)
        {
            if (inventoryIndex > -1)
            {
                for (int i = 0; i <= inventoryIndex; i++)
                {
                    invenDisplay.text = invenDisplay.text + inventory[i].ItemType + ": " + inventory[i].Quantity + "\n";
                }
            }
            else
            {
                invenDisplay.text = "Inventory empty";
            }
            invenDisplay.gameObject.SetActive(true);
            displaying = true;
        }
        else
        {
            invenDisplay.gameObject.SetActive(false);
            displaying = false;
            invenDisplay.text = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool full;
        if (collision.gameObject.tag == "Item")
        {
            full = AddItem(collision.gameObject);
            if (full)
            {
                fullInventoryDisplay.CrossFadeAlpha(1, 0, true);
                fullInventoryDisplay.CrossFadeAlpha(0, 2f, true);
            }
            else
            {
                UpdateDisplay();
                ItemDetails itemDetails = GameObject.FindObjectOfType<ItemDetails>();
                Destroy(collision.gameObject);
            }
        }
    }
    int CheckInventory(string groundItem)
    {
        int index = -1;   //will point to where item should be added (if not already in inventory)
        for (int i = 0; i <= inventoryIndex; i++)
        {
            if (groundItem == inventory[i].ItemType)
            {
                index = i;
            }
        }

        return index;
    }

    bool AddItem(GameObject groundItem)
    {
        bool fullInventory = true;
        int existingItemIndex;
        ItemDetails itemDetails;

        itemDetails = groundItem.gameObject.GetComponent<ItemDetails>();

        existingItemIndex = CheckInventory(itemDetails.itemType);
        if (inventoryIndex + 1 < inventory.Length || existingItemIndex != -1)
        {
            fullInventory = false;
            if (existingItemIndex == -1)
            {
                inventoryIndex++;
                inventory[inventoryIndex] = new Item(itemDetails.itemType);
            }
            else if (!inventory[existingItemIndex].IncreaseQuantity())
            {
                fullInventory = true;
            }
        }

        return fullInventory;
    }

    //classes
    class Item
    {
        int quantity;
        string itemType;

        public Item(string itemType_)
        {
            quantity = 1;
            itemType = itemType_;
        }
        public bool IncreaseQuantity()
        {
            if (quantity < 2)
            {
                quantity += 1;
                return true;
            }
            else
            {
                return false;
            }
        }
        public string ItemType {  get { return itemType; } }
        public int Quantity { get { return quantity; } }
    }
}
