
using UnityEngine;
using UnityEngine.Networking;

public class NetReference : NetworkBehaviour
{
    [ReadOnly]
    [SyncVar(hook = "UponChange")]
    [SerializeField]
    private uint ID;

    private NetworkInstanceId NetID
    {
        get
        {
            return new NetworkInstanceId(this.ID);
        }
    }

    public void Start()
    {
        if(isClient && !isServer)
            ReferenceChanged(0);
    }

    public virtual GameObject GetReference()
    {
        var id = NetID;
        if (id.IsEmpty() || id == NetworkInstanceId.Invalid)
            return null;

        return ClientScene.FindLocalObject(id);
    }

    [Server]
    public virtual void SetReference(NetworkInstanceId id)
    {
        var old = this.ID;
        this.ID = id.Value;
        ReferenceChanged(old);
    }

    public virtual void ReferenceChanged(uint old)
    {
        
    }

    private void UponChange(uint newID)
    {
        var old = this.ID;
        if(!NetworkServer.active) // isServer does not work as soon as you create the object.
            ReferenceChanged(old);
        this.ID = newID;
    }

    public sealed override float GetNetworkSendInterval()
    {
        return 0f;
    }
}
