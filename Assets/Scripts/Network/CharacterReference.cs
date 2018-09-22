using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class CharacterReference : NetReference
{
    [SerializeField]
    [ReadOnly]
    private Character _char;

    public override void ReferenceChanged(uint old)
    {
        var go = base.GetReference();
        if(go != null)
        {
            _char = go.GetComponent<Character>();
        }
        else
        {
            _char = null;
        }

        Debug.LogWarning("Updated character to " + _char);
    }

    public Character GetCharacter()
    {
        return _char;
    }

    [Server]
    public void SetCharacter(Character c)
    {
        if (c == null)
            this.SetReference(NetworkInstanceId.Invalid);
        else
            this.SetReference(c.netId);
    }
}
