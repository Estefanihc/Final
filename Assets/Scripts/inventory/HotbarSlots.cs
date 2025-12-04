using UnityEngine;
using static UnityEditor.Progress;

public class HotbarSlots : MonoBehaviour
{
    public KeyCode key;
    public int EquipmentIndex;
    public bool IsSelected = false;
    private bool isConsumable = false;
    private buildSystem buildSys;
    private Item currentItem;
    private void Start()
    {
        buildSys = buildSystem.Instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(key))
        {
            DeselectAllHotbarSlots();
            IsSelected = true;
            transform.GetChild(1).gameObject.SetActive(true);
            Debug.Log("Hotbar key pressed");
            Equip();
        }
        if (isConsumable && Input.GetKeyDown(KeyCode.E) && IsSelected)
            UseConsumable(currentItem);
    }

    private void DeselectAllHotbarSlots()
    {
        foreach (GameObject slot in inventory.Instance.HotBarSlots)
        {
            if (slot.transform.childCount > 1)
            {
                slot.transform.GetChild(1).gameObject.SetActive(false);
                slot.GetComponent<HotbarSlots>().IsSelected = false;
            }
        }
    }

    private void Equip()
    {
        if (transform.childCount <= 2)
        {
            ResetItemStates();
            return;
        }

        currentItem = transform.GetChild(2).GetComponent<Item>();
        ItemType itemType = currentItem.itemType;

        ResetItemStates();

        switch (itemType)
        {
            case ItemType.EnergyPotion:
            case ItemType.Weapon:
            case ItemType.Equipment:
                EquipEquipment(currentItem);
                break;
            case ItemType.HealPotion:
            case ItemType.Consumable:
                isConsumable = true;
                // UseConsumable(currentItem);
                break;

            case ItemType.Object:
                /* ActivateBuildModeIfNeeded();
                 buildSys.SetCurrentBlockAndItem(item.blockID,item.itemID);
                 GameManager.Instance.IsObject = true;*/
                break;

            case ItemType.Build:
                //   buildSys.SetCurrentBlockAndItem(currentItem.blockID, currentItem.itemID);
                buildSys.SetCurrentBuildable(currentItem.itemData);
                buildSys.ActivateBuildMode(true);
                isConsumable = false;
                break;

            default:
                Debug.LogWarning($"Unhandled item type: {itemType}");
                break;
        }
    }

    private void EquipEquipment(Item item)
    {
        EquipmentIndex = item.itemData.EquipmentIndex;
        inventory.Instance.EquipMents[EquipmentIndex].SetActive(true);
        isConsumable = false;
        GameManager.Instance.IsEquipment = true;
        ActivateBuildModeIfNeeded();
    }

    private void UseConsumable(Item item)
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("Player not found!");
            return;
        }

        var stats = player.GetComponent<PlayerStats>();
        var health = player.GetComponent<PlayerHealth>();

        if (stats == null || health == null)
        {
            Debug.LogWarning("Missing PlayerStats or PlayerHealth component on Player!");
            return;
        }

        switch (item.itemData.ConsumableType)
        {
            case ConsumableType.Water:
                if (!stats.AddWater(item.itemData.Amount))
                    return;
                break;

            case ConsumableType.Food:
                if (!stats.AddFood(item.itemData.Amount))
                    return;
                break;

            case ConsumableType.HealPotion:
                if (!health.Heal(item.itemData.Amount))
                    return;
                break;

            default:
                Debug.LogWarning("Invalid consumable type!");
                return;
        }

        inventory.Instance.RmoveItemAmount(item.itemID, 1);
    }


    private void UsePotion(Item item)
    {
        GameManager.Instance.Player.GetComponent<PlayerHealth>().Heal(10);
        inventory.Instance.RmoveItemAmount(item.itemID, 1);
        isConsumable = false;
        ActivateBuildModeIfNeeded();
    }

    private void ResetItemStates()
    {
        GameManager.Instance.IsEquipment = false;
        isConsumable = false;
        GameManager.Instance.IsObject = false;
        UnequipAll();
    }

    private void UnequipAll()
    {
        foreach (GameObject equipment in inventory.Instance.EquipMents)
        {
            equipment.SetActive(false);
        }
    }

    private void ActivateBuildModeIfNeeded()
    {
        if (buildSys != null && buildSys.BuildModeOn)
        {
            buildSys.ActivateBuildMode(false);
        }
    }
}
