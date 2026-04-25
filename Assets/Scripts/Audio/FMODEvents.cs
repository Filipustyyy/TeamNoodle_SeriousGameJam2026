using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    public static FMODEvents instance { get; private set; }
    
    [field: Header("Ambience")]
    [field: SerializeField] public EventReference ambience { get; private set; }

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference playerFootsteps { get; private set; }
    [field: SerializeField] public EventReference playerGrounded { get; private set; }
    
    
    [field: Header("SocketSFX")]
    [field: SerializeField] public EventReference attachTether { get; private set; }
    [field: SerializeField] public EventReference socketIdle { get; private set; }

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
