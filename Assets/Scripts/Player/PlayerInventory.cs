using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    PlayerInventory _playerInventory{get;set;}
    
    public event Action OnInventoryChanged;
    
    private Dictionary<ItemData, int> inventory = new Dictionary<ItemData, int>();
    
    public Dictionary<ItemData, int> Inventory { get { return inventory; } }

    public void AddItem(ItemData item, int amount = 1)
    {
        //방어
        if(item == null||amount <= 0) return;
        
        //인밴토리 조회
        if(!inventory.TryGetValue(item,out int current)) current = 0;
        
        if (item.canStack)
            inventory[item] = Mathf.Min(current + amount, item.maxStackAmount);
        else
            inventory[item] =current+ amount;
        
        OnInventoryChanged?.Invoke();
    }

    public void RemoveItem(ItemData item, int amount = 1)
    {
        //잔여수량 계산
        if(!inventory.TryGetValue(item, out int itemCount)) return;
        int left = itemCount - amount;
        
        if(left >0) inventory[item] = left;
        else inventory.Remove(item);
        OnInventoryChanged?.Invoke();
        
        
        
        
    }
    
    
}
