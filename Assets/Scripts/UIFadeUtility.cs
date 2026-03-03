using UnityEngine;
using System.Collections;

public static class UIFadeUtility
{
    public static IEnumerator FadeCanvasGroup(CanvasGroup cg, float target, float duration, System.Action onComplete = null)
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
        onComplete?.Invoke();
    }
}