using UnityEngine;
using System.Collections.Generic;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GridTerrainGenerator : MonoBehaviour
{

    [Header("Grid Settings")]
    public int width = 100;
    public int height = 100;
    public float cellSize = 1f;
    public Vector2 originPosition = Vector2.zero;

    [Header("Terrain Settings")]
    public float noiseScale = 0.1f;
    public float sandLevel = 0.3f;  // Adjusted thresholds
    public float dertLevel = 0.7f;
    public float stoneLevel = 0.9f;

    [Header("Tile References")]
    public GameObject sandTilePrefab;
    public GameObject dertTilePrefab;
    public GameObject stoneTilePrefab;
    public GameObject ironTilePrefab;

    private Dictionary<Vector2Int, GameObject> gridTiles = new Dictionary<Vector2Int, GameObject>();

    void Start()
    {
        GenerateTerrain();
    }

    public void GenerateTerrain()
    {
        ClearTerrain();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int gridPosition = new Vector2Int(x, y);
                Vector3 worldPosition = GetWorldPosition(gridPosition);

                float noiseValue = Mathf.PerlinNoise(
                    (x + originPosition.x) * noiseScale,
                    (y + originPosition.y) * noiseScale
                );

                GameObject tilePrefab = GetTilePrefabForNoiseValue(noiseValue);
                GameObject tile = ObjectPoolingManager.instance.spawnGameObject(tilePrefab, worldPosition, Quaternion.identity);
                tile.layer = LayerMask.NameToLayer("SolidBlock");
                gridTiles.Add(gridPosition, tile);
            }
        }
    }

    private GameObject GetTilePrefabForNoiseValue(float noiseValue)
    {
        if (noiseValue < sandLevel) return sandTilePrefab;
        if (noiseValue < dertLevel) return dertTilePrefab;
        if (noiseValue < stoneLevel) return stoneTilePrefab;
        return ironTilePrefab;
    }

    public void ClearTerrain()
    {
        foreach (var tile in gridTiles.Values)
        {
            Destroy(tile);
        }
        gridTiles.Clear();
    }

    public Vector3 GetWorldPosition(Vector2Int gridPosition)
    {
        return new Vector3(
            gridPosition.x * cellSize + originPosition.x,
            gridPosition.y * cellSize + originPosition.y,
            0
        );
    }

    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        return new Vector2Int(
            Mathf.FloorToInt((worldPosition.x - originPosition.x) / cellSize),
            Mathf.FloorToInt((worldPosition.y - originPosition.y) / cellSize)
        );
    }
}