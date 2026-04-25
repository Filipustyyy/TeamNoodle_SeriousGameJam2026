using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerTether : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactRadius = 1.5f;
    [SerializeField] private LayerMask outletLayer;
    [SerializeField] private InputActionReference interactAction;

    [Header("References")]
    [SerializeField] private LineRenderer cord;
    
    [Header("Sound event references")] 
    [SerializeField] private EventReference attachTether;

    private Rigidbody2D rb;
    private PowerOutlet currentOutlet;

    private void Reset()
    {
        cord = GetComponent<LineRenderer>();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (cord != null)
        {
            cord.positionCount = 2;
            cord.enabled = false;
        }

        var nearest = FindNearestOutlet();
        if (nearest != null) Plug(nearest);
    }

    private PowerOutlet FindNearestOutlet()
    {
        var outlets = Object.FindObjectsByType<PowerOutlet>(FindObjectsInactive.Exclude);
        PowerOutlet best = null;
        float bestSqr = float.PositiveInfinity;
        Vector2 me = transform.position;
        foreach (var o in outlets)
        {
            float sqr = ((Vector2)o.transform.position - me).sqrMagnitude;
            if (sqr < bestSqr) { bestSqr = sqr; best = o; }
        }
        return best;
    }

    private void Plug(PowerOutlet outlet)
    {
        AudioManager.instance.PlayOneShot(attachTether, this.transform.position);
        currentOutlet = outlet;
        if (cord != null) cord.enabled = true;
    }

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
        TryPlug();
    }

    private void TryPlug()
    {
        var hit = Physics2D.OverlapCircle(transform.position, interactRadius, outletLayer);
        if (hit == null) return;

        var outlet = hit.GetComponent<PowerOutlet>() ?? hit.GetComponentInParent<PowerOutlet>();
        if (outlet == null) return;
        if (outlet == currentOutlet) return;

        Plug(outlet);
    }

    private void FixedUpdate()
    {
        if (currentOutlet == null) return;

        Vector2 anchor = currentOutlet.transform.position;
        Vector2 pos = rb.position;
        Vector2 delta = pos - anchor;
        float maxDist = currentOutlet.CordLength;

        if (delta.sqrMagnitude <= maxDist * maxDist) return;

        Vector2 clamped = anchor + delta.normalized * maxDist;
        rb.position = clamped;

        Vector2 outward = delta.normalized;
        float radialSpeed = Vector2.Dot(rb.linearVelocity, outward);
        if (radialSpeed > 0f)
            rb.linearVelocity -= outward * radialSpeed;
    }

    private void LateUpdate()
    {
        if (currentOutlet == null || cord == null) return;
        cord.SetPosition(0, currentOutlet.transform.position);
        cord.SetPosition(1, transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
