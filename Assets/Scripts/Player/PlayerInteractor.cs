using UnityEngine;
using UnityEngine.InputSystem;

public class 
    PlayerInteractor : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactRadius = 1.5f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private InputActionReference interactAction;

    private void OnEnable()
    {
        if (interactAction == null) return;
        interactAction.action.performed += OnInteract;
        interactAction.action.Enable();
    }

    private void OnDisable()
    {
        if (interactAction == null) return;
        interactAction.action.performed -= OnInteract;
    }

    private void OnInteract(InputAction.CallbackContext _)
    {
        var hit = Physics2D.OverlapCircle(transform.position, interactRadius, interactableLayer);
        if (hit == null) return;

        var interactable = hit.GetComponent<IInteractable>() ?? hit.GetComponentInParent<IInteractable>();
        if (interactable == null) return;

        interactable.Interact(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}