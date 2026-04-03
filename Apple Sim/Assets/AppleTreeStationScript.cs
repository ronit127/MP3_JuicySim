using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class AppleTreeStation : MonoBehaviour
{
    public AppleCalc manager;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI currentProductionText;
    public GameObject clickerButton;
    public Button stationButton;
    public MeshFilter treeVisual;
    public Mesh[] treeLevels;

    public float price = 10f;
    public float productionPowerChange = 1f;
    private float currProductionPower = 0f;
    private bool isOwned = false;
    private int upgradeCount = 0;
    public int clickerRate = 1;

    private static int globalAppleTreeCount = 0;
    private float FirstBuyPrice() => Mathf.Round(price * Mathf.Pow(1.5f, globalAppleTreeCount));

    private float lastClickTime = -999f;
    private float clickCooldown = 0.05f;

    public GameObject fruitPrefab;
    public Transform fruitSpawnPoint;
    private float fruitSpawnTimer = 0f;
    private const float MinSpawnInterval = 0.5f;

    public ParticleSystem particles;

    public AudioSource clickerAudioSource;
    public float clickerSoundVolume = 1f;

    public AudioSource[] generatorAudioSources;
    public float[] generatorSoundVolumes;

    [Header("Generator Haptics")]
    public bool enableGeneratorHaptics = true;
    public float baseHapticAmplitude = 0.3f;
    public float baseHapticDuration = 0.2f;
    public float hapticLevelMultiplier = 0.2f;

    [Header("Visual Effects")]
    public ScaleEaser signScaleEaser;

    public void Start()
    {
        treeVisual.gameObject.SetActive(false);
        clickerButton.SetActive(false);
        statusText.text = "Buy: " + (int)FirstBuyPrice() + " apples";
        currentProductionText.text = "";
        CheckButtonStatus();
    }

    void UpdateParticleRate()
    {
        if (particles == null) return;
        var emission = particles.emission;
        if (!isOwned)
        {
            emission.rateOverTime = 0f;
            return;
        }
        if (upgradeCount == 1)
            emission.rateOverTime = 5f;
        else if (upgradeCount == 2)
            emission.rateOverTime = 50f;
        else if (upgradeCount >= 3)
            emission.rateOverTime = 100f;
        else
            emission.rateOverTime = 0f;
    }

    void Update()
    {
        CheckButtonStatus();
        UpdateParticleRate();

        if (!isOwned || currProductionPower <= 0f || fruitPrefab == null) return;

        fruitSpawnTimer += Time.deltaTime;
        float spawnInterval = Mathf.Max(MinSpawnInterval, 3f / currProductionPower);

        while (fruitSpawnTimer >= spawnInterval)
        {
            fruitSpawnTimer -= spawnInterval;
            SpawnFruit();
        } 
    }

    void SpawnFruit()
    {
        Vector3 origin = fruitSpawnPoint != null ? fruitSpawnPoint.position : transform.position + Vector3.up * 2f;
        Vector3 offset = new Vector3(Random.Range(-0.4f, 0.4f), 0f, Random.Range(-0.4f, 0.4f));
        Instantiate(fruitPrefab, origin + offset, Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));
    }

    public void OnStationClicked()
    {
        if (Time.time - lastClickTime < clickCooldown) return;
        lastClickTime = Time.time;

        float cost = isOwned ? price : FirstBuyPrice();
        if (manager.apples >= cost && upgradeCount < treeLevels.Length)
        {
            manager.apples -= cost;

            if (!isOwned)
            {
                isOwned = true;
                globalAppleTreeCount++;
                treeVisual.gameObject.SetActive(true);
                clickerButton.SetActive(true);

                if (treeLevels != null && treeLevels.Length > 0)
                    treeVisual.mesh = treeLevels[0];

                manager.generationRate += productionPowerChange;
                manager.generatorCount++;
                currProductionPower += productionPowerChange;
                upgradeCount++;
                price = Mathf.Round(cost * 2f);

                PlayGeneratorSound(upgradeCount);
                PlayGeneratorHaptics(upgradeCount);

                // Trigger visual scale effect
                if (signScaleEaser != null)
                {
                    signScaleEaser.TriggerScaleEffect();
                }
            }
            else
            {
                if (upgradeCount < treeLevels.Length)
                {
                    treeVisual.mesh = treeLevels[upgradeCount];
                    manager.generationRate += productionPowerChange;
                    currProductionPower += productionPowerChange;
                    upgradeCount++;
                    price = Mathf.Round(price * 1.5f);

                    PlayGeneratorSound(upgradeCount);
                    PlayGeneratorHaptics(upgradeCount);

                    // Trigger visual scale effect
                    if (signScaleEaser != null)
                    {
                        signScaleEaser.TriggerScaleEffect();
                    }
                }
            }

            currentProductionText.text = currProductionPower.ToString("F1") + " apples/s";

            if (upgradeCount >= treeLevels.Length)
                statusText.text = "Max Level!";
            else
                statusText.text = "Upgrade: " + (int)price + " apples";
        }

        CheckButtonStatus();
    }

    public void OnClickerClicked()
    {
        if (!isOwned) return;
        if (Time.time - lastClickTime < clickCooldown) return;
        lastClickTime = Time.time;
        manager.apples += clickerRate;

        // Play sound on manual click
        if (clickerAudioSource != null)
        {
            clickerAudioSource.volume = clickerSoundVolume;
            clickerAudioSource.Play();
        }
    }

    void PlayGeneratorSound(int generatorLevel)
    {
        if (generatorAudioSources == null || generatorAudioSources.Length == 0) return;
        if (generatorSoundVolumes == null || generatorSoundVolumes.Length == 0) return;

        // Map generator levels to sound indices: level 1 = index 0, level 2 = index 1, level 3 = index 2
        int soundIndex = Mathf.Min(generatorLevel - 1, generatorAudioSources.Length - 1);

        if (soundIndex >= 0 && soundIndex < generatorAudioSources.Length && generatorAudioSources[soundIndex] != null)
        {
            generatorAudioSources[soundIndex].volume = (soundIndex < generatorSoundVolumes.Length) ? generatorSoundVolumes[soundIndex] : 1f;
            generatorAudioSources[soundIndex].Play();
        }
    }

    void PlayGeneratorHaptics(int generatorLevel)
    {
        if (!enableGeneratorHaptics) return;

        // Increase haptic intensity with generator level
        float amplitude = Mathf.Min(baseHapticAmplitude + (generatorLevel - 1) * hapticLevelMultiplier, 1f);
        float duration = baseHapticDuration + (generatorLevel - 1) * 0.1f;

        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller,
            devices);

        foreach (var device in devices)
        {
            if (device.TryGetHapticCapabilities(out HapticCapabilities caps) && caps.supportsImpulse)
            {
                device.SendHapticImpulse(0, amplitude, duration);
            }
        }
    }

    public void ApplyPowerUp(float multiplier)
    {
        if (!isOwned) return;
        currProductionPower *= multiplier;
        currentProductionText.text = currProductionPower.ToString("F1") + " apples/s";
    }

    void CheckButtonStatus()
    {
        if (stationButton != null)
        {
            float cost = isOwned ? price : FirstBuyPrice();
            stationButton.interactable = manager.apples >= cost && upgradeCount < treeLevels.Length;
            if (!isOwned && upgradeCount < treeLevels.Length)
                statusText.text = "Buy: " + (int)cost + " apples";
        }
    }
}
