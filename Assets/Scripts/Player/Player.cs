using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{ 
    public PlayerCondition playerCondition { get; private set; }                 
   public PlayerController playerController { get; private set; }                 
   public PlayerInventory playerInventory { get; private set; }
   public PlayerRaycaster myRaycaster { get; private set; }                 

    public ItemData itemData;

    private void Awake()
    {
        playerCondition = GetComponent<PlayerCondition>();
        if (playerCondition == null) playerCondition = gameObject.AddComponent<PlayerCondition>();
        playerController = GetComponent<PlayerController>();
        if (playerController == null) playerController = gameObject.AddComponent<PlayerController>();
        playerInventory = GetComponent<PlayerInventory>();
        if (playerInventory == null) playerInventory = gameObject.AddComponent<PlayerInventory>();
        myRaycaster = GetComponent<PlayerRaycaster>();
        if (myRaycaster == null) myRaycaster = gameObject.AddComponent<PlayerRaycaster>();

        CharacterManager.Instance.Player = this;
    }

    
}
