using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIReporter : MonoBehaviour
{
   public enum UIType{GENERAL, OPTIONS, INVENTORY, CONTAINER}
   
   public UIType uiType;
   public void OnEnable()
   {
      UISystem.UIReporterRuntimeSet.Add(this);
   }

   public void OnDisable()
   {
      UISystem.UIReporterRuntimeSet.Remove(this);
   }

   public void CloseUI()
   {
      Destroy(gameObject);
   }
}
