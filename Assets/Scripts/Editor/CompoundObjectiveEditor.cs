using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System.Collections;

[CustomEditor(typeof(CompoundLevelObjective))]
public class CompoundObjectiveEditor : Editor
{
    private ReorderableList list;

    public void OnEnable()
    {
        list = new ReorderableList(serializedObject, serializedObject.FindProperty("Requirements"), true, true, true, true);

        list.drawHeaderCallback = rect => {
            EditorGUI.LabelField(rect, "Requirements", EditorStyles.boldLabel);
        };

        list.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
            };
        list.displayAdd = false;
        list.displayRemove = false;
        list.onChangedCallback += OnThingChanged;
    }

    private void OnThingChanged(ReorderableList list)
    {
        Debug.Log("Changed!");
        foreach (var item in list.list)
        {
            var comp = item as Component;
            if (!comp.transform.IsChildOf((serializedObject.targetObject as Component).transform))
            {
                list.list.Remove(item);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}