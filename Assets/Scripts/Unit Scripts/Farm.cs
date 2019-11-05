using UnityEngine;

public class Farm : MonoBehaviour
{
  [System.Serializable]
  public struct FarmUpgradeProperties
  {
    public int foodProvided;
    public int cost;
  }

  public FarmUpgradeProperties[] farmUpgradeProperties;

  [HideInInspector]
  public int farmLevel = 1;

  private int maxLevels = 3;

  [SerializeField]
  private GameObject upgradePanelPrefab = null;

  private GameObject upgradePanel = null;
  private UIInterface uiInterface;

  private GameManager gameManager;
  private Camera playerCamera;

  private void Awake()
  {
    uiInterface = FindObjectOfType<UIInterface>();

    upgradePanel = Instantiate(upgradePanelPrefab, uiInterface.transform, false);

    // + 1 to includes the current upgrade level
    maxLevels = farmUpgradeProperties.Length;

    gameManager = FindObjectOfType<GameManager>();
    playerCamera = Camera.main;
  }

  private void Start()
  {
    upgradePanel.GetComponent<FarmUpgradePanel>().SetUpgradingFarm(this);
  }

  private void OnEnable()
  {
    GameManager.resourceManager.FarmClaimed(this, farmLevel);
  }

  private void Update()
  {
    if (upgradePanel == null)
    {
      return;
    }

    // Check if this object is selected
    if (playerCamera.GetComponent<CameraObjectSelection>().SelectedUnitsList.Contains(gameObject) && (gameManager.CurrentPhase == PHASES.PREPARATION || gameManager.CurrentPhase == PHASES.PREPARATION_DEFENSE))
    {
      upgradePanel.SetActive(true);
    }

    else
    {
      upgradePanel.SetActive(false);
    }
  }

  public bool UpgradeFarm()
  {
    GameManager.resourceManager.FarmLost(this, farmLevel);

    ++farmLevel;

    GameManager.resourceManager.FarmClaimed(this, farmLevel);

    GameManager.uiSoundEmitter.PlayConstructSound();

    if (farmLevel == maxLevels)
    {
      return false;
    }

    else return true;
  }
}