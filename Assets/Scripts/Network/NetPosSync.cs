using UnityEngine;
using UnityEngine.Networking;

public class NetPosSync : NetworkBehaviour
{
    // Synchronises position and rotation across all clients.
    // Normally is server authorative, but can also optionally be client authrative, when the script
    // has local player authority.

    [Range(0f, 50f)]
    public float UPS = 20f;

    public float LerpSpeed = 20f;

    public bool HasBody
    {
        get
        {
            return Body != null;
        }
    }

    public Rigidbody2D Body
    {
        get
        {
            if (_body == null)
                _body = GetComponent<Rigidbody2D>();
            return _body;
        }
    }
    private Rigidbody2D _body;

    [SyncVar(hook = "PosChange")] private Vector2 Pos;
    [SyncVar(hook = "VelChange")] private Vector2 Vel;
    [SyncVar(hook = "RotChange")] private Vector2 Rot;

    public override float GetNetworkSendInterval()
    {
        return UPS == 0f ? 0f : 1f / UPS;
    }

    public void Update()
    {
        // The data must be synched, but from where and to who?
        UpdateSending();
        UpdateSync();    
    }

    private void UpdateSending()
    {
        if (!hasAuthority)
            return;

        // We have authority, but we might also be on the server...
        if (isServer)
        {
            if (HasBody)
            {
                // If on the server, just send rigidbody position, rotation.
                if (Pos != Body.position)
                    Pos = Body.position;
                if (Vel != Body.velocity)
                    Vel = Body.velocity;
                Vector2 br = new Vector2(Body.rotation, Body.angularVelocity);
                if (Rot != br)
                    Rot = br;
            }
            else
            {
                // Send regular transform info, there is no velocity to send.
                Vector2 rot = new Vector2(transform.localEulerAngles.z, 0f);
                if (Pos != (Vector2)transform.localPosition)
                    Pos = (Vector2)transform.localPosition;
                if (Rot != rot)
                    Rot = rot;
            }
        }
        else
        {
            // Not on server, but has authority! This means that we want to sync position from this particular client,
            // to the server, and finally to all other clients.

            if (HasBody)
            {
                Vector2 rot = new Vector2(Body.rotation, Body.angularVelocity);
                bool send = Pos != Body.position || Vel != Body.velocity || Rot != rot;
                if (send)
                {
                    CmdSendData(Body.position, Body.velocity, rot);
                }
            }
            else
            {
                Vector2 rot = new Vector2(transform.localEulerAngles.z, 0f);
                bool send = Pos != (Vector2)transform.localPosition || Rot != rot;
                if (send)
                {
                    CmdSendData((Vector2)transform.localPosition, Vector2.zero, rot);
                }
            }
        }
    }

    private void UpdateSync()
    {
        // Takes data sent from the server, and applies it to the client.
        // The server also has to do a bit of processing too to make sure everything stays in sync.
        
        if (isServer)
        {
            if (HasBody)
            {
                Body.angularVelocity = Rot.y;
                Body.velocity = Vel;
            }
            return;
        }

        if (HasBody)
        {
            // Note that only velocities are applied constantly: the position and rotation are applied once received.
            Body.angularVelocity = Rot.y;
            Body.velocity = Vel;
        }
        else
        {
            // Just lerp towards the most recent values.
            transform.localPosition = Vector2.Lerp(transform.localPosition, Pos, Time.unscaledDeltaTime * LerpSpeed);
            var r = transform.localEulerAngles;
            r.z = Mathf.LerpAngle(r.z, Rot.x, Time.unscaledDeltaTime * LerpSpeed);
            transform.localEulerAngles = r;
        }
    }

    private void VelChange(Vector2 newVel)
    {
        this.Vel = newVel;

        // If we don't have authority and are not on the server...
        if (isServer)
            return;
        if (hasAuthority)
            return;

        if (HasBody)
        {
            Body.velocity = Vel;
        }
    }

    private void PosChange(Vector2 pos)
    {
        this.Pos = pos;

        // If we don't have authority and are not on the server...
        if (isServer)
            return;
        if (hasAuthority)
            return;

        if (HasBody)
        {
            Body.MovePosition(Pos);
        }
    }

    private void RotChange(Vector2 newRot)
    {
        this.Rot = newRot;

        // If we don't have authority and are not on the server...
        if (isServer)
            return;
        if (hasAuthority)
            return;

        if (HasBody)
        {
            Body.rotation = Rot.x;
            Body.angularVelocity = Rot.y;
        }
    }

    [Command]
    private void CmdSendData(Vector2 pos, Vector2 vel, Vector2 rot)
    {
        this.Pos = pos;
        this.Vel = vel;
        this.Rot = rot;

        // Apply.
        if (HasBody)
        {
            Body.MovePosition(pos);
            Body.MoveRotation(rot.x);
            Body.velocity = vel;
            Body.angularVelocity = rot.y;
        }
        else
        {
            // Apply directly to the transform: note that this gives a jerky look on the the server side when sent from a client: TODO fix (just interpolate in update)
            transform.localPosition = pos;
            var r = transform.localEulerAngles;
            r.z = rot.x;
            transform.localEulerAngles = r;
        }
    }
}
