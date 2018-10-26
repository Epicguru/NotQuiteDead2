
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
        Assembly a = Assembly.GetAssembly(typeof(LevelObjective));
        var found = from t in a.GetTypes().AsParallel()
                    where !t.IsAbstract && t.IsSubclassOf(typeof(LevelObjective))
                    select t;


    }


    [MenuItem("Level/Objectives/New...")]
    private static void OpenCreateMenu()
    {
        var lm = FindLevelManager();
        if(lm == null)
        {
            Debug.LogError("No level manager found, you need to create one in the scene!");
            return;
        }
        Debug.Log("New thingy to be added, yay... " + lm);

        var window = ScriptableObject.CreateInstance<LevelObjectiveEditor>();
        window.position = new Rect(0, 0, 200, 300);
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Add new level objective", EditorStyles.boldLabel);
    }
}
