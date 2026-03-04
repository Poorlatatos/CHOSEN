using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerUI : MonoBehaviour
{
    [Header("UI Sliders")]
    public Slider healthBar;
    public Slider manaBar;
    public Slider staminaBar;

    [Header("Prompts")]
    public GameObject restUI;
    public CanvasGroup restUICanvasGroup;
    public Coroutine fadeCoroutine;

    [Header("Fade Durations")]
    public float restFadeInDuration = 0.3f;
    public float restFadeOutDuration = 0.1f;
    void Awake()
    {
        if (restUI != null)
        {
            restUICanvasGroup = restUI.GetComponent<CanvasGroup>();
            restUI.SetActive(false);
            if (restUICanvasGroup != null)
                restUICanvasGroup.alpha = 0;
        }
    }

    public void FadeRestUI(bool show)
    {
        if (restUICanvasGroup == null) return;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        if (show)
            restUI.SetActive(true);

        float duration = show ? restFadeInDuration : restFadeOutDuration;
        fadeCoroutine = StartCoroutine(
            UIFadeUtility.FadeCanvasGroup(
                restUICanvasGroup,
                show ? 1 : 0,
                duration,
                () => { if (!show) restUI.SetActive(false); }
            )
        );
    }
    public void SetHealth(float current, float max)
    {
        healthBar.value = current / max;
    }

    public void SetMana(float current, float max)
    {
        manaBar.value = current / max;
    }

    public void SetStamina(float current, float max)
    {
        staminaBar.value = current / max;
    }
}