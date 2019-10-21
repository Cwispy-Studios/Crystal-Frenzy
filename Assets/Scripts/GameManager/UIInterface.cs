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

  private Camera playerCamera;

  private void Awake()
  {
    lootPopup.gameObject.SetActive(false);

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

  //////////////////////////////////////////////////////////////////////////////////////
  // ESCORT PHASE
  //////////////////////////////////////////////////////////////////////////////////////
  public void EscortPhaseRemoveAllUnits()
  {
    unitManager.RemoveAllUnits();
  }

  public void ShowRewardPopup(int gold, int crystal)
  {
    lootPopup.SetText(gold, crystal);

    lootPopup.gameObject.SetActive(true);
  }

  public void HideRewardPopup()
  {
    lootPopup.gameObject.SetActive(false);
  }
}
