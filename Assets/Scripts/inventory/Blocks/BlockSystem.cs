using System;
using UnityEngine;

public class BlockSystem : MonoBehaviour
{
    [SerializeField] private BlockType[] allBlockTypes;
    [HideInInspector] public Block[] allBlocks;

    private void Awake()
    {
        allBlocks = new Block[allBlockTypes.Length];
        for (int i = 0; i < allBlockTypes.Length; i++)
        {
            var type = allBlockTypes[i];
            allBlocks[i] = new Block(type.blockID, type.blockSprite, type.blockIsSolid, type.blockPrefab);
        }
    }
}

[Serializable]
public class Block
{
    public int blockID;
    public bool isSolid;
    public int amtInInv;
    public GameObject blockPrefab;

    public Block(int id, Sprite sprite, bool solid, GameObject prefab)
    {
        blockID = id;
        isSolid = solid;
        amtInInv = 0;
        blockPrefab = prefab;
    }
}

[Serializable]
public struct BlockType
{
    public int blockID;
    public Sprite blockSprite;
    public bool blockIsSolid;
    public GameObject blockPrefab;
}