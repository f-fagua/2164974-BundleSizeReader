using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestMonobehaviour))]
public class TestMonobehaviourEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var referencesProperty = serializedObject.FindProperty("MyReferences");
        for (int i = 0; i < referencesProperty.arraySize; i++)
        {
            var arrayElement = referencesProperty.GetArrayElementAtIndex(i);

                // Set the new display name.
            EditorGUILayout.PropertyField(arrayElement, new GUIContent($"Custom Property Name {i}"));
        }
    }
}
