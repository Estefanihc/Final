using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class dragAndDrop : MonoBehaviour
{
    public inventory inv;
    GameObject curSlot;
    Item curSlotItem;
    public Image followMouseImage;


    private void Update()
    {
        followMouseImage.transform.position = Input.mousePosition;

        if (Input.GetKeyDown(KeyCode.G))
        {
            GameObject obj = GetObjectUnderMouse();
            if (obj)
            {

                obj.GetComponent<Slot>().dropItem();

            }

        }
        if (Input.GetMouseButtonDown(0))
        {
            curSlot = GetObjectUnderMouse();


        }
        else if (Input.GetMouseButton(0))
        {
            if (curSlot && curSlot.GetComponent<Slot>().slotsItem)
            {
                followMouseImage.color = new Color(255, 2555, 255, 255);
                followMouseImage.sprite = curSlot.GetComponent<Image>().sprite;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (curSlot && curSlot.GetComponent<Slot>().slotsItem) {
                curSlotItem = curSlot.GetComponent<Slot>().slotsItem;
            curSlotItem = curSlot.GetComponent<Slot>().slotsItem;
            GameObject NewObj = GetObjectUnderMouse();
                if (NewObj && NewObj != curSlot)
                {
                    if (NewObj.GetComponent<Equipment>() && NewObj.GetComponent<Equipment>().Equipmenttype != curSlotItem.Equipmenttype)
                        return;
                    if (NewObj.GetComponent<Slot>().slotsItem)
                    {

                        Item objectsItem = NewObj.GetComponent<Slot>().slotsItem;
                        if (objectsItem.itemID == curSlotItem.itemID&&objectsItem.amountInStack != objectsItem.maxStackSize&&!NewObj.GetComponent<Equipment>())
                        {

                            curSlotItem.transform.parent = null;

                            inv.AddItem(curSlotItem, objectsItem);


                        }
                        else
                        {
                            objectsItem.transform.parent = curSlot.transform;

                            curSlotItem.transform.parent = NewObj.transform;

                        }
                    }
                    else
                    {

                        curSlotItem.transform.parent = NewObj.transform;
                    }

                }
            }

          /*  foreach(Slot i in inv.EquipSlots)
            {

                i.GetComponent<Equipment>().Equip();

            }*/
        }

        else
        {

            followMouseImage.sprite = null;
            followMouseImage.color = new Color(0, 0, 0, 0);
        }
    }
    GameObject GetObjectUnderMouse()
    {

        GraphicRaycaster rayCaster = GetComponent<GraphicRaycaster>();
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        rayCaster.Raycast(eventData, results);
        foreach(RaycastResult i in results)
        {

            if (i.gameObject.GetComponent<Slot>())
            {

                return i.gameObject;

            }

        }
        return null;


    }
}
