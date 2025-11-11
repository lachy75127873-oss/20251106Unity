using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    static UiManager instance;
    public static UiManager Instance => instance;
    
    [Header("Aim")]
    [SerializeField] private GameObject aimImage;
    
    [Header("Inventory")]
    [SerializeField] private GameObject inventoryPanel;
    private bool openInventory =false;
    [SerializeField] private Transform inventoryPanelTransform;
    private int maxItemType = 12;
    [SerializeField]private GameObject slotPrefab;
    private int itemsType = 0;
    
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
        
        SetAim();
        
        OnOffInventory();
        
    }

    void SetAim()
    {
        if (!openInventory) aimImage.SetActive(true);
        else aimImage.SetActive(false);
    }

    void OnOffInventory()
    {
        
        if (Input.GetKeyDown(KeyCode.Tab))
        { ;
            openInventory = !openInventory;
           
            if(openInventory) UpdateInventory();
            
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

    void UpdateInventory()
    {
        Debug.Log("UpdateInventory");
       ArrangeSlots();
    }
    
    
    //이벤트를 통해서 UI를 껐다 킴
    public void HandleInventoryOn()
    {
        
    }

    public void HandleInventoryOff()
    {
        
    }
    
    public void ArrangeSlots()
    {
        Debug.Log("ArrangeSlots");
        int lastItemAmount = 0;
        int curItemsType = itemsType;

        var inv =(CharacterManager.Instance.Player.playerInventory.Inventory);
        lastItemAmount = inv.Count;
        
        Debug.Log(lastItemAmount);
        
        //음수 방지최대 아이템 종류 초과 방지
        if (lastItemAmount <= 0 || lastItemAmount >maxItemType)
        {
            Debug.Log("Number error"); 
            return;
        }
        
        //아이템 종류의 수가 변할 때 프리팹 수에 변화를 준다.
        if(lastItemAmount == curItemsType) return;
        if (curItemsType < lastItemAmount)
        {
            for (int i = 0; i < lastItemAmount - curItemsType; i++)
            {
                Instantiate(slotPrefab, inventoryPanelTransform);
            }
        }
        else
        {
            int toRemove = curItemsType - lastItemAmount;
            for (int k = 0; k < toRemove; k++)
            {
                int lastIndex = inventoryPanelTransform.childCount - 1;
                Destroy(inventoryPanelTransform.GetChild(lastIndex).gameObject);
            }
        }
        
        itemsType = lastItemAmount;
    }
    
    
    
}
