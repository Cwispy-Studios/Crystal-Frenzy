﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
  public delegate void NextScene();

  [SerializeField]
  private GameObject uiInterface = null, confirmationPanel = null;

  [SerializeField]
  private Button continueButton = null, controlsButton = null, restartButton = null, quitButton = null, yesButton = null, noButton = null;

  [SerializeField]
  private Image confirmationGreyScreen = null, fadeScreen = null;

  [SerializeField]
  private Text confirmationText = null;

  private FMODUnity.StudioEventEmitter musicEmitter;

  private const float FADE_DURATION = 2f;

  private void Awake()
  {
    continueButton.onClick.AddListener(ContinueGame);
    controlsButton.onClick.AddListener(ControlsMenu);
    restartButton.onClick.AddListener(delegate { AreYouSure(RestartGame, "Restart The Game?"); });
    quitButton.onClick.AddListener(delegate { AreYouSure(QuitGame, "Quit The Game?"); });

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

  private void RestartGame()
  {
    SceneManager.LoadScene("Level");
  }

  private void QuitGame()
  {
    SceneManager.LoadScene("MainMenu");
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

  private void ReturnToMenu()
  {
    continueButton.interactable = true;
    controlsButton.interactable = true;
    quitButton.interactable = true;

    confirmationGreyScreen.enabled = false;
    confirmationPanel.SetActive(false);
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
      musicEmitter.SetParameter("LevelVolume", 1 - ((Time.time - startTime) / FADE_DURATION));

      Debug.Log(1 - ((Time.time - startTime) / FADE_DURATION));

      yield return 1;
    }

    fadeScreen.color = targetColor;
    musicEmitter.SetParameter("LevelVolume", 0);

    nextScene();
  }

  private Coroutine StartFade(NextScene nextScene)
  {
    return StartCoroutine(LerpFadeScreen(nextScene));
  }
}
