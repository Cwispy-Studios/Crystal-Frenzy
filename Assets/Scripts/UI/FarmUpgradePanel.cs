using UnityEngine;

public class FarmUpgradePanel : MonoBehaviour
{
  [SerializeField]
  private GameObject upgradeButton = null;

  private UIInterface uiInterface = null;

  private Farm upgradingFarm = null;

  private void Awake()
  {
    uiInterface = FindObjectOfType<UIInterface>();
  }

  private void Update()
  {
    // Building has been destroyed
    if (upgradeButton == null)
    {
      Destroy(gameObject);
    }
  }

  public void SetUpgradingFarm(Farm farm)
  {
    upgradingFarm = farm;
    upgradeButton.GetComponent<FarmUpgradeButton>().upgradingFarm = farm;
  }
}
