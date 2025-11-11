using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    static UiManager instance;
    public static UiManager Instance => instance;
    
    [Header("Inventory")]
    [SerializeField] private GameObject inventoryPanel;
    private bool openInventory =false;
    
    [Header("Interactables")]
    [SerializeField] private TextMeshProUGUI interactablesText;

    private void OnEnable()
    {
       PlayerRaycaster.OnLookAtItem += HandleLookAtItem;
       PlayerRaycaster.OnLookAwayItem += HandleLookAway;
    }

    private void OnDisable()
    {
        PlayerRaycaster.OnLookAtItem -= HandleLookAtItem;
        PlayerRaycaster.OnLookAwayItem -= HandleLookAway;
    }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            openInventory = !openInventory;
            inventoryPanel.SetActive(openInventory);
            
            Cursor.lockState = openInventory ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = openInventory;
            
            GameManager.Instance.Inventory(openInventory);
            
        }
    }


    void HandleLookAtItem(InteractableItems item)
    {
        if (interactablesText == null )return;
        interactablesText.text = $"{item.itemData.name}\n{item.itemData.description}";
        interactablesText.gameObject.SetActive(true);
    }

    public void HandleLookAway()
    {
        if(interactablesText == null ) return;
        interactablesText.gameObject.SetActive(false);
        interactablesText.text = "";
    }
    
    
}
