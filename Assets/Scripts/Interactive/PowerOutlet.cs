using UnityEngine;
using FMODUnity;

public class PowerOutlet : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isPowered = true;
    public bool IsPowered => isPowered;
    
    public bool IsPluggedIn { get; private set; }
    
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite onUnpluggedSprite;
    [SerializeField] private Sprite offUnpluggedSprite;
    [SerializeField] private Sprite onPluggedSprite;
    [SerializeField] private Sprite offPluggedSprite;
    [SerializeField] private GameObject hintSpriteObject;
    
    private StudioEventEmitter emitter;

    private void Start()
    {
        emitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.socketIdle, this.gameObject);
        emitter.Play();
        UpdateSprite();
        HideHint();
    }

    public void Interact(GameObject interactor)
    {
        if (!isPowered) return;
        var tether = interactor.GetComponent<PlayerTether>();
        if (tether == null) return;
        tether.Plug(this);
    }

    public void Plug()
    {
        IsPluggedIn = true;
        UpdateSprite();
    }

    public void Unplug()
    {
        IsPluggedIn = false;
        UpdateSprite();
    }

    public void PowerOn()
    {
        isPowered = true;
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (spriteRenderer == null) return;

        if (isPowered && IsPluggedIn)
            spriteRenderer.sprite = onPluggedSprite;
        else if (isPowered && !IsPluggedIn)
            spriteRenderer.sprite = onUnpluggedSprite;
        else if (!isPowered && IsPluggedIn)
            spriteRenderer.sprite = offPluggedSprite;
        else
            spriteRenderer.sprite = offUnpluggedSprite;
    }

    public void DisplayHint()
    {
        hintSpriteObject.SetActive(true);
    }

    public void HideHint()
    {
        hintSpriteObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        var tether = FindFirstObjectByType<PlayerTether>(FindObjectsInactive.Include);
        if (tether == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, tether.MaxCordLength);
    }
}