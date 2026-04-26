using UnityEngine;

public class PickupBob : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.15f;
    [SerializeField] private float frequency = 1.5f;
    [SerializeField] private float phaseOffset = 0f;

    private Vector3 startLocalPos;

    private void Awake()
    {
        startLocalPos = transform.localPosition;
    }

    private void Update()
    {
        float y = Mathf.Sin((Time.time + phaseOffset) * frequency * Mathf.PI * 2f) * amplitude;
        transform.localPosition = startLocalPos + new Vector3(0f, y, 0f);
    }
}
