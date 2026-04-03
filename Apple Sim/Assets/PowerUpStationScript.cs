using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class PowerUpStationScript : MonoBehaviour
{
    public AppleCalc manager;
    public TextMeshProUGUI productionRateText;
    public TextMeshProUGUI buttonText;
    public Button button;

    public float price = 100f;
    public float productionMultiplier = 1.2f;
    public int maxPurchases = 5;
    private int purchases = 0;
    private float cumulativeMultiplier = 1f;

    public AudioSource powerUpAudioSource;
    public ParticleSystem powerUpParticles;

    [Header("Eye Shock Effect")]
    public Transform eyeLeft;
    public Transform eyeRight;
    public float eyeShockScale = 1.5f;
    public float eyeShockSpeed = 8f;
    public float eyeShockHoldTime = 0.3f;

    private AppleTreeStation[] cachedAppleTrees;
    private OrangeTreeStationScript[] cachedOrangeTrees;

    private float lastClickTime = -999f;
    private float clickCooldown = 0.1f;

    public void Start()
    {
        buttonText.text = "Boost: " + (int)price + " apples";
        productionRateText.text = "Boost x1.0";
        cachedAppleTrees = FindObjectsByType<AppleTreeStation>(FindObjectsSortMode.None);
        cachedOrangeTrees = FindObjectsByType<OrangeTreeStationScript>(FindObjectsSortMode.None);

        CheckButtonStatus();
    }

    public void Update()
    {
        CheckButtonStatus();
    }

    public void OnStationClicked()
    {
        if (Time.time - lastClickTime < clickCooldown) return;
        lastClickTime = Time.time;

        if (purchases >= maxPurchases) return;

        if (manager.apples >= price)
        {
            manager.apples -= price;

            manager.generationRate *= productionMultiplier;
            manager.orangeGenerationRate *= productionMultiplier;
            cumulativeMultiplier *= productionMultiplier;

            foreach (AppleTreeStation t in cachedAppleTrees)
                t.ApplyPowerUp(productionMultiplier);

            foreach (OrangeTreeStationScript t in cachedOrangeTrees)
                t.ApplyPowerUp(productionMultiplier);

            productionRateText.text = "Boost x" + cumulativeMultiplier.ToString("F2");

            productionMultiplier += 0.1f;
            price = Mathf.Round(price * 2f);
            purchases++;

            buttonText.text = purchases >= maxPurchases ? "Sold Out!" : "Boost: " + (int)price + " apples";

            if (powerUpParticles != null) powerUpParticles.Play();
            if (powerUpAudioSource != null) powerUpAudioSource.Play();

            PlayHaptic();
            TriggerEyeShock();
        }

        CheckButtonStatus();
    }

    void TriggerEyeShock()
    {
        if (eyeLeft != null) StartCoroutine(ShockEye(eyeLeft));
        if (eyeRight != null) StartCoroutine(ShockEye(eyeRight));
    }

    IEnumerator ShockEye(Transform eye)
    {
        Vector3 originalScale = eye.localScale;
        Vector3 targetScale = originalScale * eyeShockScale;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * eyeShockSpeed;
            eye.localScale = Vector3.Lerp(originalScale, targetScale, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        yield return new WaitForSeconds(eyeShockHoldTime);

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * eyeShockSpeed;
            eye.localScale = Vector3.Lerp(targetScale, originalScale, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        eye.localScale = originalScale;
    }

    void PlayHaptic()
    {
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller,
            devices);

        foreach (var device in devices)
        {
            if (device.TryGetHapticCapabilities(out HapticCapabilities caps) && caps.supportsImpulse)
                device.SendHapticImpulse(0, 0.6f, 0.15f);
        }
    }

    void CheckButtonStatus()
    {
        if (button != null)
            button.interactable = purchases < maxPurchases && manager.apples >= price;
    }
}