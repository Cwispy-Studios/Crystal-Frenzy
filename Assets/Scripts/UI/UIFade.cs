using System.Collections;
using UnityEngine;

public class UIFade : MonoBehaviour
{
  private const float FADE_SPEED = 0.5f;
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

    yield return null;
  }
}
