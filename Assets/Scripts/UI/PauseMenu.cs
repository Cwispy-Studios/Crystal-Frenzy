using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
  [SerializeField]
  private GameObject uiInterface = null, confirmationPanel = null;

  [SerializeField]
  private Button continueButton = null, controlsButton = null, quitButton = null, yesButton = null, noButton = null;

  [SerializeField]
  private Image confirmationGreyScreen = null;

  private void Awake()
  {
    continueButton.onClick.AddListener(ContinueGame);
    controlsButton.onClick.AddListener(ControlsMenu);
    quitButton.onClick.AddListener(AreYouSure);

    yesButton.onClick.AddListener(QuitGame);
    noButton.onClick.AddListener(ReturnToMenu);
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
}
