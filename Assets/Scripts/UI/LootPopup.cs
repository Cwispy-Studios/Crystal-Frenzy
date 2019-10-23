using UnityEngine;
using UnityEngine.UI;

public class LootPopup : MonoBehaviour
{
  [SerializeField]
  private Text goldText = null, crystalText = null;
  [SerializeField]
  private LootTargetPanel lootTargetPanel = null;

  public void SetText(int gold, int crystal, bool collected)
  {
    // If loot not collected yet, we show the normal colours
    if (!collected)
    {
      string compareColour;

      // When comparing the different loots, if the loot we are looking at gives a lower value than the loot we have already chosen, 
      // the text becomes green. Otherwise it's red. If it is the same value, it is just white
      if (gold > lootTargetPanel.cacheGold)
      {
        compareColour = "<color=lime>";
      }

      else if (gold < lootTargetPanel.cacheGold)
      {
        compareColour = "<color=red>";
      }

      else
      {
        compareColour = "<color=white>";
      }

      goldText.text = compareColour + gold.ToString() + "</color> (" + lootTargetPanel.cacheGold + ")";

      if (crystal > lootTargetPanel.cacheCrystalIncome)
      {
        compareColour = "<color=lime>";
      }

      else if (crystal < lootTargetPanel.cacheCrystalIncome)
      {
        compareColour = "<color=red>";
      }

      else
      {
        compareColour = "<color=white>";
      }

      crystalText.text = compareColour + "+" + crystal.ToString() + "</color> (" + "+" + lootTargetPanel.cacheCrystalIncome + ")";
    }
    
    else
    {
      goldText.text = "<color=grey>" + gold.ToString() + "</color>";
      crystalText.text = "<color=grey>" + "+" + crystal.ToString() + "</color>";
    }
  }
}
