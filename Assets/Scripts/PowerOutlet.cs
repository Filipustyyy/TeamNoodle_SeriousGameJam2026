using UnityEngine;
using FMODUnity;

public class PowerOutlet : MonoBehaviour, IInteractable
{
    [SerializeField] private bool IsPowered = true;

    private StudioEventEmitter emitter;

    private void Start()
    {
        emitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.socketIdle, this.gameObject);
        emitter.Play();
    }

    public void Interact(GameObject interactor)
    {
        if (!IsPowered) return;
        var tether = interactor.GetComponent<PlayerTether>();
        if (tether == null) return;
        tether.Plug(this);
    }

    public void PowerOn()
    {
        IsPowered = true;
    }

    private void OnDrawGizmosSelected()
    {
        var tether = FindFirstObjectByType<PlayerTether>(FindObjectsInactive.Include);
        if (tether == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, tether.MaxCordLength);
    }
}