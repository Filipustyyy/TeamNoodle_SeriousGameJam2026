using UnityEngine;

public class PowerOutlet : MonoBehaviour, IInteractable
{
    [SerializeField] private float cordLength = 5f;
    [SerializeField] private bool IsPowered = true;

    public float CordLength => cordLength;

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