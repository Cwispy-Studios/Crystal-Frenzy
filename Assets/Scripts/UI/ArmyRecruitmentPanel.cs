﻿using UnityEngine;
using UnityEngine.UI;

public class ArmyRecruitmentPanel : MonoBehaviour
{
  [SerializeField]
  private RecruitmentButton stabbyBoyButton = null, shootyBoyButton = null, bruteButton = null, warlockButton = null;

  private UIInterface uiInterface = null;

  private void Awake()
  {
    uiInterface = FindObjectOfType<UIInterface>();
  }

  private void FixedUpdate()
  {
    shootyBoyButton.available = shootyBoyButton.GetComponent<Button>().interactable =
      GameManager.buildingManager.archeryRangeConstructed && GameManager.buildingManager.archeryRangeInControl;
    bruteButton.available = bruteButton.GetComponent<Button>().interactable =
      GameManager.buildingManager.brawlPitConstructed && GameManager.buildingManager.blacksmithInControl;
    warlockButton.available = warlockButton.GetComponent<Button>().interactable =
      GameManager.buildingManager.mageTowerConstructed && GameManager.buildingManager.mageTowerInControl;
  }

  // Identifies the button and then sets the unit tooltip text accordingly
  public void SetText(RecruitmentButton button, GameObject recruitableUnit)
  {
    string unitName = "";
    string description = "";
    string constructMessage = "";

    if (button == stabbyBoyButton)
    {
      unitName = "Stabby Boy";
    }

    else if (button == shootyBoyButton)
    {
      unitName = "Shooty Boy";

      if (!GameManager.buildingManager.archeryRangeConstructed)
      {
        constructMessage = "CONSTRUCT ARCHERY RANGE";
      }
    }

    else if (button == bruteButton)
    {
      unitName = "Brute";

      if (!GameManager.buildingManager.brawlPitConstructed)
      {
        constructMessage = "CONSTRUCT BRAWL PIT";
      }
    }

    else if (button == warlockButton)
    {
      unitName = "Warlock";

      if (!GameManager.buildingManager.mageTowerConstructed)
      {
        constructMessage = "CONSTRUCT MAGE TOWER";
      }
    }

    else
    {
      Debug.LogError("Button type not found in ArmyRecruitmentPanel! Button name is " + button.name);
    }

    // Retrieve the upgraded properties
    int upgradedCost = recruitableUnit.GetComponent<RecruitableUnit>().unitPoints;

    float
      upgradedHealth = recruitableUnit.GetComponent<Health>().MaxHealth, 
      upgradedDamage = recruitableUnit.GetComponent<Attack>().AttackDamage,
      upgradedAttackSpeed = recruitableUnit.GetComponent<Attack>().AttacksPerSecond;

    UPGRADE_TYPE[] affectedByUpgrades = recruitableUnit.GetComponent<Upgradable>().affectedByUpgrades;

    for (int i = 0; i < affectedByUpgrades.Length; ++i)
    {
      // Retrieve the ugrade properties
      UpgradeProperties[] upgradeProperties = GameManager.upgradeManager.RetrieveUpgradeProperties(affectedByUpgrades[i]);

      if (upgradeProperties != null)
      {
        for (int up = 0; up < upgradeProperties.Length; ++up)
        {
          upgradedCost += upgradeProperties[up].cost;
          upgradedHealth += upgradeProperties[up].health;
          upgradedDamage += upgradeProperties[up].damage;
          upgradedAttackSpeed += upgradeProperties[up].attackSpeed;
        }
      }
    }

    uiInterface.ShowUnitTooltipPopup(unitName,
                                     upgradedCost,
                                     upgradedHealth,
                                     upgradedDamage,
                                     upgradedAttackSpeed,
                                     description,
                                     constructMessage);
  }
}
