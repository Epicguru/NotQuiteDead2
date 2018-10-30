
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

    [MenuItem("GameObject/New Objective...", false, 10)]
    private static void OpenCreateMenu(MenuCommand cmd)
    {
        var lm = FindLevelManager();
        if(lm == null)
        {
            Debug.LogError("No level manager found, you need to create one in the scene!");
            return;
        }

        LoadTypes();

        var window = ScriptableObject.CreateInstance<LevelObjectiveEditor>();
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

        GameObject spawned = EditorUtility.CreateGameObjectWithHideFlags(t.Name, HideFlags.None, t);

        GameObjectUtility.SetParentAndAlign(spawned, parent.gameObject);
        Undo.RegisterCreatedObjectUndo(spawned, "Create level objective (" + t.Name + ")");

        var selected = Selection.activeObject;
        CompoundLevelObjective compound = null;
        if(selected != null)
        {
            if(selected is GameObject)
            {
                var go = selected as GameObject;
                var found = go.GetComponent<CompoundLevelObjective>();
                if (found != null)
                    compound = found;
            }
        }

        var comp = (LevelObjective)spawned.GetComponent(t);

        if(compound == null)
        {
            if(!parent.Objectives.Contains(comp))
                parent.Objectives.Add(comp);
        }
        else
        {
            if (!compound.Requirements.Contains(comp))
                compound.Requirements.Add(comp);
            GameObjectUtility.SetParentAndAlign(spawned, compound.gameObject);
        }
        Selection.activeObject = spawned;
    }
}
