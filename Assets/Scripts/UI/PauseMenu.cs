using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
  public delegate void NextScene();

  [SerializeField]
  private GameObject uiInterface = null, menuPanel = null, confirmationPanel = null;

  [SerializeField]
  private Button continueButton = null, controlsButton = null, restartButton = null, quitButton = null, yesButton = null, noButton = null;

  [SerializeField]
  private Image greyScreen = null, confirmationGreyScreen = null, fadeScreen = null, controlsImage = null;

  [SerializeField]
  private Text confirmationText = null;

  [FMODUnity.EventRef]
  public string menuOpenSound = "", menuCloseSound = "";

  private FMOD.Studio.Bus Master;

  private const float FADE_DURATION = 2f;

  private void Awake()
  {
    continueButton.onClick.AddListener(ContinueGame);
    controlsButton.onClick.AddListener(Controls);
    restartButton.onClick.AddListener(delegate { AreYouSure(RestartGame, "Restart The Game?"); });
    quitButton.onClick.AddListener(delegate { AreYouSure(QuitGame, "Quit The Game?"); });

    noButton.onClick.AddListener(ReturnToMenu);

    Master = FMODUnity.RuntimeManager.GetBus("bus:/Master");

    Master.setVolume(1);
  }

  private void OnEnable()
  {
    confirmationGreyScreen.enabled = false;
    confirmationPanel.SetActive(false);

    FMODUnity.RuntimeManager.PlayOneShotAttached(menuOpenSound, Camera.main.gameObject);

    Time.timeScale = 0;
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      if (confirmationPanel.activeSelf || controlsImage.gameObject.activeSelf)
      {
        if (controlsImage.gameObject.activeSelf)
        {
          FMODUnity.RuntimeManager.PlayOneShot(menuCloseSound);
        }
        
        ReturnToMenu();
      }

      else
      {
        ContinueGame();
      }
    }
  }

  private void ContinueGame()
  {
    FMODUnity.RuntimeManager.PlayOneShotAttached(menuCloseSound, Camera.main.gameObject);

    Time.timeScale = 1f;
    uiInterface.GetComponent<CanvasGroup>().interactable = true;
    gameObject.SetActive(false);
  }

  private void RestartGame()
  {
    SceneManager.LoadScene("Level");
  }

  private void QuitGame()
  {
    PlayerPrefs.SetInt("sceneToLoad",0);
    SceneManager.LoadScene("LoadingScreen");
  }


  private void AreYouSure(NextScene nextScene, string confirmText)
  {
    yesButton.onClick.RemoveAllListeners();
    yesButton.onClick.AddListener( delegate { FadeScreen(nextScene); });

    confirmationText.text = confirmText;

    continueButton.interactable = false;
    controlsButton.interactable = false;
    quitButton.interactable = false;

    confirmationGreyScreen.enabled = true;
    confirmationPanel.SetActive(true);
  }

  private void Controls()
  {
    continueButton.interactable = false;
    controlsButton.interactable = false;
    quitButton.interactable = false;

    controlsImage.gameObject.SetActive(true);
  }

  private void ReturnToMenu()
  {
    continueButton.interactable = true;
    controlsButton.interactable = true;
    quitButton.interactable = true;

    confirmationGreyScreen.enabled = false;
    confirmationPanel.SetActive(false);

    controlsImage.gameObject.SetActive(false);
  }

  private void FadeScreen(NextScene nextScene)
  {
    Time.timeScale = 1;
    GetComponent<CanvasGroup>().interactable = false;

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

  public void GameOverRestart()
  {
    gameObject.SetActive(true);
    greyScreen.enabled = false;
    menuPanel.SetActive(false);

    FadeScreen(RestartGame);
  }

  public void GameOverQuit()
  {
    gameObject.SetActive(true);
    greyScreen.enabled = false;
    menuPanel.SetActive(false);

    FadeScreen(QuitGame);
  }
}
