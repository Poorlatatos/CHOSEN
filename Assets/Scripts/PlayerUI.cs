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

    public void FadeRestUI(bool show, float duration = 0.3f)
    {
        if (restUICanvasGroup == null) return;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        if (show)
            restUI.SetActive(true); // Only activate when fading in

        fadeCoroutine = StartCoroutine(FadeCanvasGroup(restUICanvasGroup, show ? 1 : 0, duration));
    }

    private System.Collections.IEnumerator FadeCanvasGroup(CanvasGroup cg, float target, float duration)
    {
        float start = cg.alpha;
        float time = 0f;
        while (time < duration)
        {
            cg.alpha = Mathf.Lerp(start, target, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        cg.alpha = target;
        if (target == 0)
            restUI.SetActive(false); // Only deactivate after fade out
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