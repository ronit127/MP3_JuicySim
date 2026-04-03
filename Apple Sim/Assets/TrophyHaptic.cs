using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class TrophyHaptic : MonoBehaviour
{
    [Header("Haptic Settings")]
    public float hapticAmplitude = 0.8f;
    public float hapticDuration = 0.3f;
    public bool enableHaptics = true;

    void OnEnable()
    {
        if (enableHaptics)
        {
            PlayHapticFeedback();
        }
    }

    void PlayHapticFeedback()
    {
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller,
            devices);

        foreach (var device in devices)
        {
            if (device.TryGetHapticCapabilities(out HapticCapabilities caps) && caps.supportsImpulse)
            {
                device.SendHapticImpulse(0, Mathf.Clamp01(hapticAmplitude), Mathf.Max(0f, hapticDuration));
            }
        }
    }
}