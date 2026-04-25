using UnityEngine;

public class PowerSwitch : MonoBehaviour, IInteractable
{
    [Header("Connections")]
    [SerializeField] private PowerOutlet targetOutlet;
    
    private bool _isOn = false;

    public void Interact(GameObject interactor)
    {
        if (targetOutlet != null && !_isOn)
        {
            targetOutlet.TogglePower();
            
            Debug.Log($"Switch flipped by {interactor.name}");
        }
        else
        {
            Debug.LogWarning("Power Switch doesnt have a target outlet assigned!");
        }
    }
}
