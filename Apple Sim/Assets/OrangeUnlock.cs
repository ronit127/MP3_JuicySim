using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using TMPro;

public class OrangeUnlock : MonoBehaviour
{
    public AppleCalc manager;

    public GameObject tutorialPanel;       // UI panel with tutorial text
    public TextMeshProUGUI buttonText;

    [Header("Haptics")]
    public bool enableHaptics = true;
    public float hapticAmplitude = 0.6f;
    public float hapticDuration = 0.15f;

    public Renderer gateRenderer;
    public Color unlockedColor = Color.green;

    public AudioSource pickupAudioSource;
    public float pickupSoundVolume = 1f;
    public ParticleSystem unlockParticles;

    public bool unlocked = false;

    public void OnUnlockClicked()
    {
        if (unlocked) return;

        if (manager.apples >= manager.unlockCost)
        {
            manager.apples -= manager.unlockCost;
            manager.unlockClicked = true;

            tutorialPanel.SetActive(true);  // show tutorial
            Invoke(nameof(ClearText), 45f);

            unlocked = true;

            if (pickupAudioSource != null)
            {
                pickupAudioSource.volume = pickupSoundVolume;
                pickupAudioSource.Play();
            }

            if (unlockParticles != null)
                unlockParticles.Play();

            if (gateRenderer != null)
                gateRenderer.material.color = unlockedColor;

            if (enableHaptics)
                PlayHaptics();

            if (buttonText != null)
                buttonText.text = "Unlocked";
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (unlocked && gateRenderer != null)
            gateRenderer.gameObject.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (unlocked && gateRenderer != null)
            gateRenderer.gameObject.SetActive(false);
    }

    void PlayHaptics()
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

    void ClearText()
    {
        tutorialPanel.SetActive(false);
    }
}