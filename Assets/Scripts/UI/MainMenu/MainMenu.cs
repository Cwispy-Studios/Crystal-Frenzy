using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
  public delegate void NextScene();

  [SerializeField]
  private Button playButton = null, quitButton = null, controlsButton = null;

  [SerializeField]
  private Image fadeScreen = null, greyScreen = null, controlsImage = null;

  private const float FADE_DURATION = 2f;

  private FMOD.Studio.Bus Master;

  [FMODUnity.EventRef]
  public string openControlsSound = "", closeControlsSound = "";

  private void Awake()
  {
    playButton.onClick.AddListener(delegate { FadeScreen(PlayGame); });
    
    quitButton.onClick.AddListener(delegate { FadeScreen(QuitGame); });

    controlsButton.onClick.AddListener(Controls);

    Master = FMODUnity.RuntimeManager.GetBus("bus:/Master");

    Master.setVolume(1);
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      if (controlsImage.gameObject.activeSelf)
      {
        FMODUnity.RuntimeManager.PlayOneShot(closeControlsSound);
        ExitControlsScreen();
      }
    }
  }

  private void PlayGame()
  {
    PlayerPrefs.SetInt("sceneToLoad",1);
    SceneManager.LoadScene("LoadingScreen");
  }

  private void QuitGame()
  {
    Application.Quit();
  }

  private void Controls()
  {
    playButton.interactable = false;
    controlsButton.interactable = false;
    quitButton.interactable = false;

    greyScreen.gameObject.SetActive(true);
    controlsImage.gameObject.SetActive(true);

    FMODUnity.RuntimeManager.PlayOneShot(openControlsSound);
  }

  private void ExitControlsScreen()
  {
    playButton.interactable = true;
    controlsButton.interactable = true;
    quitButton.interactable = true;

    greyScreen.gameObject.SetActive(false);
    controlsImage.gameObject.SetActive(false);
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
