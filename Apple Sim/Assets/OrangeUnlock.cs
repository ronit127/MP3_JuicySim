using UnityEngine;
using TMPro;

public class OrangeUnlock : MonoBehaviour
{
    public AppleCalc manager;

    public GameObject tutorialPanel;       // UI panel with tutorial text
    public TextMeshProUGUI buttonText;

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

    void ClearText()
    {
        tutorialPanel.SetActive(false);
    }
}