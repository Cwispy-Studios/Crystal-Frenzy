using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
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

  public void CollectLoot(int gold, int crystalIncome)
  {
    Gold += gold;
    crystalsIncome += crystalIncome;
    Crystals += crystalsIncome;
  }

  public void FarmClaimed(FoodProvider farm)
  {
    UnitCap += farm.foodProvided;
  }

  public void FarmLost(FoodProvider farm)
  {
    UnitCap -= farm.foodProvided;
  }
}
