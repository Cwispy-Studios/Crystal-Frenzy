using UnityEngine;

public class BuildSlot : MonoBehaviour
{
  private UIInterface uiInterface;

  private void Awake()
  {
    uiInterface = FindObjectOfType<UIInterface>();
  }

  private void Update()
  {
    // Check if this object is selected
    if ((GameManager.CurrentPhase == PHASES.PREPARATION || GameManager.CurrentPhase == PHASES.PREPARATION_DEFENSE) && CameraObjectSelection.SelectedUnitsList.Contains(gameObject))
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
