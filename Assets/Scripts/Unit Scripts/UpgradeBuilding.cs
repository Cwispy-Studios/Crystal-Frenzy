using UnityEngine;

public class UpgradeBuilding : MonoBehaviour
{
  [SerializeField]
  private GameObject upgradePanelPrefab = null;
  [SerializeField]
  private bool instantiate = true;

  private UIInterface uiInterface;
  private GameObject upgradePanel;

  private GameManager gameManager;
  private Camera playerCamera;

  private void Start()
  {
    uiInterface = FindObjectOfType<UIInterface>();
    playerCamera = Camera.main;

    if (instantiate)
    {
      upgradePanel = Instantiate(upgradePanelPrefab, uiInterface.transform, false);
    }
    
    else
    {
      upgradePanel = upgradePanelPrefab;
    }

    upgradePanel.SetActive(false);

    gameManager = FindObjectOfType<GameManager>();
  }

  private void Update()
  {
    // Check if this object is selected and belongs to player
    if (playerCamera.GetComponent<CameraObjectSelection>().SelectedUnitsList.Contains(gameObject) && GetComponent<Faction>().faction == Faction.FACTIONS.GOBLINS &&
      (gameManager.CurrentPhase == PHASES.PREPARATION || gameManager.CurrentPhase == PHASES.PREPARATION_DEFENSE))
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
