using UnityEngine;

public class ScaleEaser : MonoBehaviour
{
    [Header("Easing Settings")]
    public float easeSpeed = 8f;
    public float maxScaleMultiplier = 1.3f; // How much bigger it gets (1.3 = 30% bigger)
    public float triggerDuration = 0.5f; // How long the effect lasts

    [Header("References")]
    public Transform targetTransform; // The transform to scale (leave empty to use this transform)

    private Vector3 originalScale;
    private Vector3 targetScale;
    private float triggerTimer = 0f;
    private bool isTriggered = false;

    void Start()
    {
        if (targetTransform == null)
        {
            targetTransform = transform;
        }

        originalScale = targetTransform.localScale;
        targetScale = originalScale;
    }

    void Update()
    {
        if (isTriggered)
        {
            triggerTimer -= Time.deltaTime;
            if (triggerTimer <= 0f)
            {
                // Effect is over, ease back to original
                targetScale = originalScale;
                isTriggered = false;
            }
        }

        // Use easing formula: v = k * (goal - x) * Time.deltaTime
        Vector3 currentScale = targetTransform.localScale;
        Vector3 scaleDifference = targetScale - currentScale;
        Vector3 velocity = easeSpeed * scaleDifference * Time.deltaTime;

        targetTransform.localScale = currentScale + velocity;
    }

    // Call this method to trigger the scale effect
    public void TriggerScaleEffect()
    {
        if (!isTriggered)
        {
            targetScale = originalScale * maxScaleMultiplier;
            triggerTimer = triggerDuration;
            isTriggered = true;
        }
    }

    // Optional: Trigger with custom duration
    public void TriggerScaleEffect(float duration)
    {
        triggerDuration = duration;
        TriggerScaleEffect();
    }
}