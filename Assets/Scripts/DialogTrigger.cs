using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField] private string speakerName;
    [SerializeField, TextArea(2, 6)] private string[] lines;

    public void Trigger()
    {
        if (lines == null || lines.Length == 0) return;
        DialogBus.Request(new DialogRequest(speakerName, lines));
    }
}