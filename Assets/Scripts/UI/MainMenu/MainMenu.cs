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

  private const float FADE_DURATION = 2f;

  private FMOD.Studio.Bus Master;

  public bool cheatVersion;

  private void Awake()
  {
    if (cheatVersion)
    {
      playButton.onClick.AddListener(delegate { FadeScreen(CheatGame); });
    }

    else
    {
      playButton.onClick.AddListener(delegate { FadeScreen(PlayGame); });
    }
    
    quitButton.onClick.AddListener(delegate { FadeScreen(QuitGame); });

    Master = FMODUnity.RuntimeManager.GetBus("bus:/Master");

    Master.setVolume(1);
  }

  private void CheatGame()
  {
    SceneManager.LoadScene("PresentationCheatScene");
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
      Master.setVolume(1 - ((Time.time - startTime) / FADE_DURATION));

      yield return 1;
    }

    fadeScreen.color = targetColor;
    Master.setVolume(0);

    nextScene();
  }

  private Coroutine StartFade(NextScene nextScene)
  {
    return StartCoroutine(LerpFadeScreen(nextScene));
  }
}
