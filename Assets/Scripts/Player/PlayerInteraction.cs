using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private InteractableItems currentItem;
    private PlayerInventory _playerInventory;

    private void Awake()
    {
        _playerInventory = GetComponent<PlayerInventory>();
        if(_playerInventory == null) Debug.Log("No player inventory");
    }

    private void OnEnable()
    {
        PlayerRaycaster.OnLookAtItem += HandleLookAt;
        PlayerRaycaster.OnLookAwayItem += HandleLookAway;
    }

    private void OnDisable()
    {
        PlayerRaycaster.OnLookAtItem -= HandleLookAt;
        PlayerRaycaster.OnLookAwayItem -= HandleLookAway;
    }

    private void HandleLookAt(InteractableItems item)
    {
        currentItem = item;
    }
    private void HandleLookAway()
    {
        currentItem=null;
    }
    
    
    private void GetItem(InteractableItems item)
    {
        if( GameManager.Instance.InventoryUIOpen) return;
        
        if (currentItem && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Try Getting item");
            _playerInventory.AddItem(currentItem.itemData ,currentItem.quantity);
            Destroy(currentItem.gameObject);
            UiManager.Instance.HandleLookAway();
            
            currentItem = null;
        }
    }

    private void Update()
    {
        GetItem(currentItem);
    }
}
