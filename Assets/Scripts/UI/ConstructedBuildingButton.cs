using UnityEngine;
using UnityEngine.UI;

public class ConstructedBuildingButton : MonoBehaviour
{
  private GameObject constructedBuilding;
  private Camera playerCamera;

  private void Awake()
  {
    playerCamera = Camera.main;
  }

  public void SetBuilding(GameObject building)
  {
    constructedBuilding = building;
    constructedBuilding.GetComponent<ConstructableBuilding>().SetBuildingButton(this);
    GetComponent<Button>().onClick.AddListener(delegate { playerCamera.GetComponent<CameraManager>().PointCameraAtPosition(constructedBuilding.transform.position, playerCamera.GetComponent<CameraControls>().birdsEyeViewMode, true); });
  }

  public void SetButtonInteractable(bool interactable)
  {
    GetComponent<Button>().interactable = interactable;
  }
}
