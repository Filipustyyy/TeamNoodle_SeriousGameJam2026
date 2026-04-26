using UnityEngine;

public class PowerSwitch : MonoBehaviour, IInteractable
{
    [Header("Connections")]
    [SerializeField] private PowerOutlet targetOutlet;
    
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;
    
    private bool _isOn;

    void Awake()
    {
        _isOn = false;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = offSprite;
    }
    
    public void Interact(GameObject interactor)
    {
        if (targetOutlet != null && !_isOn)
        {
            targetOutlet.PowerOn();
            _isOn = true;
            _spriteRenderer.sprite = onSprite;
            AudioManager.instance.PlayOneShot(FMODEvents.instance.powerSwitch, transform.position);
        }
        
    }
}
