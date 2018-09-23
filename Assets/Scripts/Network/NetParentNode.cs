using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetParentNode : MonoBehaviour
{
    public byte ID;

    public NetworkIdentity NetworkIdentity
    {
        get
        {
            if (_nid == null)
                _nid = GetComponentInParent<NetworkIdentity>();
            return _nid;
        }
    }
    private NetworkIdentity _nid;
}
