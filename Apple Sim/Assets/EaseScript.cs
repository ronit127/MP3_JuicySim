using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class EaseScript : MonoBehaviour
{
    public AppleCalc manager;
    public float easeSpeed = 8f;
    public bool enableHaptics = true;
    public float hapticAmplitude = 0.2f;
    public float hapticDuration = 0.05f;
    public AudioSource easeAudioSource;
    public float easeSoundVolume = 0.5f;

    private float currentProgress = 0f;
    private float hapticCooldown = 0f;
    private float soundCooldown = 0f;

    void Start()
    {
        // Create instance material so we don't affect shared material
        GetComponent<Renderer>().material = new Material(GetComponent<Renderer>().material);
    }

    void Update()
    {
        if (manager == null) return;

        // Read generator count from manager
        float goal = manager.maxGenerators > 0 
            ? (float)manager.generatorCount / manager.maxGenerators 
            : 0f;
        goal = Mathf.Clamp01(goal);

        // Ease toward goal
        float delta = goal - currentProgress;
        currentProgress += easeSpeed * delta * Time.deltaTime;
        currentProgress = Mathf.Clamp01(currentProgress);

        // Haptic tick when progress changes enough
        if (enableHaptics && hapticCooldown <= 0f && Mathf.Abs(delta) > 0.005f)
        {
            HapticPulse(hapticAmplitude, hapticDuration);
            hapticCooldown = hapticDuration * 1.5f;
        }

        if (hapticCooldown > 0f)
            hapticCooldown -= Time.deltaTime;

        // Sound tick when progress changes enough
        if (easeAudioSource != null && soundCooldown <= 0f && Mathf.Abs(delta) > 0.005f)
        {
            easeAudioSource.volume = easeSoundVolume;
            easeAudioSource.Play();
            soundCooldown = 0.1f; // short cooldown for sound
        }

        if (soundCooldown > 0f)
            soundCooldown -= Time.deltaTime;

        // Apply color and scale
        Color col = Color.Lerp(Color.red, Color.green, currentProgress);
        GetComponent<Renderer>().material.color = col;

        float scale = 1f + 0.5f * currentProgress;
        transform.localScale = Vector3.one * scale;
    }

    void HapticPulse(float amplitude, float duration)
    {
        if (!enableHaptics) return;

        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller,
            devices);

        foreach (var device in devices)
        {
            if (device.TryGetHapticCapabilities(out HapticCapabilities caps) && caps.supportsImpulse)
            {
                device.SendHapticImpulse(0, Mathf.Clamp01(amplitude), Mathf.Max(0f, duration));
            }
        }
    }
}
