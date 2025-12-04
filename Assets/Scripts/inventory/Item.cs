using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemScriptableObject;
//using static UnityEditor.Progress;

public class Item : MonoBehaviour
{
    public ItemScriptableObject itemData;
    public int amountInStack = 1;

    private inventory inv;
   
    [HideInInspector] public ConsumableType EquipmentType;
    [HideInInspector] public EquipmentTypes Equipmenttype;
    [HideInInspector] public int EquipmentIndex;
    [HideInInspector] public Sprite itemSprite;
    [HideInInspector] public int maxStackSize = 1000;
    [HideInInspector] public int itemID;
    [HideInInspector] public ItemType itemType;
    [HideInInspector] public bool build_Iteam;



    void Start()
    {
        inv =inventory.Instance;
        if (itemData!=null)
        {
            itemSprite = itemData.itemSprite;
            itemID = itemData.itemID;
            maxStackSize = itemData.maxStackSize;
            EquipmentType = itemData.ConsumableType;
            EquipmentIndex = itemData.EquipmentIndex;
            itemType = itemData.itemType;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == "Player")
        {

            if (inv)
            {
               
                inv.AddItem(this);
                //  Destroy(gameObject);
            }
        }
    }
   
}
