using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cameraTransform;
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float turnSmoothTime = 0.1f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.2f;

    [Header("Roll Settings")]
    public float rollSpeed = 10f;
    public float rollDuration = 0.5f;
    
    bool isRolling = false;
    float rollTimer = 0f;
    Vector3 rollDirection;

    [Header("Player Stats")]
    public float MaxHealth = 100f;
    public float Health = 100f;
    public float MaxMana = 50f;
    public float Mana = 50f;
    public float MaxStamina = 100f;
    public float Stamina = 100f;

    [Header("Stamina Settings")]
    public float sprintStaminaCostPerSecond = 15f;
    public float jumpStaminaCost = 10f;
    public float staminaRegenPerSecond = 10f;

    float turnSmoothVelocity;
    Vector3 velocity;
    bool isGrounded;

    // Input Actions
    public InputActionAsset inputActions;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction runAction;
    InputAction rollAction;
    InputAction interactAction;

    void Awake()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        // Setup Input Actions
        var playerMap = inputActions.FindActionMap("Player");
        moveAction = playerMap.FindAction("Move");
        jumpAction = playerMap.FindAction("Jump");
        runAction = playerMap.FindAction("Run");
        rollAction = playerMap.FindAction("Roll");
        interactAction = playerMap.FindAction("Interact");
    }

    void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        runAction.Enable();
        rollAction.Enable();
        interactAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        runAction.Disable();
        rollAction.Disable();
        interactAction.Disable();
    }

void Update()
{
    isGrounded = controller.isGrounded;
    if (isGrounded && velocity.y < 0)
        velocity.y = -2f;

    Vector2 input = moveAction.ReadValue<Vector2>();
    Vector3 direction = new Vector3(input.x, 0f, input.y).normalized;

    bool isSprinting = runAction.ReadValue<float>() > 0.5f && Stamina > 0f;
    float speed = isSprinting ? runSpeed : walkSpeed;

    // Running stamina cost
    if (isSprinting && direction.magnitude >= 0.1f)
    {
        SetStamina(Stamina - sprintStaminaCostPerSecond * Time.deltaTime);
        if (Stamina <= 0f)
            speed = walkSpeed; // fallback to walk if out of stamina
    }
    else if (!isRolling && isGrounded)
    {
        // Stamina regen when not running/rolling
        SetStamina(Mathf.Min(Stamina + staminaRegenPerSecond * Time.deltaTime, MaxStamina));
    }

    // Rolling
    if (!isRolling && rollAction.triggered && isGrounded && direction.magnitude >= 0.1f && Stamina >= jumpStaminaCost)
    {
        isRolling = true;
        rollTimer = rollDuration;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
        rollDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        SetStamina(Stamina - jumpStaminaCost); // Use jumpStaminaCost for roll, or add a separate rollStaminaCost if desired
    }

    if (isRolling)
    {
        controller.Move(rollDirection.normalized * rollSpeed * Time.deltaTime);
        rollTimer -= Time.deltaTime;
        if (rollTimer <= 0f)
        {
            isRolling = false;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        return;
    }

    if (direction.magnitude >= 0.1f)
    {
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        controller.Move(moveDir.normalized * speed * Time.deltaTime);
    }

    // Jump
    if (jumpAction.triggered && isGrounded && Stamina >= jumpStaminaCost)
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        SetStamina(Stamina - jumpStaminaCost);
    }

    velocity.y += gravity * Time.deltaTime;
    controller.Move(velocity * Time.deltaTime);
}

    public void SetHealth(float value)
    {
        Health = Mathf.Clamp(value, 0, MaxHealth);
        var ui = FindFirstObjectByType<PlayerUI>();
        if (ui != null) ui.SetHealth(Health, MaxHealth);
    }

    public void SetMana(float value)
    {
        Mana = Mathf.Clamp(value, 0, MaxMana);
        var ui = FindFirstObjectByType<PlayerUI>();
        if (ui != null) ui.SetMana(Mana, MaxMana);
    }

    public void SetStamina(float value)
    {
        Stamina = Mathf.Clamp(value, 0, MaxStamina);
        var ui = FindFirstObjectByType<PlayerUI>();
        if (ui != null) ui.SetStamina(Stamina, MaxStamina);
    }

    public void TakeDamage(float amount, Vector3 source, float knockback)
    {
        SetHealth(Health - amount);
        velocity += (transform.position - source).normalized * knockback;
    }
}

