using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Iteam", menuName = "Iteam")]
public class ItemScriptableObject : ScriptableObject
{

     public ItemType itemType;
     public ConsumableType ConsumableType;
    public EquipmentTypes Equipmenttype;
    public int EquipmentIndex;
     public Sprite itemSprite;
     public float Amount=20;
     public int maxStackSize = 1000;
     public int itemID;
    [Header("Buildable Settings (Optional)")]
    public GameObject buildPrefab; 
    public bool isSolid;
    public LayerMask detectionLayer;
}
[System.Serializable]
public enum ItemType
{
    Equipment,
    Consumable,
    Object,
    HealPotion,
    EnergyPotion,
    Build,
    Gun, Weapon, Tower
}
[System.Serializable]
public enum ConsumableType
{
    Food,
    Water,
    HealPotion,
    EnergyPotion,
    Weapon,
    Armor,
    Tool,
    Shield,
    Accessory,
}
public enum EquipmentTypes
{
    None,
    Weapon,
    Armor,
    Tool,
    Shield,
    Accessory
}