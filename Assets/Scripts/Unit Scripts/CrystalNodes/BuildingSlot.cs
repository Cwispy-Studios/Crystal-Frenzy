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

  private Camera playerCamera;

  private void Awake()
  {
    playerCamera = Camera.main;
  }

  private void Update()
  {
    if (Constructed == false)
    {
      // Makes the building slot unselectable when we are not in control of the node
      buildingSlot.GetComponent<Selectable>().enabled = inControl;

      if (inControl == false)
      {
        playerCamera.GetComponent<CameraObjectSelection>().SelectedUnitsList.Remove(buildingSlot);
        playerCamera.GetComponent<CameraObjectSelection>().MouseHoverUnitsList.Remove(buildingSlot);
      }
    }
  }

  public void SetConstruction(GameObject constructedBuilding)
  {
    Constructed = true;
    buildingSlot = constructedBuilding;

    switch (constructedBuilding.GetComponent<BuildingType>().buildingType)
    {
      case BUILDING_TYPE.ARCHERY_RANGE:
        GameManager.buildingManager.archeryRangeConstructed = true;
        GameManager.buildingManager.archeryRangeInControl = true;
        break;

      case BUILDING_TYPE.BLACKSMITH:
        GameManager.buildingManager.blacksmithConstructed = true;
        GameManager.buildingManager.blacksmithInControl = true;
        break;

      case BUILDING_TYPE.BRAWL_PIT:
        GameManager.buildingManager.brawlPitConstructed = true;
        GameManager.buildingManager.brawlPitInControl = true;
        break;

      case BUILDING_TYPE.MAGE_TOWER:
        GameManager.buildingManager.mageTowerConstructed = true;
        GameManager.buildingManager.mageTowerInControl = true;
        break;
    }
  }

  public void RecaptureBuilding()
  {
    buildingSlot.GetComponent<Faction>().faction = Faction.FACTIONS.GOBLINS;
    buildingSlot.GetComponent<ConstructableBuilding>().SetBuildingButtonInteractable(true);
    GameManager.buildingManager.RecaptureBuilding(buildingSlot);
  }

  public void LoseBuilding()
  {
    buildingSlot.GetComponent<Faction>().faction = Faction.FACTIONS.NEUTRAL;
    buildingSlot.GetComponent<ConstructableBuilding>().SetBuildingButtonInteractable(false);
    GameManager.buildingManager.LoseBuilding(buildingSlot);
  }

  public bool HasBuildingSlot()
  {
    return buildingSlot != null;
  }

  private void OnDisable()
  {
    buildingSlot.SetActive(false);
  }
}
