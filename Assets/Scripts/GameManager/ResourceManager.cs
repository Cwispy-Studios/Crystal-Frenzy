using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
  public int crystalsPerUnitPoint = 5;

  [SerializeField]
  private int startingGold = 100, startingCrystals = 0, crystalsIncome = 0, startingUnitCap = 10;
  [SerializeField]
  private Text goldText = null, crystalText = null, UnitCapText = null;

  public int Gold { get; private set; }
  public int Crystals { get; private set; }
  public int ArmySize { get; private set; }
  public int UnitCap { get; private set; }

  private void Awake()
  {
    Gold = startingGold;
    Crystals = startingCrystals;
    ArmySize = 0;
    UnitCap = startingUnitCap;
  }

  private void LateUpdate()
  {
    goldText.text = Gold.ToString();
    crystalText.text = Crystals.ToString() + " (+" + crystalsIncome.ToString() + ")";
    UnitCapText.text = ArmySize.ToString() + " / " + UnitCap;
  }

  public void UpdateArmySize(int value)
  {
    ArmySize += value;
  }

  public void SpendGold(int amount)
  {
    Gold -= amount;
  }

  public void CollectLoot(int gold, int crystalIncome, bool conquered)
  {
    // Only collect gold if node has not already been conquered before
    if (!conquered)
    {
      Gold += gold;
    }
    
    crystalsIncome += crystalIncome;
    Crystals += crystalsIncome;
  }

  public void LoseIncome(int crystalIncome)
  {
    crystalsIncome -= crystalIncome;
  }

  public void FarmClaimed(Farm farm, int farmLevel)
  {
    UnitCap += farm.farmUpgradeProperties[farmLevel - 1].foodProvided;
  }

  public void FarmLost(Farm farm, int farmLevel)
  {
    UnitCap -= farm.farmUpgradeProperties[farmLevel - 1].foodProvided;
  }

  public void UpgradeCost(UpgradeButton upgradeButton)
  {
    Crystals -= upgradeButton.cost;
  }
  
  public void GainCrystalManpower()
  {
    Crystals += crystalsPerUnitPoint * ArmySize;
  }
}
