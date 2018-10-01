
using UnityEngine;

public class CharacterHandManager : MonoBehaviour
{
    // Manages items on the character's body that can be equipped into the hands, as well as the actual object in the hands.
    // Does not manage armour or anything like that.

    // The currently held item. A held item is, unsuprisingly, held in the hands as opposed to just
    // sitting somewhere on the characters body.
    public Item Holding;


    // The item that is really held by the character. Unlike Holding, this value is hidden.
    // This is used to remove items from the character hands with correct animations and interpolation.
    // Both the item in Holding and in CurrentlyHolding must be in the Equiped array.
    private Item currentlyHolding;

    [Header("References")]
    public Transform HoldPoint;
    public HandTracker Left;
    public HandTracker Right;

    public void Update()
    {
        var Holding = this.Holding;

        // First, check if the Holding value is the same as the currentlyHolding value.
        // If they are the same, ensure that the character's hands are moving towards the currentlyHolding.
        if(currentlyHolding == Holding)
        {
            // Only if the item is not null.
            if(currentlyHolding != null)
            {
                Left.Target = currentlyHolding.LeftHand;
                Right.Target = currentlyHolding.RightHand;

                // If both hands are at the target positon, then set the item state to not stored.
                // This will cause the equip animation to play.
                bool held = Left.AtTarget && Right.AtTarget;

                if (held && currentlyHolding.Stored)
                {
                    currentlyHolding.InHands = true;
                }
            }
            else
            {
                // We aren't holding anything!
                // Set the hand targets to null, making the move back to the idle position.
                Left.Target = null;
                Right.Target = null;
            }
        }
        else
        {
            // The exposed Holding value is different to the currentlyHolding value.
            // This could mean:
            // 1. The Holding is a different item to currentlyHolding.
            // 2. The currentlyHolding value is null and the Holding is not.
            // 3. The Holding is null.
            // 4. The currentlyHolding is null?
            
            // The fourth option should never happen, because if the currently held item is destroyed or removed, then
            // so should the exposed Holding value be, making both of them null.
            
            if(Holding != null && currentlyHolding != null) // 1. Are they both not null?
            {
                // We need to swap items...
                // Store the current item by setting it's state to stored. Once it is completely stored,
                // set the value of currentlyHolding to Holding. This causes the hands to move towards the target item,
                // and equip it by setting it's state to not stored.

                // Sometimes, the animation and code don't link up quite perfectly. For a few frames, once the hands reach
                // the item and the Stored state is set to false, the animation state will still be 'stored'. This slightly
                // confuses the script and causes the gun to equip and dequip with no hands following it.
                // To prevent this, we check the conflicted flag. If it is conflicting, then we don't equip the new item yet.
                bool conflicted = !currentlyHolding.Stored && currentlyHolding.CurrentlyStored;

                if(!conflicted)
                    currentlyHolding.InHands = false;

                if (currentlyHolding.CurrentlyStored && !conflicted)
                {
                    // Now the item is completely stored on the character (on back, waist, invisible, whatever).
                    // Now set the current item to the target (Holding) item, which causes it to be equipped.
                    currentlyHolding = Holding;
                }
            }
            else if(currentlyHolding == null && Holding != null) // 2. We have nothing in our hands, but we want to equip something.
            {
                // Easy. Set the value of currentlyHolding to Holding, causing the hands to move to the target and equip it.
                currentlyHolding = Holding;
            }
            else if(currentlyHolding != null && Holding == null) // 3. We want to store the current item. No new item.
            {
                // We need to store the current item.
                // We set the current item state to stored, to play the dequip animation.
                // Once the item is completely stored (we can check that), then we set the currentlyHolding to
                // null. This makes both Holding and currentlyHolding null, causing the hands to move to an idle position.
                currentlyHolding.InHands = false;
                if (currentlyHolding.CurrentlyStored)
                {
                    // Now the item is completely stored on the character (on back, waist, invisible, whatever) so
                    // we set the current item to null.
                    currentlyHolding = null;
                }
            }
            else
            {
                // Something went wrong.
                Debug.LogError("Something went wrong in the item handling code!");
            }
        }
    }

