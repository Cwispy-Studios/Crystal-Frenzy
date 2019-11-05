using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConstructButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
  [SerializeField]
  private GameObject constructableBuildingPrefab = null;
  private BUILDING_TYPE constructableBuildingType;

  private UIInterface uiInterface = null;

  [HideInInspector]
  public GameObject connectedNode;

  [HideInInspector]
  public bool available = true;

  [SerializeField]
  private ConstructedBuildingsPanel constructedBuildingPanel = null;

  private GameManager gameManager;

  private void Awake()
  {
    uiInterface = FindObjectOfType<UIInterface>();
    constructableBuildingType = constructableBuildingPrefab.GetComponent<BuildingType>().buildingType;

    gameManager = FindObjectOfType<GameManager>();
  }

  private void Start()
  {
    GetComponent<Button>().onClick.AddListener(ConstructBuilding);
  }

  private void Update()
  {
    GetComponent<Button>().interactable = (constructableBuildingPrefab.GetComponent<ConstructableBuilding>().goldCost <= GameManager.resourceManager.Gold && available);
  }

  private void ConstructBuilding()
  {
    GameManager.resourceManager.SpendGold(constructableBuildingPrefab.GetComponent<ConstructableBuilding>().goldCost);

    var constructedBuilding = Instantiate(constructableBuildingPrefab, connectedNode.transform.position, connectedNode.transform.rotation, connectedNode.transform.parent);
    connectedNode.GetComponentInParent<BuildingSlot>().SetConstruction(constructedBuilding);
    constructedBuildingPanel.AddNewConstructedBuilding(constructedBuilding, GetComponent<Image>().sprite);

    uiInterface.HideBuildingTooltipPopup();
    uiInterface.HideConstructPanel(connectedNode);
    Destroy(connectedNode);
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    if (gameManager.CurrentPhase == PHASES.ESCORT || gameManager.CurrentPhase == PHASES.DEFENSE)
    {
      return;
    }

    string buildingName = "";
    int cost = constructableBuildingPrefab.GetComponent<ConstructableBuilding>().goldCost;
    string description = "";
    string constructed = "";
    
    switch (constructableBuildingPrefab.GetComponent<BuildingType>().buildingType)
    {
      case BUILDING_TYPE.FARM:
        Farm farm = constructableBuildingPrefab.GetComponent<Farm>();
        buildingName = "Farm";
        description = "Increases maximum unit cap by <color=magenta>" + farm.farmUpgradeProperties[0].foodProvided + 
          "</color>, allowing you to command more units in battle. ";

        break;

      case BUILDING_TYPE.ARCHERY_RANGE:
        buildingName = "Archery Range";
        description = "Allows you command of <color=magenta>Shooty Boys</color>. Ranged imps that rain arrows down upon your enemies from a distance.";

        if (GameManager.buildingManager.archeryRangeConstructed)
        {
          constructed = "BUILDING ALREADY CONSTRUCTED";
        }

        break;

      case BUILDING_TYPE.BLACKSMITH:
        buildingName = "Blacksmith";
        description = "Allows you to upgrade <color=magenta>Stabby Boys</color>, <color=magenta>Shooty Boys</color> and <color=magenta>Brutes</color>.";

        if (GameManager.buildingManager.blacksmithConstructed)
        {
          constructed = "BUILDING ALREADY CONSTRUCTED";
        }

        break;

      case BUILDING_TYPE.BRAWL_PIT:
        buildingName = "Brawl Pit";
        description = "Allows you command of <color=magenta>Brutes</color>. Heavy melee units that are able to both soak up and inflict massive damage.";

        if (GameManager.buildingManager.brawlPitConstructed)
        {
          constructed = "BUILDING ALREADY CONSTRUCTED";
        }

        break;

      case BUILDING_TYPE.MAGE_TOWER:
        buildingName = "Mage Tower";
        description = "Allows you command of <color=magenta>Warlocks</color>. Gifted magic users able to heal your army while inflicting unnatural devastation. Also allows upgrading of Warlocks.";

        if (GameManager.buildingManager.mageTowerConstructed)
        {
          constructed = "BUILDING ALREADY CONSTRUCTED";
        }

        break;
    }

    uiInterface.ShowBuildingTooltipPopup(buildingName, cost, description, constructed);
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    uiInterface.HideBuildingTooltipPopup();
  }
}
