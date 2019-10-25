﻿using UnityEngine;

public class UpgradeBuilding : MonoBehaviour
{
  [SerializeField]
  private GameObject upgradePanelPrefab = null;

  private UIInterface uiInterface;
  private GameObject upgradePanel;

  private void Awake()
  {
    uiInterface = FindObjectOfType<UIInterface>();
    upgradePanel = Instantiate(upgradePanelPrefab, uiInterface.transform, false);

    upgradePanel.SetActive(false);
  }

  private void Update()
  {
    // Check if this object is selected
    if (CameraObjectSelection.SelectedUnitsList.Contains(gameObject) && (GameManager.CurrentPhase == PHASES.PREPARATION || GameManager.CurrentPhase == PHASES.PREPARATION_DEFENSE))
    {
      upgradePanel.SetActive(true);
    }

    else
    {
      upgradePanel.SetActive(false);
    }
  }
}
