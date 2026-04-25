using UnityEngine;

public class PowerOutlet : MonoBehaviour, IInteractable
{
    [SerializeField] private float cordLength = 5f;

    public float CordLength => cordLength;

    public void Interact(GameObject interactor)
    {
        var tether = interactor.GetComponent<PlayerTether>();
        if (tether == null) return;
        tether.Plug(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, cordLength);
    }
}