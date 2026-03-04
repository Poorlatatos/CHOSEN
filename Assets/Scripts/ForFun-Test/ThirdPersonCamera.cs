using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 5f;
    public float height = 2f;
    public float followSpeed = 8f;
    public float rotateSpeed = 120f;

    public InputActionAsset inputActions;
    InputAction lookAction;
    InputAction snapAction;

    float currentYaw = 0f;
    float targetYaw = 0f;

    void Awake()
    {
        var playerMap = inputActions.FindActionMap("Player");
        lookAction = playerMap.FindAction("Look");
        snapAction = playerMap.FindAction("SnapCamera");
    }

    void OnEnable()
    {
        lookAction.Enable();
        snapAction.Enable();
    }

    void OnDisable()
    {
        lookAction.Disable();
        snapAction.Disable();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        targetYaw += lookInput.x * rotateSpeed * Time.deltaTime;

        if (snapAction.triggered)
        {
            targetYaw = target.eulerAngles.y;
        }

        currentYaw = Mathf.LerpAngle(currentYaw, targetYaw, followSpeed * Time.deltaTime);

        Vector3 offset = Quaternion.Euler(0, currentYaw, 0) * Vector3.back * distance;
        offset.y += height;
        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        Vector3 lookTarget = target.position + Vector3.up * height * 0.5f;
        transform.LookAt(lookTarget);
    }
}