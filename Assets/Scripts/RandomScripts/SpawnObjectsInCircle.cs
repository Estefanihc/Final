using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectsInCircle : MonoBehaviour
{
    public GameObject objectToSpawn;  // The prefab to spawn
    public float radius = 5f;         // The radius of the circle
    public int numberOfObjects = 10;  // The number of objects to spawn

    void Start()
    {
        SpawnObjects();
    }

    void SpawnObjects()
    {
        float angleStep = 360f / numberOfObjects;
        float angle = 0f;

        for (int i = 0; i < numberOfObjects; i++)
        {
            float spawnPosX = transform.position.x + Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float spawnPosY = transform.position.y + Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            Vector2 spawnPosition = new Vector2(spawnPosX, spawnPosY);
            ObjectPoolingManager.instance.spawnGameObject(objectToSpawn, spawnPosition, Quaternion.identity);

            angle += angleStep;
        }
    }
}