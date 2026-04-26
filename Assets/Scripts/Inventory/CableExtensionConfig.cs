using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Cable Extension Config", fileName = "CableExtensionConfig")]
public class CableExtensionConfig : ScriptableObject
{
    [SerializeField] private float lengthBonus = 2f;

    public float LengthBonus => lengthBonus;
}
