using System;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public static InputController instance;

    public static Action OnMove;
    public static Action OnJumpPressed;
    public static Action OnJumpReleased;
    public static Action OnInteract;
    public static Action OnClimbPressed;
    public static Action OnClimbReleased;
    
    public static Vector2 MoveInput { get; private set; }
    public static bool IsClimbing { get; private set; } 
    
    private InputSystem _input;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        
        _input = new InputSystem();
        
        _input.Player.Jump.started += _ => OnJumpPressed?.Invoke();
        _input.Player.Jump.canceled += _ => OnJumpReleased?.Invoke();
        
        _input.Player.Interact.started += _ => OnInteract?.Invoke();
        
        _input.Player.Climb.started += _ => { OnClimbPressed?.Invoke(); IsClimbing = true; };
        _input.Player.Climb.canceled += _ => { OnClimbReleased?.Invoke(); IsClimbing = false; };
        _input.Enable();
    }
    
    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        MoveInput = _input.Player.Move.ReadValue<Vector2>();
    }
    
    private void OnDestroy()
    {
        if (_input != null) _input.Disable();
    }
}
