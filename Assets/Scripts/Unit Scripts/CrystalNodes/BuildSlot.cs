using UnityEngine;

public class BuildSlot : MonoBehaviour
{
  private UIInterface uiInterface;
  private GameManager gameManager;
  private Camera playerCamera;

  private void Awake()
  {
    uiInterface = FindObjectOfType<UIInterface>();
    gameManager = FindObjectOfType<GameManager>();
    playerCamera = Camera.main;
  }

  private void Update()
  {
    // Check if this object is selected
    if ((gameManager.CurrentPhase == PHASES.PREPARATION || gameManager.CurrentPhase == PHASES.PREPARATION_DEFENSE) && playerCamera.GetComponent<CameraObjectSelection>().SelectedUnitsList.Contains(gameObject))
    {
      // Show construct panel
      uiInterface.ShowConstructPanel(gameObject);
    }

    else
    {
      uiInterface.HideConstructPanel(gameObject);
    }
  }
}
