using UnityEngine;

public class FallingFruit : MonoBehaviour
{
    public float fallSpeed = 2.5f;
    public float lifetime = 4f;
    public float spinSpeed = 90f;

    private bool hasLanded = false;
    private float elapsed = 0f;
    private Vector3 drift;
    private Rigidbody rb;

    void Start()
    {
        drift = new Vector3(Random.Range(-0.3f, 0.3f), 0f, Random.Range(-0.3f, 0.3f));
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Ensure physics will move the fruit; prefer physics when available
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.down * fallSpeed + drift;
            rb.angularVelocity = Random.insideUnitSphere * (spinSpeed * Mathf.Deg2Rad);
        }
    }

    void Update()
    {
        if (!hasLanded) return;

        elapsed += Time.deltaTime;
        // if (rb == null)
        // {
        //     transform.position += (Vector3.down * fallSpeed + drift) * Time.deltaTime;
        //     transform.Rotate(Random.insideUnitSphere * spinSpeed * Time.deltaTime);
        // }
        // else
        // {
        //     // keep velocity stable in case other forces apply
        //     rb.linearVelocity = Vector3.down * fallSpeed + drift;
        // }

        if (elapsed >= lifetime)
            Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        hasLanded = true;
    }
}
