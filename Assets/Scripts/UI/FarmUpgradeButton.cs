using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FarmUpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
  private UIInterface uiInterface = null;

  [HideInInspector]
  public Farm upgradingFarm;

  private void Awake()
  {
    uiInterface = FindObjectOfType<UIInterface>();
  }

  private void Start()
  {
    GetComponent<Button>().onClick.AddListener(UpgradeFarm);
  }

  private void Update()
  {
    GetComponent<Button>().interactable = upgradingFarm.farmUpgradeProperties[upgradingFarm.farmLevel].cost <= GameManager.resourceManager.Gold;
  }

  private void UpgradeFarm()
  {
    uiInterface.HideBuildingTooltipPopup();

    GameManager.resourceManager.SpendGold(upgradingFarm.farmUpgradeProperties[upgradingFarm.farmLevel].cost);

    if (upgradingFarm.UpgradeFarm() == false)
    {
      Destroy(gameObject);

      return;
    }

    string buildingName = "Upgrade Farm";
    int cost = upgradingFarm.farmUpgradeProperties[upgradingFarm.farmLevel].cost;
    string description = "Increases maximum unit cap from " + upgradingFarm.farmUpgradeProperties[upgradingFarm.farmLevel - 1].foodProvided +
      " to <color=magenta> " + upgradingFarm.farmUpgradeProperties[upgradingFarm.farmLevel].foodProvided + "</color>, allowing you to command more units in battle. ";
    string constructed = "";

    uiInterface.ShowBuildingTooltipPopup(buildingName, cost, description, constructed);
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    if (GameManager.CurrentPhase == PHASES.ESCORT || GameManager.CurrentPhase == PHASES.DEFENSE)
    {
      return;
    }

    string buildingName = "Upgrade Farm";
    int cost = upgradingFarm.farmUpgradeProperties[upgradingFarm.farmLevel].cost;
    string description = "Increases maximum unit cap from " + upgradingFarm.farmUpgradeProperties[upgradingFarm.farmLevel - 1].foodProvided +
      " to <color=magenta> " + upgradingFarm.farmUpgradeProperties[upgradingFarm.farmLevel].foodProvided + "</color>, allowing you to command more units in battle. ";
    string constructed = "";

    uiInterface.ShowBuildingTooltipPopup(buildingName, cost, description, constructed);
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    uiInterface.HideBuildingTooltipPopup();
  }
}
