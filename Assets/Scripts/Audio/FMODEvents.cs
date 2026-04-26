using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    public static FMODEvents instance { get; private set; }
    
    [field: Header("Ambience")]
    [field: SerializeField] public EventReference ambience { get; private set; }
    [field: SerializeField] public EventReference cityAmbience { get; private set; }
    
    [field: Header("Music")]
    [field: SerializeField] public EventReference music { get; private set; }
    [field: SerializeField] public EventReference menuMusic { get; private set; }

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference playerFootsteps { get; private set; }
    [field: SerializeField] public EventReference playerGrounded { get; private set; }
    
    
    [field: Header("GameplaySFX")]
    [field: SerializeField] public EventReference attachTether { get; private set; }
    [field: SerializeField] public EventReference socketIdle { get; private set; }
    
    [field: SerializeField] public EventReference powerSwitch { get; private set; }
    
    [field: Header("MenuSFX")]
    [field: SerializeField] public EventReference button { get; private set; }

    private void Awake()
    {
        if (instance != null )
        {
            Debug.LogError("More than one FMODEvents script present in the scene.");
            Destroy(gameObject);
        }
        instance = this;
    }
    
    
}
