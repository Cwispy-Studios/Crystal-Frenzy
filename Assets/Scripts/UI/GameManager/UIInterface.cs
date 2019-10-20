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
  //////////////////////////////////////////////////////////////////////////////////////
  // PREPARATION PHASE
  //////////////////////////////////////////////////////////////////////////////////////
  public void PreparationPhaseSelectNodeUI()
  {
    bottomUIPanel.SetActive(false);
    preparationPhaseUI.SelectNodeUI();
  }

  public void PreparationPhaseSelectArmyUI()
  {
    bottomUIPanel.SetActive(true);
    preparationPhaseUI.SelectArmyUI();
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
}