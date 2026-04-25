using UnityEngine;
using UnityEngine.InputSystem;
using FMOD.Studio;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float acceleration = 60f;
    [SerializeField] private float deceleration = 70f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 13f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBuffer = 0.1f;
    [SerializeField] private int maxJumps = 2;

    [Header("Gravity")]
    [SerializeField] private float fallGravityMultiplier = 2f;
    [SerializeField] private float maxFallSpeed = 20f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    private EventInstance playerFootsteps;

    private Rigidbody2D rb;
    private float baseGravity;
    private float inputX;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private bool isGrounded;
    private bool facingRight = true;
    private int jumpsRemaining;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        baseGravity = rb.gravityScale;
    }

    private void Start()
    {
        playerFootsteps = AudioManager.instance.CreateEventInstance(FMODEvents.instance.playerFootsteps);
    }

    private void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        var left = (kb.aKey.isPressed || kb.leftArrowKey.isPressed) ? 1f : 0f;
        var right = (kb.dKey.isPressed || kb.rightArrowKey.isPressed) ? 1f : 0f;
        inputX = right - left;

        var wasGrounded = isGrounded;
        isGrounded = groundCheck &&
            Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && !wasGrounded) jumpsRemaining = maxJumps;

        coyoteCounter = isGrounded ? coyoteTime : coyoteCounter - Time.deltaTime;

        var jumpPressed = kb.spaceKey.wasPressedThisFrame || kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame;
        var jumpReleased = kb.spaceKey.wasReleasedThisFrame || kb.wKey.wasReleasedThisFrame || kb.upArrowKey.wasReleasedThisFrame;

        if (jumpPressed)
        {
            jumpBufferCounter = jumpBuffer;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
            jumpsRemaining = maxJumps - 1;
        }
        else if (jumpPressed && jumpsRemaining > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0f;
            jumpsRemaining--;
        }

        if (jumpReleased && rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);

        if ((inputX > 0f && !facingRight) || (inputX < 0f && facingRight)) Flip();
    }

    private void FixedUpdate()
    {
        var targetSpeed = inputX * moveSpeed;
        var speedDiff = targetSpeed - rb.linearVelocity.x;
        var rate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
        var movement = speedDiff * rate;
        rb.AddForce(new Vector2(movement * Time.fixedDeltaTime, 0f), ForceMode2D.Impulse);

        rb.gravityScale = rb.linearVelocity.y < 0f ? baseGravity * fallGravityMultiplier : baseGravity;

        if (rb.linearVelocity.y < -maxFallSpeed)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxFallSpeed);
        
        UpdateSound();
    }

    private void Flip()
    {
        facingRight = !facingRight;
        var s = transform.localScale;
        s.x *= -1f;
        transform.localScale = s;
    }

    private void UpdateSound()
    {
        if (rb.linearVelocityX != 0 && isGrounded)
        {
            PLAYBACK_STATE playbackState;
            playerFootsteps.getPlaybackState(out playbackState);
            if (playbackState == PLAYBACK_STATE.STOPPED) playerFootsteps.start();

        }
        else
        {
            playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
