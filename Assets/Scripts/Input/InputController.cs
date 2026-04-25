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
        
        Debug.Log("Input Controller Awake ran!");
        
        _input.Player.Jump.started += _ => OnJump?.Invoke();
        _input.Player.Interact.started += _ => OnInteract?.Invoke();
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