    /// <summary>
    /// Grabs an item in the game world and places it on the character. It is NOT put into the character's hands.
    /// See EquipItem to put a stored item into the hands.
    /// </summary>
    /// <param name="item">The existing, spawned, item.</param>
    public void StoreItem(Item item)
    {
        if (item == null)
            return;

        if (item.Stored)
        {
            Debug.LogWarning("Item '{0}' cannot be stored, it is already stored by this character ({1}) or another character!".Form(item.Name, name));
        }
        else if (!item.Dropped)
        {
            Debug.LogWarning("Item '{0}' is not dropped, cannot store on this ({1}) character!".Form(item.Name, name));
        }
        else
        {
            item.InHands = false;
            item.Dropped = false;
            item.OnCharacter = true;
            item.transform.SetParent(this.HoldPoint);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
        }
    }

    public void EquipItem(Item item)
    {
        if(item == null)
        {
            Debug.LogError("Null item to equip!");
            return;
        }

        // Dropped items cannot be directly equipped into hands.
        if (item.Dropped)
        {
            StoreItem(item);
        }

        if (item.InHands)
            return;

        //Debug.Log("Equipping item... " + item);
        Holding = item;
    }

    public void DequipCurrent()
    {
        if (Holding == null)
            return;

        Holding = null;
    }

    public void DropCurrent()
    {
        if (Holding == null)
            return;

        Holding.OnCharacter = false;
        Holding.InHands = false;
        Holding.Dropped = true;

        if(currentlyHolding == Holding)
        {
            currentlyHolding = null;
        }
        Holding.transform.SetParent(null);
        Holding = null;
    }
}

public static class HandCommands
{
    [DebugCommand("On the player, put the item into their hands. Assumes that the item is already stored on their body.", GodModeOnly = true, Parameters = "STRING:name:The name of the item to equip.")]
    public static void EquipItem(string name)
    {
        if (Player.Character == null)
        {
            Commands.LogError("Player does not have control of any character!");
            return;
        }

        var parent = Player.Character.Hands.HoldPoint.transform;

        var items = parent.GetComponentsInChildren<Item>();
        Item found = null;
        foreach (var item in items)
        {
            if (item.Name.ToLower().Trim() == name.Trim().ToLower())
            {
                found = item;
                break;
            }
        }

        if (found != null)
        {
            Player.Character.Hands.EquipItem(found);
            Commands.Log("Equipped item '{0}'.".Form(found.Name));
        }
        else
        {
            Commands.LogError("Item not found stored on player. Check spelling, and make sure the item is already stored.");
        }
    }

    [DebugCommand("Dequips (stores) the currently held item on the player.", GodModeOnly = true)]
    public static void DequipItem()
    {
        if (Player.Character == null)
        {
            Commands.LogError("Player does not have control of any character!");
            return;
        }

        if(Player.Character.Hands.Holding == null)
        {
            Commands.Log("Nothing to dequip.");
        }
        else
        {
            Commands.Log("Dequipped the {0}.".Form(Player.Character.Hands.Holding.Name));
            Player.Character.Hands.DequipCurrent();
        }
    }

    [DebugCommand("Dequips (stores) the currently held item on the player.", GodModeOnly = true)]
    public static void DropItem()
    {

        var target = Player.Character;
        if (target == null)
        {
            Commands.LogError("Local player does not have control of any character!");
            return;
        }

        if (target.Hands.Holding == null)
        {
            Commands.Log("Nothing to drop.");
        }
        else
        {
            Commands.Log("Dropped the {0}.".Form(target.Hands.Holding.Name));
            target.Hands.DropCurrent();
        }
    }
}