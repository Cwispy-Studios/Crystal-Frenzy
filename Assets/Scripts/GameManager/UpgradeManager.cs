using System.Collections.Generic;
using UnityEngine;

public struct UpgradeTypeAveragePrice
{
  public UPGRADE_TYPE upgradeType;
  public int averagePrice;
}

// Holds all the upgrades buttons and logic for upgrading
public class UpgradeManager : MonoBehaviour
{
  public class UpgradeLevels
  {
    public bool available;

    public int maxLevel;
    public int currentUpgradeLevel;
    public Dictionary<int, GameObject> upgradeLevelsButtons;
    public List<UPGRADE_TYPE> upgradesTo;
    public List<UPGRADE_TYPE> exclusiveFrom;
  }

  // Prefabs for the upgrade buttons. These buttons also hold the upgrade properties.
  [SerializeField]
  private GameObject[]
    minerHealthUpgradePrefabs = null,
    minerSpeedUpgradePrefabs = null,
    stabbyUpgradePrefabs = null,
    shootyUpgradePrefabs = null,
    bruteUpgradePrefabs = null,
    warlockUpgradePrefabs = null,
    stabbyCheapUpgrade = null,
    stabbyStrongUpgrade = null,
    shootyNukeUpgrade = null,
    shootyFastUpgrade = null,
    bruteCheapUpgrade = null,
    bruteTankUpgrade = null,
    crystalMiningUpgrade = null;

  // The LH key holds the upgrade type. The value of this key holds all the data of the upgrade type
  // UpgradeLevels holds the max upgradable level, the current level of upgrade and all the upgrade buttons for the upgrade type
  public Dictionary<UPGRADE_TYPE, UpgradeLevels> upgrades;

  private void Awake()
  {
    // Units do not possess any upgrade levels, they simply hold the enums to the upgrades that would affect them.
    // When units are instantiated, they simply check the corresponding subscribed upgrades what level they are at
    // and retrieve those values to add on to their existing attributes. This also has to be done with the army
    // recruitment panel when viewing their stats.
    upgrades = new Dictionary<UPGRADE_TYPE, UpgradeLevels>();

    InitialiseUpgrades(minerHealthUpgradePrefabs, true, new List<UPGRADE_TYPE>() , new List<UPGRADE_TYPE>());
    InitialiseUpgrades(minerSpeedUpgradePrefabs, true, new List<UPGRADE_TYPE>(), new List<UPGRADE_TYPE>());

    InitialiseUpgrades(stabbyUpgradePrefabs, true, new List<UPGRADE_TYPE> { UPGRADE_TYPE.STABBY_CHEAP, UPGRADE_TYPE.STABBY_BEEFED }, new List<UPGRADE_TYPE>());
    InitialiseUpgrades(shootyUpgradePrefabs, true, new List<UPGRADE_TYPE> { UPGRADE_TYPE.SHOOTY_FAST, UPGRADE_TYPE.SHOOTY_NUKE }, new List<UPGRADE_TYPE>());
    InitialiseUpgrades(bruteUpgradePrefabs, true, new List<UPGRADE_TYPE> { UPGRADE_TYPE.BRUTE_CHEAP, UPGRADE_TYPE.BRUTE_TANK }, new List<UPGRADE_TYPE>());
    InitialiseUpgrades(warlockUpgradePrefabs, true, new List<UPGRADE_TYPE>(), new List<UPGRADE_TYPE>());

    InitialiseUpgrades(stabbyCheapUpgrade, false, new List<UPGRADE_TYPE>(), new List<UPGRADE_TYPE> { UPGRADE_TYPE.STABBY_BEEFED });
    InitialiseUpgrades(stabbyStrongUpgrade, false, new List<UPGRADE_TYPE>(), new List<UPGRADE_TYPE> { UPGRADE_TYPE.STABBY_CHEAP });

    InitialiseUpgrades(shootyFastUpgrade, false, new List<UPGRADE_TYPE>(), new List<UPGRADE_TYPE> { UPGRADE_TYPE.SHOOTY_NUKE });
    InitialiseUpgrades(shootyNukeUpgrade, false, new List<UPGRADE_TYPE>(), new List<UPGRADE_TYPE> { UPGRADE_TYPE.SHOOTY_FAST });

    InitialiseUpgrades(bruteCheapUpgrade, false, new List<UPGRADE_TYPE>(), new List<UPGRADE_TYPE> { UPGRADE_TYPE.BRUTE_TANK });
    InitialiseUpgrades(bruteTankUpgrade, false, new List<UPGRADE_TYPE>(), new List<UPGRADE_TYPE> { UPGRADE_TYPE.BRUTE_CHEAP });

    InitialiseUpgrades(crystalMiningUpgrade, true, new List<UPGRADE_TYPE>(), new List<UPGRADE_TYPE>());
  }

