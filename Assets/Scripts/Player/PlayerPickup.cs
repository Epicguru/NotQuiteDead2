
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerPickup : MonoBehaviour
{
    public Player Player
    {
        get
        {
            if (_player == null)
                _player = GetComponent<Player>();
            return _player;
        }
    }
    private Player _player;

    public bool AllowPickup = true;

    private void Update()
    {
        if (!AllowPickup)
            return;

        var ray = MainCamera.Cam.ScreenPointToRay(InputManager.ScreenMousePos);
        var hit = Physics2D.GetRayIntersection(ray);

        if(hit.transform != null)
        {
            var item = hit.transform.GetComponentInParent<Item>();
            if(item != null)
            {
                Debug.Log(item.Name);
                if (item.Dropped)
                {
                    if (item.CanBePickedUp(Player.Character))
                    {
                        UI_ActionPopup.Display(item.Name, KeyCode.E, (Vector2)item.transform.position + Vector2.right * 1f);
                    }
                }
            }
        }
    }
}
