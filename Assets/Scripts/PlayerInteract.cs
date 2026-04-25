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
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRadius, interactableLayer);
        
        if (hits.Length == 0) return;

        IInteractable closestInteractable = null;
        float closestDistance = float.MaxValue;
        
        foreach (var hit in hits)
        {
            var interactable = hit.GetComponent<IInteractable>() ?? hit.GetComponentInParent<IInteractable>();
            
            if (interactable != null)
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractable = interactable;
                }
            }
        }
        
        closestInteractable?.Interact(gameObject);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
