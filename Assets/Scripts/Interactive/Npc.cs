using UnityEngine;
using UnityEngine.Events;

public class Npc : MonoBehaviour, IInteractable
{
    [SerializeField] private string displayName = "NPC";
    [SerializeField] private UnityEvent<GameObject> onInteract;

    public string DisplayName => displayName;

    public void Interact(GameObject interactor) => onInteract?.Invoke(interactor);
}