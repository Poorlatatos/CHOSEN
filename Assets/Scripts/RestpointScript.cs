using UnityEngine;
using TMPro;
using System.Collections;

public class RestpointScript : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionRadius = 2f;

    private FirstPersonMovement player;
    private Camera playerCamera;
    private PlayerUI playerUI; // Reference to PlayerUI
    [HideInInspector] public bool canRest = false;

    void Start()
    {
        player = FindFirstObjectByType<FirstPersonMovement>();
        if (player != null)
        {
            playerCamera = Camera.main;
            playerUI = FindFirstObjectByType<PlayerUI>();
            if (playerUI != null && playerUI.restUI != null)
            {
                playerUI.restUI.SetActive(false);
                var cg = playerUI.restUI.GetComponent<CanvasGroup>();
                if (cg != null) cg.alpha = 0;
            }
        }
    }

    private bool lastCanRest = false; // Add this line at the top of the class

    void Update()
    {
        canRest = false;

        if (playerCamera != null)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, interactionRadius))
            {
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    canRest = true;
                }
            }
        }

        // Only call FadeRestUI when canRest changes
        if (playerUI != null && playerUI.restUI != null && canRest != lastCanRest)
        {
            playerUI.FadeRestUI(canRest);
        }
        lastCanRest = canRest;
    }

    public void TryRest()
    {
        if (!canRest) return;
        if (player == null) return;

        player.SetHealth = player.MaxHealth;
        player.SetMana = player.MaxMana;
        player.SetStamina = player.MaxStamina;

        Debug.Log("Rested at bonfire!");
        if (playerUI != null && playerUI.restUI != null)
            playerUI.FadeRestUI(false);
    }
}