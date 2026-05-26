using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Pickup : MonoBehaviour
{
    [SerializeField] private CableExtensionConfig config;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (config == null) return;

        var tether = other.GetComponentInParent<PlayerTether>();
        if (tether == null) return;

        tether.AddCordLength(config.LengthBonus);
        AudioManager.instance.PlayOneShot(FMODEvents.instance.cableCollected, transform.position);
        Destroy(gameObject);
    }
}
