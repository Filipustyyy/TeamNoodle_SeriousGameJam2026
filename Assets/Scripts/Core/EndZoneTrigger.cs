using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EndZoneTrigger : MonoBehaviour
{
    [SerializeField] private string targetScene = "EndScreen";

    private bool _triggered;

    private void Reset()
    {
        var col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_triggered) return;
        if (!other.CompareTag("Player")) return;
        _triggered = true;
        SceneTransition.Instance.LoadScene(targetScene);
    }
}
