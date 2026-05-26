using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactRadius = 1.5f;
    [SerializeField] private LayerMask interactableLayer;
    
    private Collider2D[] _interactables;
    private List<IInteractable> _nearbyInteractables = new List<IInteractable>();
    private IInteractable _currentClosest;
    private Coroutine _hoverRoutine;
    
    private void OnEnable()
    {
        InputController.OnInteract += TryInteract;
    }

    private void OnDisable()
    {
        InputController.OnInteract -= TryInteract;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var interactable = collision.GetComponent<IInteractable>() ?? collision.GetComponentInParent<IInteractable>();
        
        if (interactable != null && !_nearbyInteractables.Contains(interactable))
        {
            _nearbyInteractables.Add(interactable);
            
            if (_hoverRoutine == null)
            {
                _hoverRoutine = StartCoroutine(HoverCheckRoutine());
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        var interactable = collision.GetComponent<IInteractable>() ?? collision.GetComponentInParent<IInteractable>();
        
        if (interactable != null && _nearbyInteractables.Contains(interactable))
        {
            _nearbyInteractables.Remove(interactable);
            
            if (_currentClosest == interactable)
            {
                _currentClosest.HideHint();
                _currentClosest = null;
            }
            
            if (_nearbyInteractables.Count == 0 && _hoverRoutine != null)
            {
                StopCoroutine(_hoverRoutine);
                _hoverRoutine = null;
            }
        }
    }
    
    private IEnumerator HoverCheckRoutine()
    {
        var wait = new WaitForSeconds(0.1f); 

        while (true)
        {
            _nearbyInteractables.RemoveAll(i => i == null || ((MonoBehaviour)i).gameObject == null);

            if (_nearbyInteractables.Count == 0) yield break;

            IInteractable closestInteractable = null;
            float closestDistance = float.MaxValue;
            
            foreach (var interactable in _nearbyInteractables)
            {
                var mono = interactable as MonoBehaviour;
                if (mono != null)
                {
                    float dist = Vector2.Distance(transform.position, mono.transform.position);
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        closestInteractable = interactable;
                    }
                }
            }
            if (_currentClosest != closestInteractable)
            {
                _currentClosest?.HideHint();
                _currentClosest = closestInteractable;
                _currentClosest?.DisplayHint();
            }
            
            yield return wait;
        }
    }
    
    private void TryInteract()
    {
        if (DialogBus.IsOpen) return;
        if (Time.frameCount == DialogBus.LastClosedFrame) return;
        _currentClosest?.Interact(gameObject);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
