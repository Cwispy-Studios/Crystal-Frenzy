using UnityEngine;

public class BuildingSlot : MonoBehaviour
{
  [HideInInspector]
  // Whether you can view the construct panel by clicking on this object
  // This is set to false when you are not in control of the node
  public bool inControl = false;
  public bool Constructed { get; private set; } = false;

  [SerializeField]
  private GameObject buildingSlot = null;

  private void Update()
  {
    if (Constructed == false)
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
    Constructed = true;
    buildingSlot = constructedBuilding;
  }

  public void RecaptureBuilding()
  {
    buildingSlot.GetComponent<Faction>().faction = Faction.FACTIONS.GOBLINS;
    GameManager.buildingManager.RecaptureBuilding(buildingSlot);
  }

  public void LoseBuilding()
  {
    buildingSlot.GetComponent<Faction>().faction = Faction.FACTIONS.NEUTRAL;
    GameManager.buildingManager.LoseBuilding(buildingSlot);
  }

  private void OnDisable()
  {
    buildingSlot.SetActive(false);
  }
}
