using UnityEngine;

public class BuildingManager : MonoBehaviour
{
  [HideInInspector]
  public bool
    archeryRangeConstructed = false, archeryRangeInControl = false,
    blacksmithConstructed   = false, blacksmithInControl   = false,
    mageTowerConstructed    = false, mageTowerInControl    = false,
    brawlPitConstructed     = false, brawlPitInControl     = false;

  public void RecaptureBuilding(GameObject recapturedBuilding)
  {
    switch (recapturedBuilding.GetComponent<BuildingType>().buildingType)
    {
      case BUILDING_TYPE.FARM:
        Farm farm = recapturedBuilding.GetComponent<Farm>();
        GameManager.resourceManager.FarmClaimed(farm, farm.farmLevel);
        break;

      case BUILDING_TYPE.ARCHERY_RANGE:
        archeryRangeInControl = true;
        break;

      case BUILDING_TYPE.BLACKSMITH:
        blacksmithInControl = true;
        break;

      case BUILDING_TYPE.BRAWL_PIT:
        brawlPitInControl = true;
        break;

      case BUILDING_TYPE.MAGE_TOWER:
        mageTowerInControl = true;
        break;
    }
  }

  public void LoseBuilding(GameObject lostBuilding)
  {
    switch (lostBuilding.GetComponent<BuildingType>().buildingType)
    {
      case BUILDING_TYPE.FARM:
        Farm farm = lostBuilding.GetComponent<Farm>();
        GameManager.resourceManager.FarmLost(farm, farm.farmLevel);
        break;

      case BUILDING_TYPE.ARCHERY_RANGE:
        archeryRangeInControl = false;
        break;

      case BUILDING_TYPE.BLACKSMITH:
        blacksmithInControl = false;
        break;

      case BUILDING_TYPE.BRAWL_PIT:
        brawlPitInControl = false;
        break;

      case BUILDING_TYPE.MAGE_TOWER:
        mageTowerInControl = false;
        break;
    }
  }
}
