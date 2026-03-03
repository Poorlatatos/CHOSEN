using UnityEngine;

public class ContainerItemScript : MonoBehaviour
{
    [Header("Pickup Settings")]
    public GameObject itemPrefab; // The item to give to the player
    public GameObject pickupEffectPrefab; // Particle effect to play on pickup
    public Transform effectSpawnPoint; // Optional: where to spawn the effect

    [Header("Interaction")]
    public float interactionRadius = 2f;
    public TMPro.TMP_Text promptText;

    private bool canPickup = false;
    private Camera playerCamera;

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
                promptText.text = "Press E to pick up";
        }

        // Optional: rotate prompt to face camera on Y axis
        if (canPickup && promptText != null && playerCamera != null)
        {
            Vector3 direction = playerCamera.transform.position - promptText.transform.position;
            direction.y = 0;
            if (direction.sqrMagnitude > 0.001f)
                promptText.transform.rotation = Quaternion.LookRotation(-direction);
        }
    }

    // Call this from FirstPersonMovement's interact raycast
    public void TryPickup()
    {
        if (!canPickup) return;

        // Give item to player (expand this for your inventory system)
        if (itemPrefab != null)
        {
            // Example: instantiate item in front of player, or add to inventory
            // Instantiate(itemPrefab, playerCamera.transform.position + playerCamera.transform.forward, Quaternion.identity);
            // TODO: Replace with inventory logic if needed
            Debug.Log("Picked up item: " + itemPrefab.name);
        }

        // Play particle effect
        if (pickupEffectPrefab != null)
        {
            Transform spawnPoint = effectSpawnPoint != null ? effectSpawnPoint : transform;
            Instantiate(pickupEffectPrefab, spawnPoint.position, Quaternion.identity);
        }

        // Hide prompt and disable container
        if (promptText != null)
            promptText.gameObject.SetActive(false);

        // Optionally destroy or disable the container
        gameObject.SetActive(false);
    }
}