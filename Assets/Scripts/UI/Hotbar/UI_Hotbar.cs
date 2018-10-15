using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Hotbar : MonoBehaviour
{
    public static UI_Hotbar Instance;

    public Image Graphics;
    public Character Target;
    public UI_HotbarItem Prefab;
    public Transform Parent;

    private Dictionary<ItemSlot, UI_HotbarItem> spawned = new Dictionary<ItemSlot, UI_HotbarItem>();

    public void Awake()
    {
        Instance = this;
    }

    public void Update()
    {
        if(Target == null)
        {
            Clear();
            return;
        }
        if (!Target.HasHandManager)
        {
            Clear();
            return;
        }
        var onCharacter = Target.Hands.OnCharacter;
        if (onCharacter.Count != 0 || Target.Hands.Holding != null)
        {
            bool refresh = false;
            refresh = (onCharacter.Count + (Target.Hands.Holding == null ? 0 : 1)) != spawned.Count;
            if(Target.Hands.Holding != null)
            {
                if(!onCharacter.ContainsKey(Target.Hands.Holding.Slot))
                    onCharacter.Add(Target.Hands.Holding.Slot, Target.Hands.Holding);
            }         
            
            foreach (var pair in onCharacter)
            {
                if(!spawned.ContainsKey(pair.Key))
                {
                    refresh = true;
                    break;
                }

                if(spawned[pair.Key].Icon != pair.Value.Icon)
                {
                    refresh = true;
                    break;
                }
            }
            if(refresh)
                Rebuild(onCharacter);

            if (Target.Hands.Holding != null)            
                onCharacter.Remove(Target.Hands.Holding.Slot);
            
        }
        else
        {
            Clear();
        }

        // Finally, hide if nothing is spawned (there are no items to display)
        bool show = spawned.Count > 0;
        if (Graphics.enabled != show)
            Graphics.enabled = show;
    }

    public void Rebuild(Dictionary<ItemSlot, Item> items)
    {
        Clear();

        foreach (var pair in items)
        {
            var i = pair.Key;
            spawned[i] = Pool.Get(Prefab.PoolableObject).GetComponent<UI_HotbarItem>();
            spawned[i].transform.SetParent(Parent, false);
            spawned[i].Icon = items[i].Icon;
            string inputKey = "Hotbar " + char.ToUpper(i.ToString()[0]) + i.ToString().ToLower().Substring(1);
            var key = InputManager.GetInputKeys(inputKey)[0];
            spawned[i].Key = key.GetNiceName();
        }
    }

    public void Clear()
    {
        foreach (var item in spawned)
        {
            Pool.Return(item.Value.PoolableObject);
        }
        spawned.Clear();
    }
}
