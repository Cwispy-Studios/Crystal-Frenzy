using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
  public delegate void NextScene();

  [SerializeField]
  private Button playButton = null, quitButton = null;

  [SerializeField]
  private Image fadeScreen = null;

  private FMODUnity.StudioEventEmitter musicEmitter;

  private const float FADE_DURATION = 2f;

  private void Awake()
  {
    playButton.onClick.AddListener(delegate { FadeScreen(PlayGame); });
    quitButton.onClick.AddListener(delegate { FadeScreen(QuitGame); });

    musicEmitter = Camera.main.GetComponent<FMODUnity.StudioEventEmitter>();
  }

  private void PlayGame()
  {
    SceneManager.LoadScene("Level");
  }

  private void QuitGame()
  {
    Application.Quit();
  }

  private void FadeScreen(NextScene nextScene)
  {
    musicEmitter.SetParameter("MenuVolume", 0);

    fadeScreen.gameObject.SetActive(true);
    StartFade(nextScene);
  }

  private IEnumerator LerpFadeScreen(NextScene nextScene)
  {
    float startTime = Time.time;
    Color currentColor = fadeScreen.color;
    Color targetColor = currentColor;
    targetColor.a = 1;

    while (Time.time - startTime < FADE_DURATION)
    {
      fadeScreen.color = Color.Lerp(currentColor, targetColor, (Time.time - startTime) / FADE_DURATION);
      musicEmitter.SetParameter("MenuVolume", 1 - ((Time.time - startTime) / FADE_DURATION));

      Debug.Log(1 - ((Time.time - startTime) / FADE_DURATION));

      yield return 1;
    }

    fadeScreen.color = targetColor;
    musicEmitter.SetParameter("MenuVolume", 0);

    nextScene();
  }

  private Coroutine StartFade(NextScene nextScene)
  {
    return StartCoroutine(LerpFadeScreen(nextScene));
  }
}
