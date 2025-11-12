using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    static UiManager instance;
    public static UiManager Instance => instance;
    
    [Header("Aim")]
    [SerializeField] private GameObject aimImage;
    
    [Header("Inventory")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform inventoryPanelTransform;
    [SerializeField]private GameObject slotPrefab;
    
    private readonly Dictionary<ItemData, SlotView> slots = new (); //UI갱신용 캐시 dictionary
    private int maxItemType = 12;
    private int itemsType = 0;
    
    [Header("Interactables")]
    [SerializeField] private TextMeshProUGUI interactablesText;

    private bool openInventory =false;
    private PlayerInventory playerInv;
    
    private struct SlotView
    {
        public GameObject go;
        public Image icon;
        public TextMeshProUGUI countText;
    }

    private void Start()
    {
            if (instance == null) instance = this;
            else Destroy(gameObject);

            playerInv = CharacterManager.Instance.Player.playerInventory;
    }
    
    
    private void OnEnable()
    {
       PlayerRaycaster.OnLookAtItem += HandleLookAtItem;
       PlayerRaycaster.OnLookAwayItem += HandleLookAway;
       playerInv.OnInventoryChanged += HandleChangeItem;
    }

    private void OnDisable()
    {
        PlayerRaycaster.OnLookAtItem -= HandleLookAtItem;
        PlayerRaycaster.OnLookAwayItem -= HandleLookAway;
        playerInv.OnInventoryChanged -= HandleChangeItem;
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
        Debug.Log("OnOffInventory");
        if (Input.GetKeyDown(KeyCode.Tab))
        { ;
            Debug.Log("Tab");
            openInventory = !openInventory;
           
            if(openInventory) RebuildSlots();
            
            inventoryPanel.SetActive(openInventory);
            
            Cursor.lockState = openInventory ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = openInventory;
            
            GameManager.Instance.Inventory(openInventory);
            
        }
        
    }


    public void HandleLookAtItem(InteractableItems item)
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

    //인밴토리 갱신
    private void HandleChangeItem(ItemData item,int newCount )
    {
        if(item == null) return; //방어

        if (newCount <= 0) //아이템 소진 시
        {
            RemoveSlots(item); //아이템 슬롯 재거
            itemsType = Mathf.Min(maxItemType, itemsType);
            return;
        }

        if (!slots.TryGetValue(item, out var view)) //새로운 아이템일 경우
        {
            view = CreateSlots(item); //새로운 슬롯 생성
        }
        
        UpdateSlotsUI(view,item,newCount); //인밴토리 UI에 정보 갱신
        
        itemsType = Mathf.Min(maxItemType, itemsType);
    
    }

    //인밴토리 열람시 리빌드 
    private void RebuildSlots()
    {
        //청소
        foreach (var kv in slots)
            if(kv.Value.go) Destroy(kv.Value.go);
        slots.Clear();

        int countType = 0;
        foreach (var kv in playerInv.Inventory) //리빌딩
        {
            if(kv.Key == null || kv.Value <=0) continue; //빈 데이터
            if(countType >= maxItemType) break; //종류 초과 방지

            var view = CreateSlots(kv.Key); 
            UpdateSlotsUI(view, kv.Key, kv.Value);
            countType++;

            itemsType = countType;
        }


    }

    //슬롯 생성
    private SlotView CreateSlots(ItemData item)
    {
        //방어
        if (!slotPrefab || !inventoryPanelTransform)
        {
            Debug.Log("Missing Error");
            return default;
        } 
        
        //아이템 종류 초과
        if (slots.Count >= maxItemType)
        {
            Debug.Log("MaxItemType Error");
            return default;
        }
        
        //슬롯 변수 준비
        var go = Instantiate(slotPrefab, inventoryPanelTransform);
        var icon = go.GetComponentInChildren<Image>(true);
        var countText = go.GetComponentInChildren<TextMeshProUGUI>(true);
        
        //캐시데이터 갱신
        var view = new SlotView { go = go, icon = icon, countText = countText };
        slots[item] = view;
        
        //리빌드 데이터 반환
        return view;
    }
    
    //슬롯 제거
    private void RemoveSlots(ItemData item)
    {
        //방어
        if (!slots.TryGetValue(item, out var view)) return ;
        
        //프리펩 파괴
        if(view.go) Destroy(view.go);
        
        //캐시데이터 갱신
        slots.Remove(item);
    }
    
    //슬롯UI
    private void UpdateSlotsUI(SlotView view , ItemData itemData, int newCount)
    {
        if(view.icon) view.icon.sprite = itemData.icon;
        if (view.countText) view.countText.text = newCount > 1 ? newCount.ToString() : "";
        if (view.go) view.go.SetActive(true);
    }
    
    

    void UpdateInventory()
    {
        Debug.Log("UpdateInventory");
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
