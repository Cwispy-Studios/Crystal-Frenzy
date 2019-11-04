using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
  public delegate void NextScene();

  [SerializeField]
  private GameObject uiInterface = null, confirmationPanel = null;

  [SerializeField]
  private Button continueButton = null, controlsButton = null, quitButton = null, yesButton = null, noButton = null;

  [SerializeField]
  private Image confirmationGreyScreen = null;

  private FMODUnity.StudioEventEmitter musicEmitter;

  private void Awake()
  {
    continueButton.onClick.AddListener(ContinueGame);
    controlsButton.onClick.AddListener(ControlsMenu);
    quitButton.onClick.AddListener(AreYouSure);

    yesButton.onClick.AddListener(QuitGame);
    noButton.onClick.AddListener(ReturnToMenu);

    musicEmitter = Camera.main.GetComponent<FMODUnity.StudioEventEmitter>();
  }

  private void OnEnable()
  {
    confirmationGreyScreen.enabled = false;
    confirmationPanel.SetActive(false);

    Time.timeScale = 0;
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      if (confirmationPanel.activeSelf)
      {
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
    Time.timeScale = 1f;
    uiInterface.GetComponent<CanvasGroup>().interactable = true;
    gameObject.SetActive(false);
  }

  private void ControlsMenu()
  {

  }

  private void AreYouSure()
  {
    continueButton.interactable = false;
    controlsButton.interactable = false;
    quitButton.interactable = false;

    confirmationGreyScreen.enabled = true;
    confirmationPanel.SetActive(true);
  }

  private void ReturnToMenu()
  {
    continueButton.interactable = true;
    controlsButton.interactable = true;
    quitButton.interactable = true;

    confirmationGreyScreen.enabled = false;
    confirmationPanel.SetActive(false);
  }

  private void QuitGame()
  {
    Application.Quit();
  }

  //private void FadeScreen(NextScene nextScene)
  //{
  //  musicEmitter.SetParameter("MenuVolume", 0);

  //  fadeScreen.gameObject.SetActive(true);
  //  StartFade(nextScene);
  //}

  //private IEnumerator LerpFadeScreen(NextScene nextScene)
  //{
  //  float startTime = Time.time;
  //  Color currentColor = fadeScreen.color;
  //  Color targetColor = currentColor;
  //  targetColor.a = 1;

  //  while (Time.time - startTime < FADE_DURATION)
  //  {
  //    fadeScreen.color = Color.Lerp(currentColor, targetColor, (Time.time - startTime) / FADE_DURATION);
  //    musicEmitter.SetParameter("MenuVolume", 1 - ((Time.time - startTime) / FADE_DURATION));

  //    Debug.Log(1 - ((Time.time - startTime) / FADE_DURATION));

  //    yield return 1;
  //  }

  //  fadeScreen.color = targetColor;
  //  musicEmitter.SetParameter("MenuVolume", 0);

  //  nextScene();
  //}

  //private Coroutine StartFade(NextScene nextScene)
  //{
  //  return StartCoroutine(LerpFadeScreen(nextScene));
  //}
}
