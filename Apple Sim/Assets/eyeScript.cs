using UnityEngine;

public class EyeScript : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public Transform targetController;

    [Header("Eye Settings")]
    public float rotationSpeed = 5f;
    public float maxRotationAngle = 40f;
    public bool useSmoothRotation = true;

    [Header("Axis Settings")]
    public Vector3 eyeForwardAxis = Vector3.forward;
    public Vector3 eyeUpAxis = Vector3.up;

    [Header("Blink Settings")]
    public float blinkDuration = 0.1f;
    public float blinkIntervalMin = 2f;
    public float blinkIntervalMax = 6f;

    private Quaternion initialLocalRotation;
    private Renderer[] eyeRenderers;
    private float nextBlinkTime;
    private bool isBlinking;
    private float blinkTimer;

    void Start()
    {
        initialLocalRotation = transform.localRotation;
        eyeRenderers = GetComponentsInChildren<Renderer>();
        ScheduleNextBlink();
    }

    void Update()
    {
        HandleBlink();

        if (targetController == null) return;

        Vector3 worldDir = (targetController.position - transform.position).normalized;
        Vector3 localDir = transform.parent.InverseTransformDirection(worldDir);
        Vector3 localForward = initialLocalRotation * eyeForwardAxis;

        Quaternion swingRotation = Quaternion.FromToRotation(localForward, localDir);
        swingRotation.ToAngleAxis(out float angle, out Vector3 axis);
        angle = Mathf.Clamp(angle, -maxRotationAngle, maxRotationAngle);
        Quaternion clampedSwing = Quaternion.AngleAxis(angle, axis);
        Quaternion targetRotation = clampedSwing * initialLocalRotation;

        if (useSmoothRotation)
        {
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            transform.localRotation = targetRotation;
        }
    }

    void HandleBlink()
    {
        if (isBlinking)
        {
            blinkTimer += Time.deltaTime;
            if (blinkTimer >= blinkDuration)
            {
                foreach (var r in eyeRenderers) r.enabled = true;
                isBlinking = false;
                ScheduleNextBlink();
            }
            return;
        }

        if (Time.time >= nextBlinkTime)
        {
            foreach (var r in eyeRenderers) r.enabled = false;
            isBlinking = true;
            blinkTimer = 0f;
        }
    }

    void ScheduleNextBlink()
    {
        nextBlinkTime = Time.time + Random.Range(blinkIntervalMin, blinkIntervalMax);
    }

    public void ResetRotation()
    {
        transform.localRotation = initialLocalRotation;
    }
}