using UnityEngine;
using UnityEngine.UI;

public class LootPopup : MonoBehaviour
{
  [SerializeField]
  private Text goldText = null, crystalText = null;
  [SerializeField]
  private Text[] additionalRewardsText = null;
  [SerializeField]
  private LootTargetPanel lootTargetPanel = null;

  public void SetText(int gold, int crystal, bool buildingSlot, bool upgradeRewarded, UPGRADE_TYPE upgradeReward, bool collected)
  {
    // If loot not collected yet, we show the normal colours
    if (!collected)
    {
      string compareColour;

      /***************************************** GOLD TEXT *****************************************/

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

      /***************************************** CRYSTAL INCOME TEXT *****************************************/

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

      int numAddRewards = 0;

      // Reset additional rewards texts
      for (int i = 0; i < additionalRewardsText.Length; ++i)
      {
        additionalRewardsText[i].text = "";
      }

      /***************************************** BUILDING SLOT TEXT *****************************************/

      if (buildingSlot == true && lootTargetPanel.isBuildingSlotRewarded)
      {
        additionalRewardsText[numAddRewards].text += "<color=white>Building Slot</color>";
        ++numAddRewards;
      }

      else if (buildingSlot == true && !lootTargetPanel.isBuildingSlotRewarded)
      {
        additionalRewardsText[numAddRewards].text += "<color=lightblue>Building Slot</color>";
        ++numAddRewards;
      }

      else if (buildingSlot == false && lootTargetPanel.isBuildingSlotRewarded)
      {
        additionalRewardsText[numAddRewards].text = "<color=red>No Building Slot</color>";
        ++numAddRewards;
      }

      /***************************************** UPGRADE TEXT *****************************************/
      
      if (upgradeRewarded == true && lootTargetPanel.isUpgradeRewarded)
      {
        compareColour = "<color=white>";

        additionalRewardsText[numAddRewards].text = compareColour + upgradeReward.ToString().Replace("_", " ") + " (" + lootTargetPanel.upgradeRewarded.ToString().Replace("_", " ") + "</color>";
        ++numAddRewards;
      }
      
      else if (upgradeRewarded == true && !lootTargetPanel.isUpgradeRewarded)
      {
        compareColour = "<color=magenta>";

        additionalRewardsText[numAddRewards].text = compareColour + upgradeReward.ToString().Replace("_", " ") + "</color>";
        ++numAddRewards;
      }
      
      else if (upgradeRewarded == false && lootTargetPanel.isUpgradeRewarded)
      {
        additionalRewardsText[numAddRewards].text = "<color=red>NONE</color> (" + upgradeReward.ToString().Replace("_", " ") + ")";
      }
    }
    
    else
    {
      goldText.text = "<color=grey>" + gold.ToString() + "</color>";

      /***************************************** CRYSTAL INCOME TEXT *****************************************/

      string compareColour;

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
  }
}
