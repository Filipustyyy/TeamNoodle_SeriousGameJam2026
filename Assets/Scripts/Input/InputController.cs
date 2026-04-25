using System;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public static InputController instance;

    public static Action OnMove;
    public static Action OnJump;
    public static Action OnInteract;
    
    private InputSystem _input;

    void Awake()
    {
        instance = this;
        _input = new InputSystem();
        
        _input.Player.Jump.performed += _ => OnJump?.Invoke();
        _input.Player.Interact.performed += _ => OnInteract?.Invoke();
        _input.Enable();
    }
    
    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        Vector2 moveInput = _input.Player.Move.ReadValue<Vector2>();
        if (moveInput.x != 0 || moveInput.y != 0)
        {
            OnMove?.Invoke();
        }
    }
}
