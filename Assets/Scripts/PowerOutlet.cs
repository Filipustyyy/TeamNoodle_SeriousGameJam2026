using UnityEngine;

public class PowerOutlet : MonoBehaviour
{
    [SerializeField] private float cordLength = 5f;

    public float CordLength => cordLength;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, cordLength);
    }
}
