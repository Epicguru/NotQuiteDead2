using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetRef : NetworkBehaviour
{
    // A reference to a game object that is synched over the network.
    // Reference can only be set on the server.

    [SerializeField]
    [ReadOnly]
    [SyncVar]
    private uint ID;

    private NetworkInstanceId NetID
    {
        get
        {
            return new NetworkInstanceId(ID);
        }
    }

    public bool HasRefValue
    {
        get
        {
            return ID != 0 && NetID != NetworkInstanceId.Invalid;
        }
    }

    public GameObject Value
    {
        get
        {
            if(_value == null)
            {
                if (HasRefValue)
                {
                    // Object is null but reference is not.
                    _value = ClientScene.FindLocalObject(NetID);
                }
            }
            else
            {
                if (!HasRefValue)
                {
                    // Object is not null, but reference is.
                    _value = null;
                }
                else
                {
                    // Ref value is not null, and neither is reference, but we need to make sure we have the right object.
                    var net = _value.GetComponent<NetworkIdentity>();
                    if(net == null)
                    {
                        // For some reason, this happened!
                        Debug.LogError("Object with no net id is referenced by this NetRef. Was the component removed?");
                        _value = null;
                    }
                    else
                    {
                        // Compare IDs.
                        if(net.netId != NetID)
                        {
                            // They don't match!
                            // To fix this, dispose the game object and run this property again.
                            _value = null;
                            return Value;
                        }
                    }
                }
            }
            return _value;
        }
    }
    [SerializeField]
    [ReadOnly]
    private GameObject _value;

    public override float GetNetworkSendInterval()
    {
        return 0f;
    }

    [Server]
    public void SetReferenceObj<T>(T obj) where T : NetworkBehaviour
    {
        if (obj == null)
            SetReference(0);
        else
            SetReference(obj.netId);
    }

    [Server]
    public void SetReference(NetworkInstanceId id)
    {
        this.SetReference(id.Value);
    }

    [Server]
    public void SetReference(uint id)
    {
        if(ID != id)
        {
            ID = id;
        }
    }
}
