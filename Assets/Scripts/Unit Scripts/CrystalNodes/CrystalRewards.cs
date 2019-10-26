using System.Collections.Generic;
using UnityEngine;

public class CrystalRewards : MonoBehaviour
{
  [SerializeField]
  private int goldLoot = 50;
  public int GoldLoot
  {
    get
    {
      return goldLoot;
    }
  }

  [SerializeField]
  private int crystalIncomeReward = 0;
  public int CrystalIncomeReward
  {
    get
    {
      return crystalIncomeReward;
    }
  }

  [SerializeField]
  private int nodeNumber = 1;

  /**************************************************** RANDOM REWARD VARIABLES ****************************************************/
  [SerializeField]
  private bool randomised = true;

  // The weight of the rewards. This affects the chance of getting additional rewards, and affects the amount of resources gained
  private const float MIN_WEIGHT = 0.65f;
  private const float MAX_WEIGHT = 2f;
  private const float WEIGHT_RANGE = MAX_WEIGHT - MIN_WEIGHT;

  private float lootWeight = 1f;

  // The margin from min and max weight before upgrades have a chance to be included as a reward
  private const float MIN_REWARD_MARGIN = 0.25f;
  private const float MAX_REWARD_MARGIN = 0.15f;
  private const float MIN_REWARD_WEIGHT = MIN_WEIGHT + MIN_REWARD_MARGIN;
  private const float MAX_REWARD_WEIGHT = MAX_WEIGHT - MAX_REWARD_MARGIN;
  private const float REWARD_WEIGHT_RANGE = MAX_REWARD_WEIGHT - MIN_REWARD_WEIGHT;
  public bool RewardsUpgrade { get; private set; } = false;
  public UPGRADE_TYPE RewardedUpgrade { get; private set; }


  /**************************************************** BUILDING SLOT VARIABLES ****************************************************/
  private const float BUILDING_SLOT_CHANCE = 0.45f;
  public bool BuildingSlotRewarded { get; private set; } = false;

  /**************************************************** RESOURCES VARIABLES ****************************************************/
  private const float GOLD_PER_POINTONE_LOOT_WEIGHT = 25f;
  private const float CRYSTAL_PER_POINTONE_LOOT_WEIGHT = 3f;
  private const float GOLD_INCREASE_PER_NODE = 7f;
  private const float CRYSTAL_INCREASE_PER_NODE = 0.8f;

  // Makes units spawn faster
  private float waveSpawnerDifficultyMultiplier = 1f;

  // Popup variables
  private const float POPUP_HOVER_TIME = 0.5f;
  private float popupHoverLength = 0;

  private UIInterface uiInterface;

  private void Awake()
  {
    uiInterface = FindObjectOfType<UIInterface>();
  }

  private void Start()
  {
    if (randomised)
    {
      //for (int i = 0; i < 10; ++i)
      //{
      //  lootWeight = RandomFromDistribution.RandomRangeLinear(MIN_WEIGHT, MAX_WEIGHT, -2f);
      //  //lootWeight = RandomFromDistribution.RandomRangeExponential(MIN_WEIGHT, MAX_WEIGHT, 0.5f, RandomFromDistribution.Direction_e.Left);

      //  /************************** Randomise if it rewards an upgrade **************************/
      //  if (lootWeight >= MIN_REWARD_WEIGHT)
      //  {
      //    RandomiseUpgradeReward();
      //  }

      //  /************************** Randomise if node has building slot **************************/
      //  if (Random.Range(0, 1f) <= BUILDING_SLOT_CHANCE)
      //  {
      //    Debug.Log("Building rewarded");
      //    GetComponent<BuildingSlot>().enabled = true;
      //    buildingSlotRewarded = true;
      //  }

      //  else
      //  {
      //    GetComponent<BuildingSlot>().enabled = false;
      //    buildingSlotRewarded = false;
      //  }

      //  Debug.Log("----------------------------------------");
      //}

      lootWeight = RandomFromDistribution.RandomRangeLinear(MIN_WEIGHT, MAX_WEIGHT, -2f);
      //lootWeight = RandomFromDistribution.RandomRangeExponential(MIN_WEIGHT, MAX_WEIGHT, 0.5f, RandomFromDistribution.Direction_e.Left);

      waveSpawnerDifficultyMultiplier = lootWeight;

      /************************** Randomise if it rewards an upgrade **************************/
      if (lootWeight >= MIN_REWARD_WEIGHT)
      {
        RandomiseUpgradeReward();
      }

      /************************** Randomise if node has building slot **************************/
      // Building slot not affected by loot weight
      if (Random.Range(0, 1f) <= BUILDING_SLOT_CHANCE)
      {
        GetComponent<BuildingSlot>().enabled = true;
        BuildingSlotRewarded = true;
        waveSpawnerDifficultyMultiplier += 0.1f;
        lootWeight -= 0.2f;
      }

      else
      {
        GetComponent<BuildingSlot>().enabled = false;
        BuildingSlotRewarded = false;
      }

      // Randomise the resource count
      float goldRewardWeight = Random.Range(0, lootWeight);
      float crystalIncomeRewardWeight = lootWeight - goldRewardWeight;

      goldLoot = Mathf.CeilToInt( (GOLD_PER_POINTONE_LOOT_WEIGHT + (nodeNumber * GOLD_INCREASE_PER_NODE)) * goldRewardWeight * 10f);
      crystalIncomeReward = Mathf.CeilToInt( (CRYSTAL_PER_POINTONE_LOOT_WEIGHT + (nodeNumber * CRYSTAL_INCREASE_PER_NODE)) * crystalIncomeRewardWeight * 10f);

      GetComponent<CrystalNode>().AdjustSpawners(waveSpawnerDifficultyMultiplier);
    }
  }

