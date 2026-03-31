using UnityEngine;
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
        }

        CheckButtonStatus();
    }
    
    void CheckButtonStatus()
    {
        if (button != null)
        {
            button.interactable = purchases < maxPurchases && manager.apples >= price;   
        }
    }
}
