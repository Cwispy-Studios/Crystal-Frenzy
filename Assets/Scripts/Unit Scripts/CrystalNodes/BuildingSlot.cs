using UnityEngine;

public class BuildingSlot : MonoBehaviour
{
  [HideInInspector]
  // Whether you can view the construct panel by clicking on this object
  // This is set to false when you are not in control of the node
  public bool inControl = false;
  // Whether a building has been constructed on this slot yet
  private bool constructed = false;

  [SerializeField]
  private GameObject buildingSlot = null;

  private void Awake()
  {
    
  }

  private void Update()
  {
    if (constructed == false)
    {
      // Makes the building slot unselectable when we are not in control of the node
      buildingSlot.GetComponent<Selectable>().enabled = inControl;

      if (inControl == false)
      {
        CameraObjectSelection.SelectedUnitsList.Remove(buildingSlot);
        CameraObjectSelection.MouseHoverUnitsList.Remove(buildingSlot);
      }
    }
  }

  public void SetConstruction(GameObject constructedBuilding)
  {
    constructed = true;
    buildingSlot = constructedBuilding;
  }
}
