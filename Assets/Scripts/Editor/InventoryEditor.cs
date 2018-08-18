using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Inventory))]
public class InventoryEditor : Editor
{
    private string cache;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Inventory i = target as Inventory;

        GUILayout.Label("Contains " + i.Items.Count + " items.");
        
        if(i.OriginalData != null)
        {
            string error = null;
            if (!i.CanBasicFit(i.OriginalData.Dimensions))
            {
                error = "The item is too large to ever fit in this inventory!";
            }

            if (error != null)
                GUI.enabled = false;
            if (GUILayout.Button("Add item"))
            {
                var newItem = ItemData.CreateNew<GunItemData>(i.OriginalData.ID);
                i.InsertItem(newItem, i.TempPos, i.TempRotation);
            }
            GUI.enabled = true;

            if (error != null)
            {
                GUILayout.Label("[ERROR] " + error);
            }
        }

        if(GUILayout.Button("Print Json"))
        {
            Debug.Log(i.ToJson());
        }

        if (GUILayout.Button("Save Json"))
        {
            Debug.Log("Saved!");
            cache = i.ToJson();
        }

        if (cache == null)
            GUI.enabled = false;
        if (GUILayout.Button("Load Json"))
        {
            i.MergeJson(cache);
            Debug.Log("Loaded!");
        }
        GUI.enabled = true;

        if (i.Items.Count == 0)
            GUI.enabled = false;
        if (GUILayout.Button("Clear inventory"))
        {
            i.Items.Clear();
        }
        GUI.enabled = true;
    }
}