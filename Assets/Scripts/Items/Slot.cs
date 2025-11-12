using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Outline outline;
    
    private ItemData _itemData;
    public ItemData ItemData => _itemData;
    
    private int _count;
    private Action<Slot> _onClicked;
    public int Count => _count;
    
   public void SetUp(ItemData itemData, int  count, Action<Slot> onClicked)
    {
            _itemData =  itemData;
            _count = count;
            _onClicked = onClicked;
            
            UpdateCount(count);
            SetSelected(false);
            
    }

    public void UpdateCount(int newCount)
    {
        _count  = newCount;
        if(quantityText) quantityText.text = newCount >1? newCount.ToString() : "";
        gameObject.SetActive(_count > 0);
    }

    public void SetSelected(bool selected)
    {
        if(outline)  outline.enabled = selected;
    }

    public void OnClick()
    {
        _onClicked ?. Invoke(this);
    }
   
   
    
}
