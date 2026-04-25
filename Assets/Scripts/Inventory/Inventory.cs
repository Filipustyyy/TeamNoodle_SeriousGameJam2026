using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    private Pickup current;

    public Item Current => current ? current.Item : null;

    void Awake()
    {
        instance = this;
    }

    public void TryPickup(Pickup pickup)
    {
        if (pickup == null) return;

        if (current != null)
        {
            current.Drop(pickup.transform.position);
        }

        current = pickup;
        pickup.OnPickedUp();
        InventoryBus.RaiseChanged(current.Item);
    }
}