  private void InitialiseUpgrades(GameObject[] upgradePrefabs, bool startsAvailable, List<UPGRADE_TYPE> furtherUpgrades, List<UPGRADE_TYPE> exclusive)
  {
    UPGRADE_TYPE upgradeType = upgradePrefabs[0].GetComponent<UpgradeButton>().upgradeType;
    
    // The upgrade levels with the corresponding upgrade buttons which also contains the upgrade properties
    Dictionary<int, GameObject> upgradeButtons = new Dictionary<int, GameObject>();

    // Upgrade level 0 means it is unupgraded (no button). When checking for upgrade levels and properties, null will mean upgrade properties values are all 0
    for (int i = 0; i <= upgradePrefabs.Length; ++i)
    {
      if (i == 0)
      {
        upgradeButtons.Add(i, null);
      }

      else
      {
        upgradeButtons.Add(i, upgradePrefabs[i - 1]);
      }
    }

    UpgradeLevels upgradeLevels = new UpgradeLevels { maxLevel = upgradePrefabs.Length, currentUpgradeLevel = 0, upgradeLevelsButtons = upgradeButtons,
      available = startsAvailable, upgradesTo = furtherUpgrades, exclusiveFrom = exclusive };

    upgrades.Add(upgradeType, upgradeLevels);
  }

  public UpgradeProperties[] RetrieveUpgradeProperties(UPGRADE_TYPE upgradeType)
  {
    int upgradeLevel = upgrades[upgradeType].currentUpgradeLevel;

    if (upgradeLevel == 0)
    {
      return null;
    }

    else
    {
      UpgradeProperties[] cumulativeUpgrades = new UpgradeProperties[upgradeLevel];

      for (int i = 1; i <= upgradeLevel; ++i)
      {
        cumulativeUpgrades[i - 1] = upgrades[upgradeType].upgradeLevelsButtons[upgradeLevel].GetComponent<UpgradeButton>().upgradeProperties;
      }

      return cumulativeUpgrades;
    }
  }

  public UpgradeProperties RetrieveCurrentUpgradeProperties(UPGRADE_TYPE upgradeType)
  {
    int upgradeLevel = upgrades[upgradeType].currentUpgradeLevel;

    return upgrades[upgradeType].upgradeLevelsButtons[upgradeLevel + 1].GetComponent<UpgradeButton>().upgradeProperties;
  }

  public GameObject UpgradeButton(UPGRADE_TYPE upgradeType)
  {
    ++upgrades[upgradeType].currentUpgradeLevel;

    SetExclusiveUnavailable(upgradeType);

    // Check if the max level has been reached
    if (upgrades[upgradeType].currentUpgradeLevel == upgrades[upgradeType].maxLevel)
    {
      // No more upgrade levels so the button will simply be removed
      // Check if there are any upgrades to and set those as available
      CheckUpgradeToReady(upgradeType);

      return null;
    }

    else
    {
      // Instantiate and return the upgrade button of the next level
      return Instantiate(upgrades[upgradeType].upgradeLevelsButtons[upgrades[upgradeType].currentUpgradeLevel + 1]);
    }
  }

