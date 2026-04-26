using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class CameraBounds : MonoBehaviour
{
    void Start()
    {
        var confiner = Object.FindAnyObjectByType<CinemachineConfiner2D>();
        if (confiner == null) return;
        confiner.BoundingShape2D = GetComponent<PolygonCollider2D>();
        confiner.InvalidateBoundingShapeCache();
    }

    void OnDestroy()
    {
        var confiner = Object.FindAnyObjectByType<CinemachineConfiner2D>();
        if (confiner != null && confiner.BoundingShape2D == GetComponent<PolygonCollider2D>())
            confiner.BoundingShape2D = null;
    }
}