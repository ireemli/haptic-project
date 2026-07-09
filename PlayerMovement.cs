using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string isMovingParam = "isMoving";

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 facingDirection = Vector2.down;

    public Vector2 FacingDirection => facingDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void OnMove(InputValue value)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGloveMode) return;
        moveInput = value.Get<Vector2>();

        if (moveInput.sqrMagnitude > 1f)
            moveInput.Normalize();

        if (moveInput.sqrMagnitude > 0.01f)
            facingDirection = moveInput.normalized;

        if (animator != null)
        {
            bool isMoving = moveInput.sqrMagnitude > 0.01f;
            animator.SetBool(isMovingParam, isMoving);
        }
    }
    private void FixedUpdate()
{
    if (GameManager.Instance != null && GameManager.Instance.IsGloveMode) return;
    rb.linearVelocity = moveInput * speed;
}
}