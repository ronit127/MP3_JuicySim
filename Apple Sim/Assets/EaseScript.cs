using UnityEngine;

public class EaseScript : MonoBehaviour
{
    public AppleCalc manager;
    public float easeSpeed = 8f;

    private float currentProgress = 0f;

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

        // Apply color and scale
        Color col = Color.Lerp(Color.red, Color.green, currentProgress);
        GetComponent<Renderer>().material.color = col;

        float scale = 1f + 0.5f * currentProgress;
        transform.localScale = Vector3.one * scale;
    }
}
