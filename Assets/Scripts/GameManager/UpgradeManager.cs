using System.Collections.Generic;
using UnityEngine;

// Holds all the upgrades buttons and logic for upgrading
public class UpgradeManager : MonoBehaviour
{
  public struct UpgradeLevels
  {
    public int maxLevel;
    public int currentUpgradeLevel;
    public Dictionary<int, GameObject> upgradeLevelsButtons;
  }

  // Prefabs for the upgrade buttons. These buttons also hold the upgrade properties.
  [SerializeField]
  private GameObject[]
    minerHealthUpgradePrefabs = null,
    minerSpeedUpgradePrefabs  = null;

  // The LH key holds the upgrade type. The value of this key is a tuple. 
  // The 1st value of the tuple is the upgraded level of that corresponding upgrade type
  // The 2nd value of the tuple holds another Dictionary, which holds all the upgrade buttons for every upgrade level of an upgrade type
  // The key of this Dictionary is the upgrade level and the value is the corresponding upgrade button
  public Dictionary<UPGRADE_TYPE, UpgradeLevels> upgrades;

  private void Awake()
  {
    // Units do not possess any upgrade levels, they simply hold the enums to the upgrades that would affect them.
    // When units are instantiated, they simply check the corresponding subscribed upgrades what level they are at
    // and retrieve those values to add on to their existing attributes. This also has to be done with the army
    // recruitment panel when viewing their stats.
    upgrades = new Dictionary<UPGRADE_TYPE, UpgradeLevels>();

    InitialiseUpgrades(minerHealthUpgradePrefabs);
    InitialiseUpgrades(minerSpeedUpgradePrefabs);
  }

  private void InitialiseUpgrades(GameObject[] upgradePrefabs)
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

    UpgradeLevels upgradeLevels = new UpgradeLevels { maxLevel = upgradePrefabs.Length, currentUpgradeLevel = 0, upgradeLevelsButtons = upgradeButtons };

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
    // Create a new struct to update the upgrade level
    UpgradeLevels newUpgradeLevel = upgrades[upgradeType];

    ++newUpgradeLevel.currentUpgradeLevel;

    upgrades[upgradeType] = newUpgradeLevel;

    // Check if the max level has been reached
    if (newUpgradeLevel.currentUpgradeLevel == newUpgradeLevel.maxLevel)
    {
      // No more upgrade levels so the button will simply be removed
      return null;
    }

    else
    {
      // Instantiate and return the upgrade button of the next level
      return Instantiate(upgrades[upgradeType].upgradeLevelsButtons[newUpgradeLevel.currentUpgradeLevel + 1]);
    }
  }

  public int GetNextUpgradeLevel(UPGRADE_TYPE upgradeType)
  {
    return upgrades[upgradeType].currentUpgradeLevel + 1;
  }
}
