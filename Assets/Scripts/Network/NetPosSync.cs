using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetPosSync : NetworkBehaviour
{
    // Synchronises the position of objects across the clients, with authority residing with the server.
    // Uses rigidbodies to do this, or transform if there is no rigidbody.

    [Tooltip("The frequency at which updates on the position, velocity and rotation are sent to clients.")]
    public float UPS = 25f;
    public float UpdateInterval
    {
        get
        {
            return 1f / UPS;
        }
    }

    public bool HasBody
    {
        get
        {
            return Rigidbody != null;
        }
    }

    public Rigidbody2D Rigidbody
    {
        get
        {
            if (_body == null)
                _body = GetComponent<Rigidbody2D>();
            return _body;
        }
    }
    private Rigidbody2D _body;

    private Vector2 bodyPos;
    private Vector2 bodyVel;
    private Vector2 bodyAngle; // Where x is current angle and y is angular velocity.

    private float timer = 0f;

    public override float GetNetworkSendInterval()
    {
        // Send updates whenever the variables are updated.
        return 0f;
    }

    public void Update()
    {
        timer += Time.unscaledDeltaTime;

        if (isServer)
        {
            // Send updates if it has a rigidbody.
            if (HasBody)
            {
                if(timer >= UpdateInterval)
                {
                    timer = 0f;
                    Vector2 angle = new Vector2(Rigidbody.rotation, Rigidbody.angularVelocity);

                    if(bodyPos != Rigidbody.position)                    
                        bodyPos = Rigidbody.position;
                    if (bodyVel != Rigidbody.velocity)
                        bodyVel = Rigidbody.velocity;
                    if (bodyAngle != angle)
                        bodyAngle = angle;                    
                }
            }
        }
    }
}
