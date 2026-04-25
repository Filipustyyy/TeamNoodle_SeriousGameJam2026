using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactRadius = 1.5f;
    [SerializeField] private LayerMask interactableLayer;
    
    private void OnEnable()
    {
        InputController.OnInteract += TryInteract;
    }

    private void OnDisable()
    {
        InputController.OnInteract -= TryInteract;
    }

    private void TryInteract()
    {
        var hit = Physics2D.OverlapCircle(transform.position, interactRadius, interactableLayer);
        if (hit != null) return;
        
        
    }
}
