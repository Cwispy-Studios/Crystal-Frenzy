using UnityEngine;
using UnityEngine.UI;

public class LootTargetPanel : MonoBehaviour
{
  [SerializeField]
  private Text goldText = null, crystalText = null;
  [HideInInspector]
  // Cache for Loot Popup
  public int cacheGold = 0, cacheCrystalIncome = 0;

  public void SetText(int gold, int crystal)
  {
    cacheGold = gold;
    cacheCrystalIncome = crystal;

    if (cacheGold == 0)
    {
      goldText.text = "None";
    }

    else
    {
      goldText.text = cacheGold.ToString();
    }

    if (cacheCrystalIncome == 0)
    {
      crystalText.text = "None";
    }

    else
    {
      crystalText.text = "+" + cacheCrystalIncome.ToString();
    }
  }
}
