﻿using UnityEngine;

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

  private Camera playerCamera;
  private GameObject showingLootObject;

  private void Awake()
  {
    lootPopup.gameObject.SetActive(false);
    unitTooltipPopup.gameObject.SetActive(false);

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
  public void ShowLootPopup(int gold, int crystal, GameObject lootObject)
  {
    showingLootObject = lootObject;

    lootPopup.SetText(gold, crystal);

    lootPopup.gameObject.SetActive(true);
  }

  public void HideLootPopup(GameObject lootObject)
  {
    if (lootObject == showingLootObject)
    {
      lootPopup.gameObject.SetActive(false);
    }
  }

  public void UpdateLootTargetPanel(int gold, int crystal)
  {
    lootTargetPanel.SetText(gold, crystal);
  }

  public void ShowUnitTooltipPopup(string name, int cost, int health, int damage, float attackSpeed, string description, string constructMessage)
  {
    unitTooltipPopup.gameObject.SetActive(true);
    unitTooltipPopup.SetText(name, cost, health, damage, attackSpeed, description, constructMessage);
  }

  public void HideUnitTooltipPopup()
  {
    unitTooltipPopup.gameObject.SetActive(false);
  }


}
