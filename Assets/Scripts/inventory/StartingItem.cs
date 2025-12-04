using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StartingItemData
{
    public ItemScriptableObject itemSO;
    public int amount;
}

public class StartingItem : MonoBehaviour
{
    public List<StartingItemData> startingItems;

    void Start()
    {
        StartCoroutine(DelayedAddItems());
    }

    IEnumerator DelayedAddItems()
    {
        foreach (StartingItemData itemData in startingItems)
        {
            yield return new WaitForSeconds(0.1f);

            if (itemData.itemSO == null) continue;
            Debug.Log("SpawnItem");
            // Create a new GameObject with an Item component
            GameObject itemObj = new GameObject(itemData.itemSO.name + "_Instance");
            Item newItem = itemObj.AddComponent<Item>();

            // Assign the scriptable object data to the new item
            newItem.itemData = itemData.itemSO;
            newItem.amountInStack = Mathf.Clamp(itemData.amount, 1, itemData.itemSO.maxStackSize);

            // Initialize the item (simulating what happens in Item.Start())
            newItem.itemSprite = itemData.itemSO.itemSprite;
            newItem.itemID = itemData.itemSO.itemID;
            newItem.maxStackSize = itemData.itemSO.maxStackSize;
            newItem.EquipmentType = itemData.itemSO.ConsumableType;
            newItem.EquipmentIndex = itemData.itemSO.EquipmentIndex;
            newItem.itemType = itemData.itemSO.itemType;

            // Add to inventory
            inventory.Instance.AddItem(newItem);

            // Destroy the GameObject if you don't need it anymore
          //  Destroy(itemObj);
        }
    }
}