using UnityEngine;

public class GateContact : MonoBehaviour
{
    public OrangeUnlock orangeUnlock;

    void OnTriggerEnter(Collider other)
    {
        if (orangeUnlock != null && orangeUnlock.unlocked)
            gameObject.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (orangeUnlock != null && orangeUnlock.unlocked)
            gameObject.SetActive(false);
    }
}
