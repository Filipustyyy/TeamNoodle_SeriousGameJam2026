using UnityEngine;

public class PowerSwitch : MonoBehaviour, IInteractable
{
    [Header("Connections")]
    [SerializeField] private PowerOutlet targetOutlet;
    
    [Header("Sprites")]
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;
    [SerializeField] private GameObject hintSpriteObject;
    
    private SpriteRenderer _spriteRenderer;
    
    private bool _isOn;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _isOn = false;
        _spriteRenderer.sprite = offSprite;
        HideHint();
    }
    
    public void Interact(GameObject interactor)
    {
        if (targetOutlet != null && !_isOn)
        {
            targetOutlet.PowerOn();
            _isOn = true;
            _spriteRenderer.sprite = onSprite;
            HideHint();
            AudioManager.instance.PlayOneShot(FMODEvents.instance.powerSwitch, transform.position);
        }
        
    }

    public void DisplayHint()
    {
        if (_isOn) return;
        hintSpriteObject.SetActive(true);
    }

    public void HideHint()
    {
        hintSpriteObject.SetActive(false);
    }
}
