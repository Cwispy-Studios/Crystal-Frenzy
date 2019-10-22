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

  public GameObject connectedNode;

  [HideInInspector]
  public bool available = true;

  private void Awake()
  {
    uiInterface = FindObjectOfType<UIInterface>();
  }

  private void Start()
  {
    GetComponent<Button>().onClick.AddListener(ConstructBuilding);
  }

  private void Update()
  {
    switch (buildingType)
    {
      case BUILDING_TYPE.FARM:
        GetComponent<Button>().interactable = farmPrefab.GetComponent<ConstructableBuilding>().goldCost <= GameManager.resourceManager.Gold && available;
        break;

      case BUILDING_TYPE.ARCHERY_RANGE:
        GetComponent<Button>().interactable = archeryRangePrefab.GetComponent<ConstructableBuilding>().goldCost <= GameManager.resourceManager.Gold && available;
        break;

      case BUILDING_TYPE.BLACKSMITH:
        GetComponent<Button>().interactable = blacksmithPrefab.GetComponent<ConstructableBuilding>().goldCost <= GameManager.resourceManager.Gold && available;
        break;

      case BUILDING_TYPE.BRAWL_PIT:
        GetComponent<Button>().interactable = brawlPitPrefab.GetComponent<ConstructableBuilding>().goldCost <= GameManager.resourceManager.Gold && available;
        break;

      case BUILDING_TYPE.MAGE_TOWER:
        GetComponent<Button>().interactable = mageTowerPrefab.GetComponent<ConstructableBuilding>().goldCost <= GameManager.resourceManager.Gold && available;
        break;
    }
  }

  private void ConstructBuilding()
  {
    switch (buildingType)
    {
      case BUILDING_TYPE.FARM:
        connectedNode.GetComponentInParent<BuildingSlot>().SetConstruction(Instantiate(farmPrefab, connectedNode.transform.position, connectedNode.transform.rotation, connectedNode.transform.parent));
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

    uiInterface.HideBuildingTooltipPopup();
    uiInterface.HideConstructPanel(connectedNode);
    Destroy(connectedNode);
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    string buildingName = "";
    int cost = 0;
    string description = "";
    string constructed = "";
    
    switch (buildingType)
    {
      case BUILDING_TYPE.FARM:
        buildingName = "Farm";
        description = "Increases maximum unit cap by <color=orange>3</color>, allowing you to command more units in battle. ";

        cost = farmPrefab.GetComponent<ConstructableBuilding>().goldCost;

        break;

      case BUILDING_TYPE.ARCHERY_RANGE:
        buildingName = "Archery Range";
        description = "Allows you command of <color=orange>Shooty Boys</color>. Ranged imps that rain arrows down upon your enemies from a distance.";

        if (GameManager.buildingManager.archeryRangeConstructed)
        {
          constructed = "BUILDING ALREADY CONSTRUCTED";
        }

        cost = archeryRangePrefab.GetComponent<ConstructableBuilding>().goldCost;

        break;

      case BUILDING_TYPE.BLACKSMITH:
        buildingName = "Blacksmith";
        description = "Allows you to upgrade <color=orange>Stabby Boys</color>, <color=orange>Shooty Boys</color> and <color=orange>Brutes</color>.";

        if (GameManager.buildingManager.blacksmithConstructed)
        {
          constructed = "BUILDING ALREADY CONSTRUCTED";
        }

        cost = blacksmithPrefab.GetComponent<ConstructableBuilding>().goldCost;

        break;

      case BUILDING_TYPE.BRAWL_PIT:
        buildingName = "Brawl Pit";
        description = "Allows you command of <color=orange>Brutes</color>. Heavy melee units that are able to both soak up and inflict massive damage.";

        if (GameManager.buildingManager.brawlPitConstructed)
        {
          constructed = "BUILDING ALREADY CONSTRUCTED";
        }

        cost = brawlPitPrefab.GetComponent<ConstructableBuilding>().goldCost;

        break;

      case BUILDING_TYPE.MAGE_TOWER:
        buildingName = "Mage Tower";
        description = "Allows you command of <color=orange>Warlocks</color>. Gifted magic users able to buff your army while inflicting unnatural devastation.";

        if (GameManager.buildingManager.mageTowerConstructed)
        {
          constructed = "BUILDING ALREADY CONSTRUCTED";
        }

        cost = mageTowerPrefab.GetComponent<ConstructableBuilding>().goldCost;

        break;
    }

    uiInterface.ShowBuildingTooltipPopup(buildingName, cost, description, constructed);
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    uiInterface.HideBuildingTooltipPopup();
  }
}
