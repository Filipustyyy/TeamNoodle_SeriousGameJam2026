using UnityEngine;

public class PowerOutlet : MonoBehaviour, IInteractable
{
    [SerializeField] private bool IsPowered = true;

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
        var tether = FindFirstObjectByType<PlayerTether>(FindObjectsInactive.Include);
        if (tether == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, tether.MaxCordLength);
    }
}