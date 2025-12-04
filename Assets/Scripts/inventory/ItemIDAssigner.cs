using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ItemIDAssigner : MonoBehaviour
{
    public List<ItemScriptableObject> itemsToAssign;

    private void Start()
    {
        AssignIDs();
    }
    public void AssignIDs()
    {
#if UNITY_EDITOR
        if (itemsToAssign == null || itemsToAssign.Count == 0)
        {
            Debug.LogError("No items to assign!");
            return;
        }

        // Sort by existing ID to maintain consistency
        itemsToAssign.Sort((a, b) => a.itemID.CompareTo(b.itemID));

        // Assign new IDs starting from 0
        for (int i = 0; i < itemsToAssign.Count; i++)
        {
            if (itemsToAssign[i].itemID != i)
            {
                itemsToAssign[i].itemID = i;
                EditorUtility.SetDirty(itemsToAssign[i]);
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Assigned IDs to {itemsToAssign.Count} items");
#endif
    }
}