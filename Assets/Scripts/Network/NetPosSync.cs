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

    [SyncVar(hook = "PosChange")] private Vector2 bodyPos;
    [SyncVar(hook = "VelChange")] private Vector2 bodyVel;
    [SyncVar(hook = "AngleChange")] private Vector2 bodyAngle; // Where x is current angle and y is angular velocity.

    private float timer = 0f;

    private const float TRANSFORM_LERP_SPEED = 20f;

    public override float GetNetworkSendInterval()
    {
        // Send updates whenever the variables are updated.
        return 0f;
    }

    public void Update()
    {
        timer += Time.unscaledDeltaTime;

        UpdateSending();
        UpdateClient();
    }

    private void UpdateClient()
    {
        if (isServer || !isClient)
            return;

        // Only if we don't have a body: body sync is handled in syncvar hooks.
        if (!HasBody)
        {
            // Interpolate (no extrapolation becauase updates are not necessarily sent periodically)

            // Lerp position...
            transform.localPosition = Vector2.Lerp(transform.localPosition, bodyPos, Time.deltaTime * TRANSFORM_LERP_SPEED);

            // Lerp angle.
            var rot = transform.localEulerAngles;
            rot.z = Mathf.LerpAngle(rot.z, bodyAngle.x, Time.deltaTime * TRANSFORM_LERP_SPEED);
            transform.localEulerAngles = rot;
        }
    }

    private void UpdateSending()
    {
        if (!isServer)
            return;

        if (timer >= UpdateInterval)
        {
            timer = 0f;

            // Send updates if it has a rigidbody.
            if (HasBody)
            {
                Vector2 angle = new Vector2(Rigidbody.rotation, Rigidbody.angularVelocity);

                if (bodyPos != Rigidbody.position)
                    bodyPos = Rigidbody.position;
                if (bodyVel != Rigidbody.velocity)
                    bodyVel = Rigidbody.velocity;
                if (bodyAngle != angle)
                    bodyAngle = angle;
            }
            else
            {
                // Send transform info. It will then have to interpolated on the other end.
                if (bodyPos != (Vector2)transform.localPosition)
                    bodyPos = (Vector2)transform.localPosition;
                Vector2 angle = new Vector2(transform.localEulerAngles.z, 0f);
                if (bodyAngle != angle)
                    bodyAngle = angle;
            }
        }
    }

    private void PosChange(Vector2 newPos)
    {
        bodyPos = newPos;

        if (isServer)
            return;

        if (HasBody)
            Rigidbody.MovePosition(newPos);
    }

    private void VelChange(Vector2 newVel)
    {
        bodyVel = newVel;

        if (isServer)
            return;

        if (HasBody)
            Rigidbody.velocity = newVel;
    }

    private void AngleChange(Vector2 angle)
    {
        bodyAngle = angle;

        if (isServer)
            return;

        if (HasBody)
        {
            Rigidbody.rotation = angle.x;
            Rigidbody.angularVelocity = angle.y;
        }
    }
}
