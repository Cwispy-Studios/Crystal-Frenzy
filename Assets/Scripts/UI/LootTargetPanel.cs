using UnityEngine;
using UnityEngine.UI;

public class LootTargetPanel : MonoBehaviour
{
  [SerializeField]
  private Text goldText = null, crystalText = null;
  [SerializeField]
  private Text[] additionalRewardsText = null;
  [HideInInspector]
  // Cache for Loot Popup
  public int cacheGold = 0, cacheCrystalIncome = 0;
  [HideInInspector]
  public bool isBuildingSlotRewarded = false, isUpgradeRewarded = false;
  [HideInInspector]
  public UPGRADE_TYPE upgradeRewarded;

  private void Awake()
  {
    SetText(0, 0, false, false, UPGRADE_TYPE.LAST);
  }

  public void SetText(int gold, int crystal, bool buildingSlot, bool upgradeIsRewarded, UPGRADE_TYPE upgradeReward)
  {
    cacheGold = gold;
    cacheCrystalIncome = crystal;
    isBuildingSlotRewarded = buildingSlot;
    isUpgradeRewarded = upgradeIsRewarded;
    upgradeRewarded = upgradeReward;

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

    int numAddRewards = 0;

    // Reset additional rewards texts
    for (int i = 0; i < additionalRewardsText.Length; ++i)
    {
      additionalRewardsText[i].text = "";
    }

    if (isBuildingSlotRewarded == true)
    {
      additionalRewardsText[numAddRewards].text += "<color=lightblue>Building Slot</color>";
      ++numAddRewards;
    }

    if (isUpgradeRewarded == true)
    {
      additionalRewardsText[numAddRewards].text = "<color=magenta>" + upgradeRewarded.ToString().Replace("_", " ") + " UPGRADE</color>";
      ++numAddRewards;
    }
  }
}
