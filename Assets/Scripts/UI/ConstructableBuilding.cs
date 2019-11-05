using UnityEngine;

public class ConstructableBuilding : MonoBehaviour
{
  public int goldCost;
  private ConstructedBuildingButton buildingButton = null;

  public void SetBuildingButton(ConstructedBuildingButton button)
  {
    buildingButton = button;
  }

  public void SetBuildingButtonInteractable(bool interactable)
  {
    buildingButton.SetButtonInteractable(interactable);
  }
}
