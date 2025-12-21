using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject model;
    [SerializeField] private CapsuleCollider col;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 6.0f;
    [SerializeField] private float sprintSpeed = 3.5f;
    [SerializeField] private float dashForce = 5.0f;
    [SerializeField][ReadOnly] private float currentSpeed;
    [SerializeField][ReadOnly] private Vector3 moveInput;
    [SerializeField][ReadOnly] private Vector3 moveVelocity;
    [SerializeField] private bool isSprinting;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField][ReadOnly] private bool isGrounded;

    [Header("Physics Materials")]
    [SerializeField] private PhysicsMaterial frictionless;
    [SerializeField] private PhysicsMaterial stickyFeet;

    [Header("Game Logic")]
    [ReadOnly] public float loseHeight;


    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        HandleInput();
        HandleJump();
        UpdateAnimator();

        if (GameManager.Instance != null && GameManager.Instance.IsGameRunning)
        {
            if (LoseBarrierManager.Instance != null)
            {
                loseHeight = LoseBarrierManager.Instance.CurrentLoseHeight;
            }

            if (transform.position.y < loseHeight)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    // Frame-rate independent update
    private void FixedUpdate()
    {
        // Grounded check
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundCheckRadius,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );

        // Switch Physics Matrial depending on if the user is moving and grounded
        col.sharedMaterial = (isGrounded && moveInput == Vector3.zero) ? stickyFeet : frictionless;
        col.enabled = false; col.enabled = true;

        HandleMovement();
    }

    // Handles all inputs that directly affect the player's movement (except jump)
    private void HandleInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 cameraRight = Vector3.Scale(Camera.main.transform.right, new Vector3(1, 0, 1)).normalized;

        moveInput = (cameraForward * vertical + cameraRight * horizontal).normalized;

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Dash();
        }

        isSprinting = Input.GetKey(KeyCode.LeftShift);
        float speed = isSprinting ? sprintSpeed : walkSpeed;
        moveVelocity = moveInput * speed;
        currentSpeed = moveVelocity.magnitude;
    }

    // Handles all logic revolving around jumping
    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rigidBody.linearVelocity = new Vector3(rigidBody.linearVelocity.x, jumpForce, rigidBody.linearVelocity.z);
            animator.SetBool("Jump", true);
        }
        else if (isGrounded)
        {
            animator.SetBool("Jump", false);
        }
    }

    // TODO A very simple dash, need to make more robust
    private void Dash()
    {
        if (moveInput == Vector3.zero) return;

        Vector3 dashDirection = moveInput.normalized;
        rigidBody.AddForce(dashDirection * dashForce, ForceMode.VelocityChange);
    }

    // Handles rigidbody movement based on user input
    private void HandleMovement()
    {
        // Only update horizontal velocity
        Vector3 horizontalVelocity = moveVelocity;
        horizontalVelocity.y = rigidBody.linearVelocity.y;

        rigidBody.linearVelocity = horizontalVelocity;

        // Rotate model toward movement
        if (moveInput.sqrMagnitude > 0.001f)
        {
            model.transform.forward = moveInput;
        }
        // Zero out movement velocity
        if (moveVelocity.sqrMagnitude < 0.0001f)
        {
            moveVelocity = Vector3.zero;
        }

    }

    // State updates for the animator
    private void UpdateAnimator()
    {
        animator.SetBool("Grounded", isGrounded);
        animator.SetFloat("Speed", currentSpeed);
    }


    // TODO Implement!!
    private void OnLand() { }
    private void OnFootstep() { }

    // Debug
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}



