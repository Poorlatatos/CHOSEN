using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    [Header("Player Input")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Movement Settings")]
    public float speed = 6f;
    public float sprintSpeed = 12f;
    public float jumpForce = 7f;
    public float lungeForce = 10f;
    public float mouseSensitivity = 2f;
    public float airControl = 0.5f;
    public float gravity = -20f;
    public float friction = 8f;

    [Header("Player Stats")]
    public float SetHealth = 100f;
    public float SetMana = 50f;
    public float SetStamina = 75f;
    public float MaxHealth = 100f;
    public float MaxMana = 50f;
    public float MaxStamina = 75f;

    [Header("Stamina Settings")]
    public float sprintStaminaCostPerSecond = 15f;
    public float jumpStaminaCost = 10f;
    public float staminaRegenPerSecond = 10f;

    [Header("Knockback")]
    public float invincibilityDuration = 1.0f;
    private bool isInvincible = false;
    private Vector3 knockbackVelocity = Vector3.zero;
    private float knockbackDamping = 8f;

    private CharacterController controller;
    private Transform cam;
    private float verticalRotation = 0f;
    private Vector3 velocity;
    private bool isSprinting = false;

    private PlayerUI playerUI;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;

        playerUI = FindFirstObjectByType<PlayerUI>();
        if (playerUI != null)
        {
            playerUI.SetHealth(SetHealth, MaxHealth);
            playerUI.SetMana(SetMana, MaxMana);
            playerUI.SetStamina(SetStamina, MaxStamina);
        }
    }

    void Update()
    {
        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        cam.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // Sprint input (only if enough stamina)
        bool sprintInput = Input.GetKey(KeyCode.LeftShift);
        isSprinting = sprintInput && SetStamina > 0.1f;

        // Movement input
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");
        Vector3 move = transform.right * inputX + transform.forward * inputZ;
        move = move.normalized;

        // Ground check
        bool grounded = controller.isGrounded;

        // Apply friction
        if (grounded && velocity.magnitude > 0.01f)
        {
            velocity.x = Mathf.Lerp(velocity.x, 0, friction * Time.deltaTime);
            velocity.z = Mathf.Lerp(velocity.z, 0, friction * Time.deltaTime);
        }

        // Apply knockback velocity
        if (knockbackVelocity.magnitude > 0.1f)
        {
            velocity += knockbackVelocity;
            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, knockbackDamping * Time.deltaTime);
        }

        if (Input.GetKeyDown(interactKey))
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                Ray ray = new Ray(cam.transform.position, cam.transform.forward);
                RaycastHit hit;
                float maxDistance = 2f; // Default, can be overridden by RestpointScript
                if (Physics.Raycast(ray, out hit, maxDistance))
                {
                    RestpointScript restpoint = hit.collider.GetComponent<RestpointScript>();
                    if (restpoint != null)
                    {
                        restpoint.TryRest();
                    }

                    ContainerItemScript container = hit.collider.GetComponent<ContainerItemScript>();
                    if (container != null)
                    {
                        container.TryPickup();
                    }
                }
            }
        }

        // Apply movement
        if (grounded)
        {
            float currentSpeed = isSprinting ? sprintSpeed : speed;
            velocity.x = move.x * currentSpeed;
            velocity.z = move.z * currentSpeed;
            
            // Reset vertical velocity when grounded
            if (velocity.y < 0)
                velocity.y = -2f;

            // Sprint stamina drain
            if (isSprinting && move.magnitude > 0.1f)
            {
                SetStamina -= sprintStaminaCostPerSecond * Time.deltaTime;
                SetStamina = Mathf.Max(SetStamina, 0f);
            }
            else if (SetStamina < MaxStamina)
            {
                // Regenerate stamina when not sprinting
                SetStamina += staminaRegenPerSecond * Time.deltaTime;
                SetStamina = Mathf.Min(SetStamina, MaxStamina);
            }

            // Jump & lunge (only if enough stamina)
            if (Input.GetButtonDown("Jump") && SetStamina >= jumpStaminaCost)
            {
                SetStamina -= jumpStaminaCost;
                velocity.y = jumpForce;

                // Lunge if sprinting and moving
                if (isSprinting && move.magnitude > 0.1f)
                {
                    velocity.x = move.x * (currentSpeed + lungeForce);
                    velocity.z = move.z * (currentSpeed + lungeForce);
                }
            }
        }
        else
        {
            // Air control
            velocity.x = Mathf.Lerp(velocity.x, move.x * speed, airControl * Time.deltaTime);
            velocity.z = Mathf.Lerp(velocity.z, move.z * speed, airControl * Time.deltaTime);
            velocity.y += gravity * Time.deltaTime;

            // Regenerate stamina in air if not sprinting
            if (!isSprinting && SetStamina < MaxStamina)
            {
                SetStamina += staminaRegenPerSecond * Time.deltaTime;
                SetStamina = Mathf.Min(SetStamina, MaxStamina);
            }
        }

        // Update UI
        if (playerUI != null)
        {
            playerUI.SetHealth(SetHealth, MaxHealth);
            playerUI.SetMana(SetMana, MaxMana);
            playerUI.SetStamina(SetStamina, MaxStamina);
        }

        controller.Move(velocity * Time.deltaTime);
    }
    
    public void TakeDamage(float amount, Vector3 sourcePosition, float knockbackForce)
    {
        if (isInvincible) return;

        SetHealth -= amount;
        SetHealth = Mathf.Max(SetHealth, 0f);

        // Calculate knockback direction
        Vector3 dir = (transform.position - sourcePosition).normalized;
        dir.y = 0.5f; // Add upward force
        knockbackVelocity = dir * knockbackForce;

        // Start invincibility frames
        StartCoroutine(InvincibilityCoroutine());
    }

    private System.Collections.IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }
}