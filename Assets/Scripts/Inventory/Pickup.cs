using UnityEngine;

public class Pickup : MonoBehaviour, IInteractable
{
    [SerializeField] private Item item;

    public Item Item => item;

    public void Interact(GameObject interactor)
    {
        if (Inventory.instance == null) return;
        Inventory.instance.TryPickup(this);
    }

    public void OnPickedUp()
    {
        gameObject.SetActive(false);
    }

    public void Drop(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
    }
}