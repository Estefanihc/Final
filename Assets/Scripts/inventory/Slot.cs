using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Item slotsItem;
    public EquipmentTypes Equipmenttype;
    Sprite defaultsprite;
    Text amountText;

    public bool HotBar;

    public void customStart()
    {
        defaultsprite = GetComponent<Image>().sprite;

        amountText = transform.GetChild(0).GetComponent<Text>();
    }
    public void dropItem()
    {

        if (slotsItem)
        {
            slotsItem.transform.parent = null;
            slotsItem.gameObject.SetActive(true);
            slotsItem.transform.position = Vector2.zero;





        }
    }

    public void checkForItem()
    {
         int x = HotBar ? 2 : 1;
        if (transform.childCount > x)
        {


            slotsItem = transform.GetChild(x).GetComponent<Item>();

            GetComponent<Image>().sprite = slotsItem.itemSprite;
            if (slotsItem.amountInStack > 1)
            {
                amountText.text = slotsItem.amountInStack.ToString();
            }
            else
            {
                amountText.text = "";
            }
        }

        else
        {
            slotsItem = null;
            GetComponent<Image>().sprite = defaultsprite;
            amountText.text = "";

        }
    }
}