  public int GetNextUpgradeLevel(UPGRADE_TYPE upgradeType)
  {
    return upgrades[upgradeType].currentUpgradeLevel + 1;
  }

  public GameObject CheckUpgradeLevel(UPGRADE_TYPE upgradeType, int upgradeLevel)
  {
      int currentUpgradeLevel = upgrades[upgradeType].currentUpgradeLevel + 1;

    // If levels are unequal, return the new button that should replace the old one
    if (upgradeLevel != currentUpgradeLevel)
    {
      return Instantiate(upgrades[upgradeType].upgradeLevelsButtons[currentUpgradeLevel]);
    }

    // Return null if nothing changed
    else return null;
  }

  public List<UpgradeTypeAveragePrice> GetListOfAverageUpgradablePrices()
  {
    List<UpgradeTypeAveragePrice> priceList = new List<UpgradeTypeAveragePrice>();
    for (int upgradeType = 0; upgradeType < (int)UPGRADE_TYPE.LAST; ++upgradeType)
    {
      UpgradeLevels upgradeLevels = upgrades[(UPGRADE_TYPE)upgradeType];
      int averageUpgradeCost = 0;

      if (upgradeLevels.available && upgradeLevels.currentUpgradeLevel != upgradeLevels.maxLevel)
      {
        for (int i = upgradeLevels.currentUpgradeLevel; i < upgradeLevels.maxLevel; ++i)
        {
          averageUpgradeCost += upgradeLevels.upgradeLevelsButtons[i + 1].GetComponent<UpgradeButton>().cost;
        }

        averageUpgradeCost /= (upgradeLevels.maxLevel - upgradeLevels.currentUpgradeLevel);

        UpgradeTypeAveragePrice upgradeTypeAveragePrice = new UpgradeTypeAveragePrice { upgradeType = (UPGRADE_TYPE)upgradeType, averagePrice = averageUpgradeCost };
        priceList.Add(upgradeTypeAveragePrice);
      }
    }

    return priceList;
  }

  public void UpgradeReward(UPGRADE_TYPE upgradeType)
  {
    // Does nothing if at max level, means player wasted their upgrade
    if (upgrades[upgradeType].currentUpgradeLevel != upgrades[upgradeType].maxLevel)
    {
      // Create a new struct to update the upgrade level
      UpgradeLevels newUpgradeLevel = upgrades[upgradeType];

      ++newUpgradeLevel.currentUpgradeLevel;

      // The button will check every fixedupdate if it's level is still the same as the one here.
      // Once it detects it is different, it will automatically change
      upgrades[upgradeType] = newUpgradeLevel;

      SetExclusiveUnavailable(upgradeType);
    }

    else
    {
      CheckUpgradeToReady(upgradeType);
    }
  }

  public bool UpgradeIsAvailable(UPGRADE_TYPE upgradeType)
  {
    return upgrades[upgradeType].available;
  }

  private void CheckUpgradeToReady(UPGRADE_TYPE upgradeType)
  {
    if (upgrades[upgradeType].upgradesTo.Count > 0)
    {
      for (int i = 0; i < upgrades[upgradeType].upgradesTo.Count; ++i)
      {
        UPGRADE_TYPE upgradeToType = upgrades[upgradeType].upgradesTo[i];

        upgrades[upgradeToType].available = true;
      }
    }
  }

  private void SetExclusiveUnavailable(UPGRADE_TYPE upgradeType)
  {
    if (upgrades[upgradeType].exclusiveFrom.Count > 0)
    {
      for (int i = 0; i < upgrades[upgradeType].exclusiveFrom.Count; ++i)
      {
        UPGRADE_TYPE exclusiveType = upgrades[upgradeType].exclusiveFrom[i];

        upgrades[exclusiveType].available = false;
      }
    }
  }
}
