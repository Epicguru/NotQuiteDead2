
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class LevelObjectiveEditor : EditorWindow
{
    private static LevelManager lm;
    private static LevelManager FindLevelManager()
    {
        if(lm == null)
            lm = GameObject.FindObjectOfType<LevelManager>();

        return lm;
    }

    private static List<Type> types = new List<Type>();
    private static void LoadTypes()
    {
        Type ty = typeof(LevelObjective);
        Assembly a = Assembly.GetAssembly(ty);
        var found = from t in a.GetTypes().AsParallel()
                    where !t.IsAbstract && t.IsSubclassOf(ty)
                    select t;

        types.Clear();
        foreach (var item in found)
        {
            types.Add(item);
        }
    }

    [MenuItem("Level/New Objective...")]
    private static void OpenCreateMenu()
    {
        var lm = FindLevelManager();
        if(lm == null)
        {
            Debug.LogError("No level manager found, you need to create one in the scene!");
            return;
        }
        Debug.Log("New thingy to be added, yay... " + lm);

        LoadTypes();

        var window = ScriptableObject.CreateInstance<LevelObjectiveEditor>();
        window.position = new Rect(0, 0, 200, 300);
        window.ShowUtility();
    }

    private Vector2 scroll;
    private void OnGUI()
    {
        EditorGUILayout.LabelField("Add new level objective", EditorStyles.boldLabel);
        EditorGUILayout.Separator();
        scroll = EditorGUILayout.BeginScrollView(scroll);
        foreach (var t in types)
        {
            bool triggered = GUILayout.Button(t.FullName);
            if (triggered)
            {
                AddType(t);
                this.Close();
            }
        }
        EditorGUILayout.EndScrollView();
    }

    private void AddType(Type t)
    {
        if (t == null)
            return;

        var parent = FindLevelManager();
        if(parent == null)
        {
            EditorUtility.DisplayDialog("Error", "No Level Manager GameObject found in the scene.", "Ok");
            return;
        }

        GameObject go = new GameObject(t.Name, t);
        go.transform.SetParent(parent.transform, false);
        go.transform.localPosition = Vector3.zero;
    }
}
