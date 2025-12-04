using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class block_health : MonoBehaviour
{
    public float maxhealth = 20;   
    public float currenthealth;
    public int blockID;
    public int ItemID;
    void Start()
    {

    
        currenthealth = maxhealth;
    
    }

    void Update()
    {

        if (currenthealth <= 0)
        {
         GameObject BlockItem =   new GameObject();
            BlockItem.transform.position = transform.position;
            BlockItem.AddComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
            BlockItem.transform.localScale = new Vector2(.5f,.5f);
            BlockItem.AddComponent<BoxCollider2D>();
            BlockItem.AddComponent<Rigidbody2D>();
            BlockItem.AddComponent<Item>();


            foreach(ItemScriptableObject  itemData in inventory.Instance.Allitems)
            {
               if(itemData.itemID == ItemID)
                {
                    BlockItem.GetComponent<Item>().itemData = itemData;
                }
            }
            Destroy(gameObject);

        }

      
    }



  

    public void TakeDamage(float damage)
    {

      

            currenthealth -= damage;
          


      

    }
   
}
