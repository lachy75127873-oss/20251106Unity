using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

    [Header("SelectedSlot")] 
    private Slot _selectedSlot;

    [SerializeField] private TextMeshProUGUI selectedItemName;
    [SerializeField] private TextMeshProUGUI selectedItemDescription;
    [SerializeField] private TextMeshProUGUI selectedItemEffect;
    
    [Header("description")]
    [SerializeField] private Button UseItemButton;
    [SerializeField] private Button DropItemButton;
    [SerializeField] private Button EquipItemButton;


    private bool openInventory =false;
    private PlayerInventory playerInv;
    private PlayerCondition condition;
    
    private struct SlotView
    {
        public GameObject go;
        public Image icon;
        public TextMeshProUGUI countText;
    }
    private void OnEnable()
    {
       PlayerRaycaster.OnLookAtItem += HandleLookAtItem;
       PlayerRaycaster.OnLookAwayItem += HandleLookAway;
      
       
    }

    private void Start()
    {
            if (instance == null) instance = this;
            else Destroy(gameObject);

            playerInv = CharacterManager.Instance.Player.playerInventory;
            condition = CharacterManager.Instance.Player.playerCondition;
            
            playerInv.OnInventoryChanged += HandleChangeItem;

            if (UseItemButton)
            {
                UseItemButton.onClick.RemoveListener(OnClickUse);
                UseItemButton.onClick.AddListener(OnClickUse);
            }

            if (DropItemButton)
            {
                DropItemButton.onClick.RemoveListener(OnClickDrop);
                DropItemButton.onClick.AddListener(OnClickDrop);
            }
    }
    
    

    private void OnDisable()
    {
        PlayerRaycaster.OnLookAtItem -= HandleLookAtItem;
        PlayerRaycaster.OnLookAwayItem -= HandleLookAway;
        playerInv.OnInventoryChanged -= HandleChangeItem;
        
        UseItemButton.onClick.RemoveListener(OnClickUse);
        DropItemButton.onClick.RemoveListener(OnClickDrop);
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
        var slot = go.GetComponent<Slot>();
        
        var icon = go.GetComponentInChildren<Image>(true);
        var countText = go.GetComponentInChildren<TextMeshProUGUI>(true);
        
        //슬롯에 정보 주입
        slot.SetUp(item, playerInv.GetItemCount(item), OnSlotClicked);
        
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
    
    //슬롯UI 갱신
    private void UpdateSlotsUI(SlotView view , ItemData itemData, int newCount)
    {
        if(view.icon) view.icon.sprite = itemData.icon;
        if (view.countText) view.countText.text = newCount > 1 ? newCount.ToString() : "";
        if (view.go)
        {
            var slot = view.go.GetComponent<Slot>();
            slot.UpdateCount(newCount);//슬롯 정보 업데이트
            
            view.go.SetActive(true);
        }
    }

    private void OnSlotClicked(Slot slot)
    {
        if (_selectedSlot != null) _selectedSlot.SetSelected(false); //선택된 버튼이 있을 경우 이를 해제
        _selectedSlot =  slot;
        slot.SetSelected(true);
        ShowItemInfo(slot);
    }

    public void OnClickUse()
    {
        if (!_selectedSlot) return;
        Debug.Log("on click");

        var item = _selectedSlot.ItemData;
       
        if (item.type == ItemType.Consumable)
        {
            var effect = item.Consumables[0].itemType;
            var value = item.Consumables[0].value;
            switch (effect)
            {
                case ConsumableType.Health:
                    condition.HealHealth(value);
                    break;
                case ConsumableType.Stamina:
                    condition.HealStamina(value);
                    break;
                case ConsumableType.Hunger:
                    condition.HealHunger(value);
                    break;
                case ConsumableType.Speed:
                    condition.SpeedUp(value);
                    break;
                case ConsumableType.Jump:
                    condition.JumpUp(value);
                    break;
                default:
                    break;
            }
            playerInv.RemoveItem(item,1);
        }
    }

    public void OnClickDrop()
    {
        if (_selectedSlot == null) return;
        var item = _selectedSlot.ItemData;
        playerInv.RemoveItem(item, 1);
    }

    private void ShowItemInfo(Slot slot)
    {
        selectedItemName.text = slot.ItemData.displayName;
        selectedItemDescription.text = slot.ItemData.description;

        UseItemButton.gameObject.SetActive(false);
        EquipItemButton.gameObject.SetActive(false);
        
        if (slot.ItemData.type == ItemType.Consumable)
        {
            var itemEffectName = slot.ItemData.Consumables[0].itemType;
            var itemValue =  slot.ItemData.Consumables[0].value;
            selectedItemEffect.text = $"{itemEffectName} : +{itemValue}";
            
            UseItemButton.gameObject.SetActive(true);
        }

        else if (slot.ItemData.type == ItemType.Equipable)
        {
            var itemEffectName = slot.ItemData.Equipables[0].equipmentType;
            var itemValue =  slot.ItemData.Equipables[0].value;
            selectedItemEffect.text = $"{itemEffectName} : +{itemValue}";
            
            EquipItemButton.gameObject.SetActive(true);
        }
        
    }
    
    
    
}
