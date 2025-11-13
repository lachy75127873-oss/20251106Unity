using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRaycaster : MonoBehaviour
{
    [Header("Interactables")]
    private Camera _camera;
    private float _distance = 3f;
    private InteractableItems lastItem;
    [SerializeField] private LayerMask interactableLayerMask;
    
    public static event Action<InteractableItems> OnLookAtItem;
    public static event Action OnLookAwayItem;

    [Header("Interact with Inventory")] 
   PlayerInventory inventory;


    private void Awake()
    {
        _camera = GetComponentInChildren<Camera>();
        Debug.Log("PlayerRaycaster Awake");
    }

    private void Update()
    {
        ShowInfoInteraction();
        
    }

    private void ShowInfoInteraction()
    {
        // UI 열려 있으면 스킵
        if(GameManager.Instance != null && GameManager.Instance.InventoryUIOpen) OnLookAwayItem?.Invoke();
        
        //감지
        Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Debug.DrawRay(ray.origin, ray.direction * 3f, Color.green);
        
        //마스크 검사
        int mask = interactableLayerMask.value == 0 ? ~0 : interactableLayerMask.value;

        if (Physics.Raycast(ray, out RaycastHit hit, _distance, mask))
        {
            if (hit.collider.TryGetComponent(out InteractableItems it) && it.itemData != null)
            {
                if (it != lastItem)
                {
                    lastItem = it;
                    
                    OnLookAtItem?.Invoke(it);
                }
                return;
            }
        }

        if (lastItem != null)
        {
            lastItem  = null;
            OnLookAwayItem?.Invoke();
        }
    }
    
    
    
}
