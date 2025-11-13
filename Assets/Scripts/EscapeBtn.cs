using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapeBtn : MonoBehaviour
{
   [SerializeField] private Button _escBtn;
   [SerializeField] private GameObject informationPanel;
   private bool open = false;

   private void Start()
   {
       _escBtn.onClick.RemoveListener(OnClickEscBtn);
       _escBtn.onClick.AddListener(OnClickEscBtn);
       informationPanel.SetActive(false);
   }

   private void Update()
   {
       if (Input.GetKeyDown(KeyCode.Escape))
           OnClickEscBtn();
   }

   void OnClickEscBtn()
   {
      open=!open;
      GameManager.Instance.Info(open); 
      informationPanel.SetActive(open);
   }
   
}
