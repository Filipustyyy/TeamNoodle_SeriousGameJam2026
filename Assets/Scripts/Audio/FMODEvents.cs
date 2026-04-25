using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    public static FMODEvents instance { get; private set; }

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference playerFootsteps { get; private set; }
    
    [field: Header("AttachTetherSFX")]
    [field: SerializeField] public EventReference attachTether { get; private set; }

    private void Awake()
    {
        if (instance is not null ) Debug.LogError("More than one FMODEvents script present in the scene.");
        instance = this;
    }
    
    
}
