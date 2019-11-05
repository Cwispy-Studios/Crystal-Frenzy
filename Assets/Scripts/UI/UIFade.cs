using System.Collections;
using UnityEngine;

public class UIFade : MonoBehaviour
{
  private const float FADE_SPEED = 0.75f;

  public void BeginFadeOut()
  {
    StartCoroutine(FadeOut());
  }

  private IEnumerator FadeOut()
  {
    CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
    canvasGroup.interactable = false;

    while (canvasGroup.alpha > 0)
    {
      canvasGroup.alpha -= Time.deltaTime * FADE_SPEED;

      yield return null;
    }

    canvasGroup.alpha = 0;
  }

  public void BeginFadeIn()
  {
    StartCoroutine(FadeIn());
  }

  private IEnumerator FadeIn()
  {
    CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

    while (canvasGroup.alpha < 1)
    {
      canvasGroup.alpha += Time.deltaTime * FADE_SPEED;

      yield return null;
    }

    canvasGroup.interactable = true;
    canvasGroup.alpha = 1;
  }
}
