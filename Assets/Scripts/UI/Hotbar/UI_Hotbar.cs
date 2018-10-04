using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Hotbar : MonoBehaviour
{
    public Character Target;
    public UI_HotbarItem Prefab;
    public Transform Parent;

    private Dictionary<ItemSlot, UI_HotbarItem> spawned = new Dictionary<ItemSlot, UI_HotbarItem>();

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
        if (onCharacter.Count != 0)
        {
            bool refresh = false;
            refresh = onCharacter.Count != spawned.Count;
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
        }
        else
        {
            Clear();
        }
    }

    public void Rebuild(Dictionary<ItemSlot, Item> items)
    {
        Clear();

        foreach (var pair in items)
        {
            var i = pair.Key;
            spawned[i] = Pool.Get(Prefab.PoolableObject).GetComponent<UI_HotbarItem>();
            spawned[i].Icon = items[i].Icon;
            spawned[i].Key = i.ToString();
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
