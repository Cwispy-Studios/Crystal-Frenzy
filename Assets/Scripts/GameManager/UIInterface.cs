using UnityEngine;

public class UIInterface : MonoBehaviour
{
  [SerializeField]
  private GameObject bottomUIPanel = null;
  [SerializeField]
  private PreparationPhaseUI preparationPhaseUI = null;

  [SerializeField]
  private UnitManager unitManager = null;
  public UnitManager UnitManager
  {
    get
    {
      return unitManager;
    }
  }

  [SerializeField]
  private LootPopup lootPopup = null;
  [SerializeField]
  private LootTargetPanel lootTargetPanel = null;
  [SerializeField]
  private UnitTooltipPopup unitTooltipPopup = null;
  [SerializeField]
  private ConstructPanel constructPanel = null;
  [SerializeField]
  private BuildingTooltipPopup buildingTooltipPopup = null;
  [SerializeField]
  private UpgradeTooltipPopup upgradeTooltipPopup = null;
  [SerializeField]
  private NodeUIPanel nodeUIPanel = null;

  private Camera playerCamera;
  private GameObject showingLootObject;
  private GameObject showingConstructObject;

  private void Awake()
  {
    lootPopup.gameObject.SetActive(false);
    unitTooltipPopup.gameObject.SetActive(false);
    constructPanel.gameObject.SetActive(false);
    buildingTooltipPopup.gameObject.SetActive(false);

    playerCamera = Camera.main;
  }

  //////////////////////////////////////////////////////////////////////////////////////
  // PREPARATION PHASE
  //////////////////////////////////////////////////////////////////////////////////////
  public void PreparationPhaseSelectNodeUI()
  {
    bottomUIPanel.SetActive(false);
    preparationPhaseUI.SelectNodeUI();
  }

  public void PreparationPhaseSelectArmyUI(bool conquered)
  {
    bottomUIPanel.SetActive(true);
    preparationPhaseUI.SelectArmyUI(conquered);
  }

  public void PreparationPhaseSetSelectArmyButtonInteractable(bool interactable)
  {
    preparationPhaseUI.SetSelectArmyButtonInteractable(interactable);
  }

  public void PreparationPhaseSetAttackButtonInteractable(bool interactable)
  {
    preparationPhaseUI.SetAttackButtonInteractable(interactable);
  }

  public void PreparationPhaseDisableUI()
  {
    preparationPhaseUI.DisableUI();
  }

  //////////////////////////////////////////////////////////////////////////////////////
  // ESCORT PHASE
  //////////////////////////////////////////////////////////////////////////////////////
  public void EscortPhaseRemoveAllUnits()
  {
    unitManager.RemoveAllUnits();
  }

  //////////////////////////////////////////////////////////////////////////////////////
  // PREPARATION DEFENSE PHASE
  //////////////////////////////////////////////////////////////////////////////////////
  public void PreparationDefensePhaseSelectArmyUI()
  {
    preparationPhaseUI.DefenseSelectArmyUI();
  }

  public void PreparationDefensePhaseSetDefendButtonInteractable(bool interactable)
  {
    preparationPhaseUI.SetDefendButtonInteractable(interactable);
  }

  public void PreparationDefensePhaseDisableUI()
  {
    preparationPhaseUI.DefenseDisableUI();
  }

  //////////////////////////////////////////////////////////////////////////////////////
  // OTHERS
  //////////////////////////////////////////////////////////////////////////////////////
  public void ShowLootPopup(int gold, int crystal, bool buildingSlot, bool upgradeRewarded, UPGRADE_TYPE upgradeReward, bool conquered, GameObject lootObject)
  {
    showingLootObject = lootObject;

    lootPopup.SetText(gold, crystal, buildingSlot, upgradeRewarded, upgradeReward, conquered);

    lootPopup.gameObject.SetActive(true);
  }

  public void HideLootPopup(GameObject lootObject)
  {
    if (lootObject == showingLootObject)
    {
      lootPopup.gameObject.SetActive(false);
    }
  }

  public void UpdateLootTargetPanel(int gold, int crystal, bool buildingSlot, bool upgradeRewarded, UPGRADE_TYPE upgradeReward)
  {
    lootTargetPanel.SetText(gold, crystal, buildingSlot, upgradeRewarded, upgradeReward);
  }

  public void ShowUnitTooltipPopup(string unitName, int cost, float health, float damage, float attackSpeed, string description, string constructMessage)
  {
    unitTooltipPopup.gameObject.SetActive(true);
    unitTooltipPopup.SetText(unitName, cost, health, damage, attackSpeed, description, constructMessage);
  }

  public void HideUnitTooltipPopup()
  {
    unitTooltipPopup.gameObject.SetActive(false);
  }

  public void ShowBuildingTooltipPopup(string buildingName, int cost, string description, string constructMessage)
  {
    buildingTooltipPopup.gameObject.SetActive(true);
    buildingTooltipPopup.SetText(buildingName, cost, description, constructMessage);
  }

  public void HideBuildingTooltipPopup()
  {
    buildingTooltipPopup.gameObject.SetActive(false);
  }

  public void ShowConstructPanel(GameObject node)
  {
    showingConstructObject = node;
    constructPanel.gameObject.SetActive(true);
    constructPanel.connectedNode = node;
  }

  public void HideConstructPanel(GameObject node)
  {
    if (node == showingConstructObject)
    {
      constructPanel.gameObject.SetActive(false);
    }
  }

  public void ShowUpgradeTooltipPopup(string unitName, int unitCost, int cost, float health, float damage, float attackSpeed, string description, string constructMessage)
  {
    upgradeTooltipPopup.gameObject.SetActive(true);
    upgradeTooltipPopup.SetText(unitName, unitCost, cost, health, damage, attackSpeed, description, constructMessage);
  }

  public void HideUpgradeTooltipPopup()
  {
    upgradeTooltipPopup.gameObject.SetActive(false);
  }

  public void UpdateUINodeColours()
  {
    nodeUIPanel.UpdateUINodesColours();
  }
}
