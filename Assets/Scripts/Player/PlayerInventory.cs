using System;
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
    TextMeshProUGUI invenManage;
    bool displaying;

    //for item use
    FloorOverhead floorOverhead;
    bool uPressed;
    Transform collisionFloor;
    public Material[] roomColours;

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
        invenManage = GameObject.Find("InvenManage").GetComponent<TextMeshProUGUI>();
        invenManage.CrossFadeAlpha(0, 0, true);

        floorOverhead = GameObject.FindObjectOfType<FloorOverhead>();
        uPressed = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            DisplayInventory();
        }

        if (!uPressed)
        {
            //deleting item in slot 1, 2 or 3
            if (Input.GetKeyDown(KeyCode.Alpha1))
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
        else
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                UseItem(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                UseItem(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                UseItem(3);
            }
        }


        if (Input.GetKeyDown(KeyCode.U))
        {
            switch (uPressed)
            {
                case true:
                    invenManage.text = "Deleting items";
                    uPressed = false;
                    break;
                case false:
                    uPressed = true;
                    invenManage.text = "Using items";
                    break;
            }
            invenManage.CrossFadeAlpha(1, 0, true);
            invenManage.CrossFadeAlpha(0, 2, true);
        }
    }

    //functions
    public void SortInventory(int index)
    {
        Item[] tempInventory = new Item[3];
        int counter = -1;

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
    }
    public void UseItem(int index)
    {
        index--;

        if (inventory[index] != null)
        {
            //potion and Paint currently have no use
            switch (inventory[index].ItemType)
            {
                case "Potion":
                    invenManage.text = "Potion has no use";
                    break;
                case "Teleport":
                    //puts the player in a random new position
                    Vector3 newPosition = floorOverhead.RoomPositions[UnityEngine.Random.Range(0, floorOverhead.RoomPositions.Count)];
                    newPosition = new Vector3(newPosition.x, newPosition.y + 1.5f, newPosition.z);
                    transform.position = newPosition;

                    invenManage.text = "Teleport used";
                    break;
                case "Paint":
                    GameObject prefabRoom;
                    Material partColour = roomColours[UnityEngine.Random.Range(0, roomColours.Length)];
                    for (int i = 0; i < floorOverhead.RoomPositions.Count; i++)
                    {
                        // find the room the player is in
                        if (floorOverhead.RoomPositions[i] == collisionFloor.position)
                        {
                            prefabRoom = collisionFloor.parent.gameObject; //gets the prefab the floor is attached to

                            for (int x = 0; x < prefabRoom.transform.childCount; x++)
                            {
                                //if it's a floor or wall then change its colour
                                if (prefabRoom.transform.GetChild(x).tag == "Floor" || prefabRoom.transform.GetChild(x).tag == "Wall")
                                {
                                    prefabRoom.transform.GetChild(x).GetComponent<MeshRenderer>().material = partColour;
                                }
                            }
                        }
                    }

                    invenManage.text = "Paint used";
                    break;
            }
            inventory[index].DecreaseQuantity();
            if (inventory[index].Quantity < 1)
            {
                inventory[index] = null;
                SortInventory(index);
            }
            UpdateDisplay();
        }
        else
        {
            invenManage.text = "Slot empty";
        }

        invenManage.CrossFadeAlpha(1, 0, true);
        invenManage.CrossFadeAlpha(0, 2, true);
    }

    public void DeleteItem(int index)
    {
        index--;    //originally num between 1 and 3, - to make into array index
        if (inventory[index] == null)
        {
            invenManage.text = "Slot Empty";
        }
        else
        {
            inventory[index] = null;
            SortInventory(index);

            UpdateDisplay();
            invenManage.text = "Item Removed";
        }

        invenManage.CrossFadeAlpha(1, 0, true);
        invenManage.CrossFadeAlpha(0, 2f, true);
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
        //for paint item
        if (collision.gameObject.tag == "Floor")
        {
            collisionFloor = collision.gameObject.transform;
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
        public void DecreaseQuantity()
        {
            quantity--;
        }
        public string ItemType {  get { return itemType; } }
        public int Quantity { get { return quantity; } }
    }
}
