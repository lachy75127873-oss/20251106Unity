using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForLadder : MonoBehaviour
{
  
   private void OnTriggerEnter(Collider other)
   {
     var p = other.GetComponent<PlayerController> ();
     if (p != null) p.SetLadderState(true);
   }

   private void OnTriggerExit(Collider other)
   {
       var p = other.GetComponent<PlayerController> ();
       if (p != null) p.SetLadderState(false);
   }
}
