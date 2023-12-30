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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            DisplayInventory();
        }
    }

    //functions
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

        if (inventoryIndex < inventory.Length)
        {
            fullInventory = false;
            existingItemIndex = CheckInventory(itemDetails.itemType);
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
            if (quantity < 1)
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
