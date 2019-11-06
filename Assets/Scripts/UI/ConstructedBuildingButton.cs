using UnityEngine;
using UnityEngine.UI;

public class ConstructedBuildingButton : MonoBehaviour
{
  private GameObject constructedBuilding;
  private Camera playerCamera;
  private GameManager gameManager;

  private void Awake()
  {
    playerCamera = Camera.main;
    gameManager = FindObjectOfType<GameManager>();
  }

  public void SetBuilding(GameObject building)
  {
    constructedBuilding = building;
    constructedBuilding.GetComponent<ConstructableBuilding>().SetBuildingButton(this);
    GetComponent<Button>().onClick.AddListener(CenterOnBuilding);
  }

  public void SetButtonInteractable(bool interactable)
  {
    GetComponent<Button>().interactable = interactable;
  }

  private void CenterOnBuilding()
  {
    playerCamera.GetComponent<CameraManager>().PointCameraAtPosition(constructedBuilding.transform.position, playerCamera.GetComponent<CameraControls>().birdsEyeViewMode, true, 0, 0.5f);

    if (gameManager.CurrentPhase == PHASES.PREPARATION || gameManager.CurrentPhase == PHASES.PREPARATION_DEFENSE)
    {
      playerCamera.GetComponent<CameraObjectSelection>().ClearSelectionList();
      playerCamera.GetComponent<CameraObjectSelection>().AddObjectToSelectionList(constructedBuilding);
    }
  }
}
