using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inventory : MonoBehaviour
{
    public static inventory Instance;

    public Slot[] Slots;
    public GameObject inventoryGameobject;
    public bool IsInventoryOpen;
    public List<GameObject> HotBarSlots;
    public Slot[] EquipSlot;
    public GameObject[] EquipMents;
    public ItemScriptableObject[] Allitems;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        foreach (Slot slot in Slots)
            slot.customStart();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryGameobject.SetActive(!inventoryGameobject.activeInHierarchy);
            IsInventoryOpen = inventoryGameobject.activeInHierarchy;
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            RmoveItemAmount(0, 1);
        }

        RefreshInventory();
    }

    public void RefreshInventory()
    {
        foreach (Slot slot in Slots)
            slot.checkForItem();
    }

    public void ClearInventoryUI()
    {
        foreach (Slot slot in Slots)
        {
            if (slot.slotsItem != null)
            {
                slot.gameObject.SetActive(false);
            }
        }
    }

    public void FilterItemsBuild()
    {
        FilterItemsByType(ItemType.Build);
    }
    public void FilterItemsByType(ItemType filterType)
    {
        foreach (Slot slot in Slots)
        {
            if (slot.slotsItem != null&&slot.HotBar==false)
            {
                bool match = slot.slotsItem.itemType == filterType;
                slot.gameObject.SetActive(match);
            }
        }
    }
   private void ClearItemFilter()
    {
        foreach (Slot slot in Slots)
        {
            if (slot.slotsItem != null )
            {
                slot.gameObject.SetActive(true);
            }
        }
    }

    public int getItemAmount(int id)
    {
        int total = 0;
        foreach (Slot slot in Slots)
        {
            if (slot.slotsItem && slot.slotsItem.itemID == id)
            {
                total += slot.slotsItem.amountInStack;
            }
        }
        return total;
    }

    public void RmoveItemAmount(int id, int amountToRemove)
    {
        foreach (Slot slot in Slots)
        {
            if (amountToRemove <= 0) return;

            Item item = slot.slotsItem;
            if (item && item.itemID == id)
            {
                int removable = item.amountInStack;
                if (removable <= amountToRemove)
                {
                    Destroy(item.gameObject);
                    amountToRemove -= removable;
                }
                else
                {
                    item.amountInStack -= amountToRemove;
                    amountToRemove = 0;
                }
            }
        }
    }

    public void AddItem(Item newItem, Item startingItem = null)
    {
        int stackAmount = newItem.amountInStack;
        List<Item> stackables = new();
        List<Slot> emptySlots = new();

        if (startingItem && startingItem.itemID == newItem.itemID && startingItem.amountInStack < startingItem.maxStackSize)
            stackables.Add(startingItem);

        foreach (Slot slot in Slots)
        {
            if (slot.slotsItem)
            {
                Item item = slot.slotsItem;
                if (item.itemID == newItem.itemID && item.amountInStack < item.maxStackSize && item != startingItem)
                    stackables.Add(item);
            }
            else
            {
                emptySlots.Add(slot);
            }
        }

        foreach (Item item in stackables)
        {
            int canAdd = item.maxStackSize - item.amountInStack;
            if (stackAmount <= canAdd)
            {
                item.amountInStack += stackAmount;
                Destroy(newItem.gameObject);
                return;
            }
            else
            {
                item.amountInStack = item.maxStackSize;
                stackAmount -= canAdd;
            }
        }

        newItem.amountInStack = stackAmount;
        if (emptySlots.Count > 0)
        {
            newItem.transform.SetParent(emptySlots[0].transform);
            newItem.gameObject.SetActive(false);
        }
    }
    public void FilterByEquipment() => FilterItemsByType(ItemType.Equipment);
    public void FilterByConsumable() => FilterItemsByType(ItemType.Consumable);
    public void FilterByBuild() => FilterItemsByType(ItemType.Build);
    public void FilterByPotion() => FilterItemsByType(ItemType.HealPotion); // Example: could also include EnergyPotion
    public void FilterByWeapon() => FilterItemsByType(ItemType.Weapon); // Example: could also include EnergyPotion
    public void ClearFilter() => ClearItemFilter();
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();
        if (item)
            AddItem(item);
    }
}



