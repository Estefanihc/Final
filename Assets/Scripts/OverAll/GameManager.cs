using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance; 
    public bool Pause;
    public GameObject Player;
   // [HideInInspector] public bool IsConsumable;
    [HideInInspector] public bool IsObject;
    [HideInInspector] public bool IsEquipment;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one GameManger in scene");
        }
        else
        {
            Instance = this;
        }
    }

    void Update()
    {
        if (Pause)
        {       
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }

       
    }
}