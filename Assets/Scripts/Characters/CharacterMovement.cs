using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour {

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

    public Vector2 NormalizedInputDirection;
    public float CurrentSpeed = 10f;

    public void Update()
    {
        // Speed cannot be less than zero...
        CurrentSpeed = Mathf.Max(0f, CurrentSpeed);

        Body.velocity = NormalizedInputDirection.normalized * CurrentSpeed;

        // Change character direction based on weather we are moving left or right, and weather there is a item being held.
        // TODO
    }
}
