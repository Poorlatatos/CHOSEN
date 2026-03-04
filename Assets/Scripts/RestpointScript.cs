using UnityEngine;
using UnityEngine.InputSystem;

public class RestpointScript : MonoBehaviour
{


    private ThirdPersonMovement player;
    private PlayerUI playerUI;
    private bool canRest = false;

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
        player = FindFirstObjectByType<ThirdPersonMovement>();
        playerUI = FindFirstObjectByType<PlayerUI>();
        if (playerUI != null && playerUI.restUI != null)
        {
            playerUI.restUI.SetActive(false);
            var cg = playerUI.restUI.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 0;
        }
    }

    void Update()
    {
        // Show prompt only if player is in trigger
        if (canRest && interactAction.triggered)
        {
            TryRest();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ThirdPersonMovement>() != null)
        {
            canRest = true;
            if (playerUI != null && playerUI.restUI != null)
                playerUI.FadeRestUI(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<ThirdPersonMovement>() != null)
        {
            canRest = false;
            if (playerUI != null && playerUI.restUI != null)
                playerUI.FadeRestUI(false);
        }
    }

    public void TryRest()
    {
        if (!canRest || player == null) return;

        player.SetHealth(player.MaxHealth);
        player.SetMana(player.MaxMana);
        player.SetStamina(player.MaxStamina);

        Debug.Log("Rested at bonfire!");
        if (playerUI != null && playerUI.restUI != null)
            playerUI.FadeRestUI(false);
    }
}