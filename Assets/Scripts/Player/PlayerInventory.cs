using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private PlayerInventory _playerInventory;
    
    private Dictionary<ItemData, int> inventory = new Dictionary<ItemData, int>();
    
    public Dictionary<ItemData, int> Inventory { get { return inventory; } }
    
    public event Action<ItemData , int> OnInventoryChanged;

    public void AddItem(ItemData item)
    {
        //방어
        if(item == null) return;
        
        //인밴토리 조회
        if(!inventory.TryGetValue(item,out int current)) current = 0;
        
        if (item.canStack)
            inventory[item] = Mathf.Min(current +1, item.maxStackAmount);
        else
            inventory[item] =current+1;
        
        int after = inventory[item];
        
        OnInventoryChanged?.Invoke(item,after);
    }

    public void RemoveItem(ItemData item, int amount = 1)
    {
        //잔여수량 계산
        if(!inventory.TryGetValue(item, out int itemCount)) return;
        
        int left = Mathf.Max(itemCount - amount,0);
        if (left == 0) inventory.Remove(item);
        else inventory[item] = left;
        
        OnInventoryChanged?.Invoke(item,left);
    }

    public int GetItemCount(ItemData item)
    {
        if(!inventory.TryGetValue(item, out int itemCount)) return 0;
        return inventory[item];
    }
    
}
