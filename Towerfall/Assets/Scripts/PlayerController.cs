using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;
public class MovementController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform; // Assign your Main Camera's transform in the Inspector

    [SerializeField] private ManaBar manaBar;
    private Animator animator;

    
    // Ground Movement
    private Rigidbody rb;
    [SerializeField] private float walkSpeed = 5f;

    [SerializeField] private float runSpeed = 10f;

    private float moveSpeed;

    private float moveHorizontal;
    private float moveForward;
    private Vector2 moveInput;
    [SerializeField] private float friction = 10.0f;

    // Jumping
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float fallMultiplier = 2.5f; 
    [SerializeField] private float ascendMultiplier = 2f;

    [SerializeField] private float slowFallMultiplier = .5f; 
    [SerializeField] private float slowFallMaxSpeed = -3f;

 
    [Header("Mana Values")]

    [SerializeField] private float dashCost = 2.0f;

    [SerializeField] private float floatCost = -0.1f;
    [SerializeField] private float doubleJumpCost = 5.0f;
    private bool isGrounded = true;
    [SerializeField] private LayerMask groundLayer;
    private float groundCheckTimer = 0f;
    private float groundCheckDelay = 0.3f;
    private float playerHeight;
    private float raycastDistance;
    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1.5f;
    private bool usedDoubleJump;

    private bool isSlowFalling;
    private bool canDash = true;
    private bool isDashing = false;
    InputAction spell1;
    InputAction spell2;
    InputAction spell3;
    InputAction reset;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        animator = GetComponent<Animator>();

        // Set the raycast to be slightly beneath the player's feet
        playerHeight = GetComponent<CapsuleCollider>().height * transform.localScale.y;
        raycastDistance = (playerHeight / 6) + 0.5f;

        // Hides the mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        spell1 = InputSystem.actions.FindAction("Spell1(Q)");
        spell2 = InputSystem.actions.FindAction("Spell2(E)");
        spell3 = InputSystem.actions.FindAction("Spell3(Z)");
        reset = InputSystem.actions.FindAction("Reset");
    }

    void Update()
    {
        moveHorizontal = Input.GetAxisRaw("Horizontal");
        moveForward   = Input.GetAxisRaw("Vertical");

        // Debug or check your input
        // Debug.Log($"Horizontal: {moveHorizontal}\nVertical: {moveForward}");

        // Update animator blend-tree parameters
        animator.SetFloat("horizontal", moveHorizontal);
        //animator.SetFloat("vertical",   moveForward);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {   
            Jump();
            //ManaBar.Instance.UseMana(5.0f);
        }
        if (spell1.WasPerformedThisFrame())
        {
            SecondJump();
            ManaBar.Instance.UseMana(doubleJumpCost);
        }
        if (spell2.IsPressed()){
            ManaBar.Instance.UseMana(floatCost);
            ApplySlowFall();
        } 
        if (spell3.IsPressed()){
            
            if (ManaBar.Instance.ReturnMana() < dashCost);
            
            StartDash();
        }
        if (reset.IsPressed()){
            ManaBar.Instance.AddMana(100.0f);
        
        }
        // Ground Check 
        if (!isGrounded && groundCheckTimer <= 0f)
        {
            Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
            isGrounded = Physics.Raycast(rayOrigin, Vector3.down, raycastDistance, groundLayer);

            if (moveForward > 0)
            {
                animator.SetTrigger("landingRollTrigger");
            }

            animator.SetBool("inAir", !isGrounded);
            usedDoubleJump = false;
        }
        else
        {
            groundCheckTimer -= Time.deltaTime;
        }

        
    }

    void FixedUpdate()
    {
        /*
        // If you want zero movement control in the air, keep this condition.
        // Otherwise, remove it if you want partial air control.
        if (isGrounded)
        {
            MovePlayer();
        }
        */

        if (Input.GetKey(KeyCode.LeftShift) && (moveHorizontal != 0 || moveForward != 0)){
            Debug.Log("Left Shift Pressed");
            moveSpeed = runSpeed;
            animator.SetFloat("vertical", 1.0f);
        }
        else{
            Debug.Log("Left Shift Not Pressed");
            moveSpeed = walkSpeed;
            animator.SetFloat("vertical", moveForward / 2);
        }

        MovePlayer();

        ApplyJumpPhysics();

        
    }

    void MovePlayer()
    {
        // 1. Flatten camera direction so player moves on horizontal plane
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        Vector3 cameraRight = cameraTransform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();

        // 2. Build movement direction using camera-based forward/right
        Vector3 movement = (cameraRight * moveHorizontal + cameraForward * moveForward).normalized;
        Vector3 targetVelocity = movement * moveSpeed;

        // Update animator for forward speed (optional)
        animator.SetFloat("zSpeed", targetVelocity.z);

        // This makes it so that the camera does not rotate the character model when the player is walking. 
        // Want to change it so that it camera also does not change the direction the character moving when walking.
        /*
        if (movement.magnitude > 0.1f && moveSpeed == runSpeed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
        */

        if (movement.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }

        // 3. Apply velocity with friction if no input
        if (moveForward != 0 || moveHorizontal != 0) // If input is detected
        {
            // Replace linearVelocity -> velocity if you see errors
            rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
        }
        else
        {
            // Apply friction by smoothly reducing velocity
            rb.linearVelocity = new Vector3(
                Mathf.Lerp(rb.linearVelocity.x, 0, friction * Time.fixedDeltaTime),
                rb.linearVelocity.y,
                Mathf.Lerp(rb.linearVelocity.z, 0, friction * Time.fixedDeltaTime)
            );
        }
    }

   
    private void StartDash()
    {
        if (canDash)
        {   
            ManaBar.Instance.UseMana(dashCost); 
            StartCoroutine(DashCoroutine());
        }
    }

    
    private IEnumerator DashCoroutine()
    {
        // Set flags
        canDash = false;
        isDashing = true;
        moveInput = new Vector2(moveHorizontal,moveForward);
        // Store original gravity
        float originalGravity = rb.useGravity ? 1 : 0;
        rb.useGravity = false;
        
        // Get dash direction from movement input, or forward if no input
        Vector3 dashDirection;
        if (moveInput != Vector2.zero)
        {
            dashDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        }
        else
        {
            dashDirection = transform.forward;
        }
        
        // Apply dash force
        rb.linearVelocity = new Vector3(0, 0, 0); // Reset velocity
        rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
        
        // Optional: Add visual effects
        // Instantiate(dashEffect, transform.position, Quaternion.identity);
        
        // Wait for dash duration
        yield return new WaitForSeconds(dashDuration);
        
        // End dash
        isDashing = false;
        rb.useGravity = originalGravity > 0;
        
        // Wait for cooldown
        yield return new WaitForSeconds(dashCooldown - dashDuration);
        canDash = true;
    }
    
    private void ApplySlowFall()
    {
        // Only apply slow fall when falling
        if (!isGrounded && rb.linearVelocity.y < 0)
        {
    
            // Apply reduced gravity scale while falling and button is held
            rb.AddForce((slowFallMultiplier - 1f) * rb.mass * Physics.gravity, ForceMode.Acceleration);
            
            // Ensure we don't fall faster than our slow fall max speed
            if (rb.linearVelocity.y < slowFallMaxSpeed)
            {
                Vector3 velocity = rb.linearVelocity;
                velocity.y = slowFallMaxSpeed;
                rb.linearVelocity = velocity;
            }
        
            
            
        
        }
    }
    void Jump()
    {
        isGrounded = false;
        groundCheckTimer = groundCheckDelay;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        animator.SetTrigger("jumpTrigger");
    }
    private void SecondJump()
    {
        
        if (!(usedDoubleJump)) // Does not work with usedDouble Jump, Dont know Why
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            usedDoubleJump = true;
        }
    }
    void ApplyJumpPhysics()
    {
        // If using linearVelocity, keep it consistent. Otherwise use rb.velocity if you prefer.
        if (rb.linearVelocity.y < 0) // Falling
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0) // Rising
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (ascendMultiplier - 1) * Time.fixedDeltaTime;
        }
    }
    
}
