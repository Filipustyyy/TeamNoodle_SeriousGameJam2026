using UnityEngine;

public class PowerOutlet : MonoBehaviour
{
    [SerializeField] private float cordLength = 5f;
    [SerializeField] private bool isPowered = true;

    public float CordLength => cordLength;
    public bool IsPowered => isPowered;

    public void TogglePower()
    {
        isPowered = !isPowered;
        
        Debug.Log($"{gameObject.name} power state is now: {isPowered}");
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, cordLength);
    }
}
