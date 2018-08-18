
using UnityEngine;

public class InventoryTests : MonoBehaviour
{
    public Inventory Inventory;

    public void Start()
    {
        Inventory = ScriptableObject.CreateInstance<Inventory>();
    }
}
