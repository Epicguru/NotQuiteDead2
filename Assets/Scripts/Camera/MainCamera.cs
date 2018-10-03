using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MainCamera : MonoBehaviour
{
    public static MainCamera Instance;
    public static Camera Cam
    {
        get
        {
            return Instance == null ? null : Instance.Camera;
        }
    }

    public Camera Camera
    {
        get
        {
            if (_cam == null)
                _cam = GetComponent<Camera>();
            return _cam;
        }
    }
    private Camera _cam;

    [Header("Targeting")]
    public static Transform Target;
    public float MaxFollowSpeed = 100f;
    public float MaxSpeedDistance = 10f;
    public AnimationCurve Curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Size")]
    public bool AllowSizeChange = true;
    public float TargetSize = 10f;
    public float MinSize = 2f;
    public float MaxSize = 30f;
    public float MaxSizeChange = 20f;
    public AnimationCurve SizeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    public void Awake()
    {
        Instance = this;
        this.tag = "MainCamera";
    }

    public void LateUpdate()
    {
        if (Target == null)
            return;

        TakeInput();

        // Move to target...
        Vector2 displacement = Target.position - transform.position;
        float distance = displacement.magnitude;
        displacement.Normalize();

        float speed = Curve.Evaluate(Mathf.Clamp01(distance / MaxSpeedDistance)) * MaxFollowSpeed;

        transform.position += (Vector3)displacement * speed * Time.deltaTime;

        // Zoom to target zoom.
        if (AllowSizeChange)
        {
            TargetSize = Mathf.Clamp(TargetSize, MinSize, MaxSize);
            float current = Camera.orthographicSize;
            float diff = TargetSize - current;
            float s = SizeCurve.Evaluate(Mathf.Abs(Mathf.Clamp(diff / 10f, -1f, 1f))) * MaxSizeChange;

            current += (diff > 0f ? 1f : diff < 0f ? -1f : 0) * s * Time.deltaTime;
            Camera.orthographicSize = current;
        }
    }

    private void TakeInput()
    {
        float scroll = Input.mouseScrollDelta.y;

        if(scroll > 0f)
        {
            TargetSize /= 1.1f;
        }
        else if(scroll < 0f)
        {
            TargetSize *= 1.1f;
        }
    }
}
