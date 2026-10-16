using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIReporter : MonoBehaviour
{
   public enum UIType{GENERAL, OPTIONS, INVENTORY, CONTAINER,CHARACTERSHEET}
   
   public UIType uiType;
   private RectTransform _rootTarget;
   public virtual RectTransform RootTarget
   {
      get{ return _rootTarget;}
   }
   //[TODO] fix this to account ensure there is always a returned value incase a UIReporter is placed on a non Rect Trasnform
   
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
      UISystem.UIReporterRuntimeSet.Remove(this);
      Destroy(gameObject);
   }

   public virtual void UpdateUIPosition()
   {
      
   }
}
