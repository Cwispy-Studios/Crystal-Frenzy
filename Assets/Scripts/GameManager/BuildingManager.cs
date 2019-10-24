using UnityEngine;

public class BuildingManager : MonoBehaviour
{
  [HideInInspector]
  public bool
    archeryRangeConstructed = true, archeryRangeInControl = true,
    blacksmithConstructed   = true, blacksmithInControl   = true,
    mageTowerConstructed    = true, mageTowerInControl    = true,
    brawlPitConstructed     = true, brawlPitInControl     = true;

  public void RecaptureBuilding(GameObject recapturedBuilding)
  {
    switch (recapturedBuilding.GetComponent<BuildingType>().buildingType)
    {
      case BUILDING_TYPE.FARM:
        GameManager.resourceManager.FarmClaimed(recapturedBuilding.GetComponent<FoodProvider>());
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
        GameManager.resourceManager.FarmLost(lostBuilding.GetComponent<FoodProvider>());
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
