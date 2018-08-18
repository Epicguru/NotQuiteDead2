using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Inventory))]
public class InventoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Inventory i = target as Inventory;

        GUILayout.Label("Contains " + i.Items.Count + " items.");

        string error = null;
        if (!i.CanBasicFit(i.TempItem))
        {
            error = "The item is too large to ever fit in this inventory!";
        }

        if (error != null)
            GUI.enabled = false;
        if(GUILayout.Button("Add item"))
        {
            i.InsertItem(new ItemData() { Dimensions = i.TempItem, ID = 0, Name = "Cool thing." }, i.TempPos, i.TempRotation);
        }
        GUI.enabled = true;

        if(error != null)
        {
            GUILayout.Label("[ERROR] " + error);
        }

        if (i.Items.Count == 0)
            GUI.enabled = false;
        if (GUILayout.Button("Clear inventory"))
        {
            (target as InputManagerGameObject).SaveToFile();
        }
        GUI.enabled = true;
    }
}