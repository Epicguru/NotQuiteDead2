
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
                if (item.Dropped)
                {
                    if (item.CanBePickedUp(Player.Character))
                    {
                        UI_ActionPopup.Display(item.Name, InputManager.GetInputKeys("Action")[0], (Vector2)item.transform.position + Vector2.right * 0.2f);
                        if (InputManager.IsDown("Action"))
                        {
                            // If we already have an item of that slot type, drop the current one.
                            if (Player.Character.Hands.OnCharacter.ContainsKey(item.Slot))
                            {
                                Player.Character.Hands.DropStored(item.Slot);
                            }
                            if(Player.Character.Hands.Holding != null && Player.Character.Hands.Holding.Slot == item.Slot)
                            {
                                Player.Character.Hands.DropCurrent();
                            }

                            // Pick up the item...
                            // First, store in on the character body.
                            Player.Character.Hands.StoreItem(item);

                            // If the hands are empty, the put the item into the hands.
                            if(Player.Character.Hands.Holding == null)
                            {
                                Player.Character.Hands.EquipItem(item);
                            }
                        }
                    }
                }
            }
        }
    }
}
