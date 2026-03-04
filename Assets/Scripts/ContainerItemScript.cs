using UnityEngine;
using UnityEngine.InputSystem;

public class ContainerItemScript : MonoBehaviour
{
    [Header("Pickup Settings")]
    public GameObject itemPrefab;
    public GameObject pickupEffectPrefab;
    public Transform effectSpawnPoint;

    [Header("Interaction")]
    public float interactionRadius = 2f;
    public TMPro.TMP_Text promptText;

    private bool canPickup = false;
    private Camera playerCamera;

    // Input Action
    public InputActionAsset inputActions;
    InputAction interactAction;

    void Awake()
    {
        var playerMap = inputActions.FindActionMap("Player");
        interactAction = playerMap.FindAction("Interact");
    }

    void OnEnable()
    {
        interactAction.Enable();
    }

    void OnDisable()
    {
        interactAction.Disable();
    }

    void Start()
    {
        playerCamera = Camera.main;
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    void Update()
    {
        canPickup = false;

        if (playerCamera != null)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, interactionRadius))
            {
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    canPickup = true;
                }
            }
        }

        if (promptText != null)
        {
            promptText.gameObject.SetActive(canPickup);
            if (canPickup)
                promptText.text = "Press Interact to pick up";
        }

        if (canPickup && promptText != null && playerCamera != null)
        {
            Vector3 direction = playerCamera.transform.position - promptText.transform.position;
            direction.y = 0;
            if (direction.sqrMagnitude > 0.001f)
                promptText.transform.rotation = Quaternion.LookRotation(-direction);
        }

        // Input System Interact
        if (canPickup && interactAction.triggered)
        {
            TryPickup();
        }
    }

    public void TryPickup()
    {
        if (!canPickup) return;

        if (itemPrefab != null)
        {
            Debug.Log("Picked up item: " + itemPrefab.name);
        }

        if (pickupEffectPrefab != null)
        {
            Transform spawnPoint = effectSpawnPoint != null ? effectSpawnPoint : transform;
            Instantiate(pickupEffectPrefab, spawnPoint.position, Quaternion.identity);
        }

        if (promptText != null)
            promptText.gameObject.SetActive(false);

        gameObject.SetActive(false);
    }
}