
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PoolableObject))]
public class UI_InventoryItem : MonoBehaviour
{
    public const int CELL_SIZE = 40;

    public UI_InventoryWindow InventoryWindow;
    public InventoryItem Item;
    public Image Icon;

    public float IconRotationSpeed = 10f;

    public PoolableObject PoolableObject
    {
        get
        {
            if (_pool == null)
                _pool = GetComponent<PoolableObject>();
            return _pool;
        }
    }
    private PoolableObject _pool;

    public void Init()
    {
        if (Item == null)
            return;

        float rotationTarget = Item.Rotated ? 90f : 0f;
        Icon.rectTransform.localEulerAngles = new Vector3(0f, 0f, rotationTarget);
    }

    public void Update()
    {
        if (Item == null)
            return;
        if (transform.parent == null)
            return;

        if(Icon.sprite != Item.Data.Static.Icon)
        {
            Icon.sprite = Item.Data.Static.Icon;
        }

        // Set position.
        var pos = Item.Position;

        var rt = transform as RectTransform;

        const int CELL_SIZE = UI_InventoryItem.CELL_SIZE;
        int x = pos.x;
        int y = pos.y;

        rt.anchoredPosition = new Vector2(x, -y) * CELL_SIZE;

        // Set size.
        rt.sizeDelta = Item.Size * CELL_SIZE;
        Icon.rectTransform.sizeDelta = Item.Size * CELL_SIZE - new Vector2Int(10, 10);

        // Rotate item.
        float rotationTarget = Item.Rotated ? 90f : 0f;
        Icon.rectTransform.localEulerAngles = Vector3.Lerp(Icon.rectTransform.localEulerAngles, new Vector3(0f, 0f, rotationTarget), Time.unscaledDeltaTime * IconRotationSpeed);

    }
}
