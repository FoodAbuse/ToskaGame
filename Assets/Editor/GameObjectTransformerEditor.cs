using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameObjectTransformer))]
public class GameObjectTransformerEditor : Editor
{
    public SerializedProperty transformationType_prop,
        onCompletion_prop,
        gameEvent_prop,
        target_prop,
        transformMoveSpeed_prop,
        transformRotationSpeed_prop,
        transformScaleSpeed_prop,
        
        xRotation_prop,
        yRotation_prop,
        zRotation_prop,
        
        xScale_prop,
        yScale_prop,
        zScale_prop;

    public void OnEnable()
    {
        transformationType_prop = serializedObject.FindProperty("transformationType");
        onCompletion_prop = serializedObject.FindProperty("onCompletion");
        gameEvent_prop = serializedObject.FindProperty("gameEvent");
        target_prop = serializedObject.FindProperty("target");
        transformMoveSpeed_prop = serializedObject.FindProperty("transformMoveSpeed");
        transformRotationSpeed_prop = serializedObject.FindProperty("transformRotationSpeed");
        transformScaleSpeed_prop = serializedObject.FindProperty("transformScaleSpeed");
        xRotation_prop = serializedObject.FindProperty("xRotation");
        yRotation_prop = serializedObject.FindProperty("yRotation");
        zRotation_prop = serializedObject.FindProperty("zRotation");
        xScale_prop = serializedObject.FindProperty("xScale");
        yScale_prop = serializedObject.FindProperty("yScale");
        zScale_prop = serializedObject.FindProperty("zScale");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(transformationType_prop);
        // heres where we check whta kind of transformation is required.
        GameObjectTransformer.TransformationTypes tType = (GameObjectTransformer.TransformationTypes)transformationType_prop.enumValueIndex;
        // ^^ cast the transformtype prop as the enum it came from so we can use it in a switch statement
        switch (tType)
        {
            case GameObjectTransformer.TransformationTypes.Translation:
                // here we show the stuff needed for translation
                EditorGUILayout.PropertyField(target_prop);
                EditorGUILayout.PropertyField(transformMoveSpeed_prop);
                break;
            case GameObjectTransformer.TransformationTypes.Teleport:
                EditorGUILayout.PropertyField(target_prop);
                break;
            case GameObjectTransformer.TransformationTypes.RotateAround:
                EditorGUILayout.PropertyField(target_prop);
                EditorGUILayout.PropertyField(transformRotationSpeed_prop);
                EditorGUILayout.PropertyField(xRotation_prop);
                EditorGUILayout.PropertyField(yRotation_prop);
                EditorGUILayout.PropertyField(zRotation_prop);
                break;
            case GameObjectTransformer.TransformationTypes.SnapRotateAround:
                EditorGUILayout.PropertyField(target_prop);
                EditorGUILayout.PropertyField(xRotation_prop);
                EditorGUILayout.PropertyField(yRotation_prop);
                EditorGUILayout.PropertyField(zRotation_prop);
                break;
            case GameObjectTransformer.TransformationTypes.Scale:
                EditorGUILayout.PropertyField(target_prop);
                EditorGUILayout.PropertyField(transformScaleSpeed_prop);
                EditorGUILayout.PropertyField(xScale_prop);
                EditorGUILayout.PropertyField(yScale_prop);
                EditorGUILayout.PropertyField(zScale_prop);
                break;
        }
        EditorGUILayout.PropertyField(onCompletion_prop);
        EditorGUILayout.PropertyField(gameEvent_prop);
        serializedObject.ApplyModifiedProperties();
    }
}
