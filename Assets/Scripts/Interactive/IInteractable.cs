using UnityEngine;

public interface IInteractable
{
    void Interact(GameObject interactor);

    void DisplayHint();
    void HideHint();
}
