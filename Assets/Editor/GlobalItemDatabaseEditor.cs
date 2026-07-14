using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GlobalItemDatabase))]
public class GlobalItemDatabaseEditor : Editor
{
    private SerializedProperty _itemList;
    

    void OnEnable()
    {
        _itemList = serializedObject.FindProperty("itemList");
    }

    public override void OnInspectorGUI()
    {
        if (serializedObject.targetObject.GetType() == typeof(GlobalItemDatabase))
        {
            for (int i = 0; i < _itemList.arraySize; i++)
            {
                SerializedProperty element = _itemList.GetArrayElementAtIndex(i);
                string objectname = element.objectReferenceValue != null ? element.objectReferenceValue.name : "";
                //EditorGUILayout.PropertyField(element);
                EditorGUILayout.LabelField(element.displayName +"        "+  objectname);
            }
        }
        else
        {
        }
    }
}
