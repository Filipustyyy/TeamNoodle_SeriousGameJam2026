using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    private Animator animator;

    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int VelocityY = Animator.StringToHash("VelocityY");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (movement == null) movement = GetComponentInParent<PlayerMovement>();
    }

    private void Update()
    {
        if (movement == null) return;
        animator.SetFloat(Speed, Mathf.Abs(movement.MoveInputX));
        animator.SetBool(IsGrounded, movement.IsGrounded);
        animator.SetFloat(VelocityY, movement.VelocityY);
    }
}