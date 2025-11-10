using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Consumable,
    Equipable,
}

public enum ConsumableType
{
    Hunger,
    Stamina,
    Health
}

[Serializable]
public class ItemDataConsumable
{
    public ConsumableType itemType;
    public float value;
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData :ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public string description;
    public ItemType type;
    public Sprite icon;
    public GameObject dropPrefabs;

    [Header("Stacking")] 
    public bool canStack;
    public int maxStackAmount;

    [Header("Consumable")] 
    public ItemDataConsumable[] Consumables;
  
    [Header("Equip")]
    public GameObject equipPrefab;
}
