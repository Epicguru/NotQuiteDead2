using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetParentSync : NetRef
{
    [SerializeField]
    [ReadOnly]
    private byte NodeID;

    [Server]
    public void SetParent(NetParentNode node)
    {
        if(node == null)
        {
            base.SetReference(0);
            transform.parent = null;
        }
        else
        {
            if (transform.parent != node.transform)
                transform.parent = node.transform;

            if (node.NetworkIdentity == null)
                Debug.LogError("Cannot parent to node '{0}', it has no parent or attached NetworkIdentity".Form(node.name));
            else
                base.SetReference(node.NetworkIdentity.netId);
            if (NodeID != node.ID)
                NodeID = node.ID;
        }
    }

    public void Update()
    {
        if (isServer)
            return;

        if (!base.HasRefValue)
        {
            if(transform.parent != null)
                transform.parent = null;
            return;
        }

        var obj = base.Value;
        if(transform.parent != obj.transform)
        {
            transform.parent = obj.transform;
        }

        // Object is null but we have a ref value. Wait until the object is found.
        if (obj == null)
            return;

        var node = GetComponentInParent<NetParentNode>();
        bool findCorrect = false;
        if(node == null)
        {
            findCorrect = true;
        }
        else
        {
            if(node.ID != NodeID)
            {
                // Not on the right node.
                findCorrect = true;
            }
        }

        if (!findCorrect)
            return;

        var nodes = GetComponentsInParent<NetParentNode>();
        NetParentNode found = null;
        foreach (var item in nodes)
        {
            if (item.ID == NodeID)
            {
                found = item;
                break;
            }
        }

        if (found != null)
        {
            transform.parent = found.transform;
        }
    }
}