  private void Update()
  {
    // Check if mouse is hovering over object
    if (CameraObjectSelection.MouseHoverUnitsList.Contains(gameObject))
    {
      popupHoverLength += Time.deltaTime;
    }

    else
    {
      popupHoverLength = 0;
    }

    if (popupHoverLength >= POPUP_HOVER_TIME)
    {
      uiInterface.ShowLootPopup(goldLoot, crystalIncomeReward, BuildingSlotRewarded, RewardsUpgrade, RewardedUpgrade, GetComponent<ConqueredNode>().conquered, gameObject);
    }

    else
    {
      uiInterface.HideLootPopup(gameObject);
    }
  }

  private void RandomiseUpgradeReward()
  {
    int cheapestUpgradeType = 999999;
    int mostExpensiveUpgradeType = 0;
    float weightPerGoldCost = 0.001f;
    List<UpgradeTypeAveragePrice> averagePricesPerUpgradeType = GetCostOfUpgradableUpgrades(ref cheapestUpgradeType, ref mostExpensiveUpgradeType, ref weightPerGoldCost);

    // Remove the upgrades that do not meet the loot weight requirement (price is too high for a low loot weight)
    for (int i = averagePricesPerUpgradeType.Count - 1; i >= 0; --i)
    {
      float requiredLootWeight = ((averagePricesPerUpgradeType[i].averagePrice - cheapestUpgradeType) * weightPerGoldCost) + MIN_REWARD_WEIGHT;

      if (lootWeight < requiredLootWeight)
      {
        averagePricesPerUpgradeType.RemoveAt(i);
      }
    }

    int[] cumulativeSumOfWeights = new int[averagePricesPerUpgradeType.Count + 1];
    int cumulatedWeight = 0;

    // Stores all the prices as cumulated sum of weights. Since lower prices should have a higher chance
    // and highest prices should 
    for (int i = 0; i < averagePricesPerUpgradeType.Count; ++i)
    {
      cumulatedWeight += mostExpensiveUpgradeType - (averagePricesPerUpgradeType[i].averagePrice - cheapestUpgradeType);
      cumulativeSumOfWeights[i] = cumulatedWeight;
    }

    // The last number is the chance that no upgrade is rewarded. This is based on the loot weight you get, multiplied by the most expensive upgrade
    // multiplied by another number to weigh it more or less towards not getting an upgrade. (Having enough loot weight to get here shouldn't be too hard
    // but actually being able to get an upgrade should be harder)
    cumulatedWeight += Mathf.FloorToInt((MAX_WEIGHT - lootWeight) * mostExpensiveUpgradeType * 3f);
    cumulativeSumOfWeights[cumulativeSumOfWeights.Length - 1] = cumulatedWeight;

    // Randomise a number which determines which upgrade to reward
    int randomNumber = Random.Range(0, cumulatedWeight);

    // Find the largest cumulative sum that is smaller than this random number, go from the lowest to highest
    for (int i = 0; i < averagePricesPerUpgradeType.Count; ++i)
    {
      if (cumulativeSumOfWeights[i] > randomNumber)
      {
        waveSpawnerDifficultyMultiplier += 0.2f;
        // Makes the resource rewards lesser
        lootWeight -= 0.55f;
        RewardedUpgrade = averagePricesPerUpgradeType[i].upgradeType;
        RewardsUpgrade = true;
        Debug.Log("Rewarded upgrade: " + RewardedUpgrade);
        break;
      }
    }
  }

  /******************************************************************************/
  /*!
      Gets the average prices of every upgrade type left to upgrade, 
      finds the cheapest and most expensive upgrade types, and stores these prices
  */
  /******************************************************************************/
  private List<UpgradeTypeAveragePrice> GetCostOfUpgradableUpgrades(ref int cheapestUpgradeType, ref int mostExpensiveUpgradeType, ref float weightPerGoldCost)
  {
    cheapestUpgradeType = 99999;
    mostExpensiveUpgradeType = 0;
    weightPerGoldCost = 0;

    List<UpgradeTypeAveragePrice> averagePricesPerUpgradeType = GameManager.upgradeManager.GetListOfAverageUpgradablePrices();

    for (int i = 0; i < averagePricesPerUpgradeType.Count; ++i)
    {
      int cost = averagePricesPerUpgradeType[i].averagePrice;

      //Debug.Log(averagePricesPerUpgradeType[i].upgradeType + " average cost: " + cost);

      if (cost < cheapestUpgradeType)
      {
        cheapestUpgradeType = cost;
      }

      if (cost > mostExpensiveUpgradeType)
      {
        mostExpensiveUpgradeType = cost;
      }
    }

    // Find the increase in loot weight needed for each increase in gold cost for the upgrade
    // For example, if cheapest upgrade type is 50, and the weight per gold cost is found to be 0.00175,
    // an upgrade type of average cost 50 would require loot weight of at least MIN_WEIGHT (0.8)
    // an upgrade type of average cost 55 would require loot weight of at least MIN_WEIGHT + (cost - cheapest) * weight per gold cost
    int costRange = mostExpensiveUpgradeType - cheapestUpgradeType;
    weightPerGoldCost = REWARD_WEIGHT_RANGE / costRange;

    return averagePricesPerUpgradeType;
  }

  public void CollectLoot(bool conquered)
  {
    GameManager.resourceManager.CollectLoot(GoldLoot, CrystalIncomeReward, conquered);
    
    // Collect upgrade
    if (RewardsUpgrade)
    {
      GameManager.upgradeManager.UpgradeReward(RewardedUpgrade);
    }
  }
}
