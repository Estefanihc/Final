using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class craftingSlot : MonoBehaviour
{
    public requireditem[] itemNeeded;

    public inventory inv;

    public GameObject carftedItem;

    public int craftedItemAmount;
    public void carftItem()
    {


        foreach(requireditem i in itemNeeded)
        {


            if (inv.getItemAmount(i.itemId) < i.amountNeeded)
                return;
        }
        foreach(requireditem i in itemNeeded)
        {

            inv.RmoveItemAmount(i.itemId, i.amountNeeded);

        }

        Item z = Instantiate(carftedItem, Vector2.zero, Quaternion.identity).GetComponent<Item>();

        z.amountInStack = craftedItemAmount;
        inv.AddItem(z);
      //  inv.AddItem(Instantiate(carftedItem, Vector2.zero, Quaternion.identity).GetComponent<Item>());

    }
}


[System.Serializable]
public class requireditem
{

    public int itemId;

    public int amountNeeded;

}
