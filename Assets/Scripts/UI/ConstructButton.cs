using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum BUILDING_TYPE
{
  FARM = 0,
  ARCHERY_RANGE,
  BLACKSMITH,
  BRAWL_PIT,
  MAGE_TOWER
}

public class ConstructButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
  [SerializeField]
  private GameObject
    farmPrefab = null,
    archeryRangePrefab = null,
    blacksmithPrefab = null,
    brawlPitPrefab = null,
    mageTowerPrefab = null;

  public BUILDING_TYPE buildingType;

  private UIInterface uiInterface = null;

  private void Awake()
  {
    uiInterface = FindObjectOfType<UIInterface>();
  }

  private void Start()
  {
    GetComponent<Button>().onClick.AddListener(ConstructBuilding);
  }

  private void ConstructBuilding()
  {
    switch (buildingType)
    {
      case BUILDING_TYPE.FARM:

        break;

      case BUILDING_TYPE.ARCHERY_RANGE:
        break;

      case BUILDING_TYPE.BLACKSMITH:
        break;

      case BUILDING_TYPE.BRAWL_PIT:
        break;

      case BUILDING_TYPE.MAGE_TOWER:
        break;
    }
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    string buildingName = "";
    string description = "";
    string constructed = "";
    
    switch (buildingType)
    {
      case BUILDING_TYPE.FARM:
        buildingName = "Farm";
        description = "Increases your maximum unit cap by <color=orange>3</color>, allowing you to command more units during the Escort and Defense Phases. " +
          "Provides food for your army by setting up a base of operations to harvest the environment for resources.";
        break;

      case BUILDING_TYPE.ARCHERY_RANGE:
        buildingName = "Archery Range";
        description = "Allows you command of <color=orange>Shooty Boys</color>. Shooty Boys are ranged imps that rain arrows down upon your enemies from a distance.";

        if (GameManager.buildingManager.archeryRangeConstructed)
        {
          constructed = "BUILDING ALREADY CONSTRUCTED";
        }

        break;

      case BUILDING_TYPE.BLACKSMITH:
        buildingName = "Blacksmith";
        description = "Allows you to upgrade <color=orange>Stabby Boys</color>, <color=orange>Shooty Boys</color> and <color=orange>Brutes</color>.";

        if (GameManager.buildingManager.blacksmithConstructed)
        {
          constructed = "BUILDING ALREADY CONSTRUCTED";
        }

        break;

      case BUILDING_TYPE.BRAWL_PIT:
        buildingName = "Brawl Pit";
        description = "Allows you command of <color=orange>Brutes</color>. Brutes are heavy melee units that are able to both soak up and inflict massive damage.";

        if (GameManager.buildingManager.brawlPitConstructed)
        {
          constructed = "BUILDING ALREADY CONSTRUCTED";
        }

        break;

      case BUILDING_TYPE.MAGE_TOWER:
        buildingName = "Mage Tower";
        description = "Allows you command of <color=orange>Warlocks</color>. Gifted magic users, Warlocks are able to both heal and buff your troops while inflicting unnatural devastation.";

        if (GameManager.buildingManager.mageTowerConstructed)
        {
          constructed = "BUILDING ALREADY CONSTRUCTED";
        }

        break;
    }
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    throw new System.NotImplementedException();
  }
}
