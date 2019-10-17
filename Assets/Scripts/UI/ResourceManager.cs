using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
  [SerializeField]
  private int startingGold = 100, startingCrystals = 0, crystalsIncome = 0, startingUnitCap = 10;
  [SerializeField]
  private Text goldText = null, crystalText = null, UnitCapText = null;

  private int gold, crystals, units;

  private void Awake()
  {
    gold = startingGold;
    crystals = startingCrystals;
    units = 0;
  }

  private void LateUpdate()
  {
    goldText.text = gold.ToString();
    crystalText.text = crystals.ToString();
    UnitCapText.text = units.ToString() + " / " + startingUnitCap;
  }
}
