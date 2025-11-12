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

    //public ItemData itemData;

    private void Awake()
    {
        playerCondition = GetComponent<PlayerCondition>()?? gameObject.AddComponent<PlayerCondition>();
        playerController = GetComponent<PlayerController>() ?? gameObject.AddComponent<PlayerController>();
        playerInventory = GetComponent<PlayerInventory>() ?? gameObject.AddComponent<PlayerInventory>();
        myRaycaster = GetComponent<PlayerRaycaster>() ?? gameObject.AddComponent<PlayerRaycaster>();
        //CharacterManager.Instance.Player = this; 등록 방식 변경
        CharacterManager.Instance.RegisterPlayer(this); 
        
        
    }
    
}
