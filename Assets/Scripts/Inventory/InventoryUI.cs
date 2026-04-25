using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    void OnEnable()
    {
        InventoryBus.OnChanged += HandleChanged;
    }

    void OnDisable()
    {
        InventoryBus.OnChanged -= HandleChanged;
    }

    void Start()
    {
        if (Inventory.instance != null)
        {
            HandleChanged(Inventory.instance.Current);
        }
        else
        {
            HandleChanged(null);
        }
    }

    private void HandleChanged(Item item)
    {
        if (iconImage == null) return;

        if (item == null || item.Icon == null)
        {
            iconImage.enabled = false;
            return;
        }

        iconImage.sprite = item.Icon;
        iconImage.enabled = true;
    }
}