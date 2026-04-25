using UnityEngine;
using FMODUnity;

public class PowerOutlet : MonoBehaviour, IInteractable
{
    [SerializeField] private float cordLength = 5f;
    [SerializeField] private bool IsPowered = true;

    private StudioEventEmitter emitter;

    public float CordLength => cordLength;

    private void Start()
    {
        emitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.socketIdle, this.gameObject);
        emitter.Play();
    }

    public void Interact(GameObject interactor)
    {
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
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, cordLength);
    }
}