using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public static GameManager Instance {get {return instance;}}
    
    public bool InventoryUIOpen {get; private set;}
    

    private void Awake()
    {
        if (Instance == null) instance =  this;
        else Destroy(gameObject);
    }

    public void Inventory(bool openInventory)
    {
        InventoryUIOpen = openInventory;
        if(openInventory)Time.timeScale = 0;
        else Time.timeScale = 1;
    }
    
}
