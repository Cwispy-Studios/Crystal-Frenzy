using UnityEngine;

public class UpgradeBuilding : MonoBehaviour
{
  [SerializeField]
  private GameObject upgradePanelPrefab = null;
  [SerializeField]
  private bool instantiate = true;

  private UIInterface uiInterface;
  private GameObject upgradePanel;

  private void Start()
  {
    uiInterface = FindObjectOfType<UIInterface>();

    if (instantiate)
    {
      upgradePanel = Instantiate(upgradePanelPrefab, uiInterface.transform, false);
    }
    
    else
    {
      upgradePanel = upgradePanelPrefab;
    }

    upgradePanel.SetActive(false);
  }

  private void Update()
  {
    // Check if this object is selected and belongs to player
    if (CameraObjectSelection.SelectedUnitsList.Contains(gameObject) && GetComponent<Faction>().faction == Faction.FACTIONS.GOBLINS &&
      (GameManager.CurrentPhase == PHASES.PREPARATION || GameManager.CurrentPhase == PHASES.PREPARATION_DEFENSE))
    {
      upgradePanel.SetActive(true);
      upgradePanel.GetComponent<UpgradePanel>().building = gameObject;
    }

    else if (upgradePanel.GetComponent<UpgradePanel>().building == gameObject)
    {
      upgradePanel.SetActive(false);
    }
  }
}
