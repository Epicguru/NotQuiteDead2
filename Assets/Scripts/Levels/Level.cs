
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Level", menuName = "Level/New Level")]
public class Level : ScriptableObject
{
    public static Dictionary<byte, Level> Loaded = new Dictionary<byte, Level>();

    /*
     * A mostly data driven class that contains info about what a level contains. 
     * A level is a particular stage in the game that has it's own goals and objectives, so is self contained.
     */

    [Tooltip("The user friendly name of this level.")]
    public string Name = "Default Level";
    [Tooltip("The unique internal ID of this level.")]
    public byte ID = 0;
    [Tooltip("The name of the Unity scene associated with this level.")]
    public string SceneName;

    public Scene Scene
    {
        get
        {
            if(_scene.name != SceneName)
            {
                _scene = SceneManager.GetSceneByName(SceneName);
            }
            return _scene;
        }
    }
    private Scene _scene;

    /// <summary>
    /// Is the level currently active? A level is considered to be active when the scene associated with it
    /// is the current loaded and active scene.
    /// </summary>
    public bool IsActive
    {
        get
        {
            return SceneManager.GetActiveScene() == Scene;
        }
    }

    /// <summary>
    /// Doesn't actually load all levels, just the data associated with them.
    /// </summary>
    public static void LoadAll()
    {
        Loaded.Clear();
        var fromDisk = Resources.LoadAll<Level>("Levels");

        byte highestID = 0;
        foreach (var l in fromDisk)
        {
            if (Loaded.ContainsKey(l.ID))
            {
                Debug.LogError("Duplicate level ID: {0} '{1}' and '{2}'".Form(l.ID, l.Name, Loaded[l.ID].Name));
            }
            else
            {
                Loaded.Add(l.ID, l);
                if(l.ID > highestID)
                {
                    highestID = l.ID;
                }
            }
        }

        Debug.Log("Loaded data for {0} levels, highest level ID is {1}".Form(Loaded.Count, highestID));
    }

    public static void UnloadAll()
    {
        Loaded.Clear();
    }

    public static bool IsLoaded(byte id)
    {
        return Loaded != null && Loaded.ContainsKey(id);
    }

    public static Level Get(byte id)
    {
        if (IsLoaded(id))
        {
            return Loaded[id];
        }
        else
        {
            Debug.LogWarning("No level for ID {0} is loaded!".Form(id));
            return null;
        }
    }
}
