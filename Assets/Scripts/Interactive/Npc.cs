using UnityEngine;
using UnityEngine.Events;

public class Npc : MonoBehaviour, IInteractable
{
    [SerializeField] private string displayName = "NPC";
    [SerializeField] private UnityEvent<GameObject> onInteract;
    [SerializeField] private GameObject hintSpriteObject;

    public string DisplayName => displayName;

    void Start()
    {
        HideHint();
    }
    
    public void Interact(GameObject interactor) {
        onInteract?.Invoke(interactor);
        HideHint();
    }
    
    public void DisplayHint()
    {
        hintSpriteObject.SetActive(true);
    }

    public void HideHint()
    {
        hintSpriteObject.SetActive(false);
    }
}