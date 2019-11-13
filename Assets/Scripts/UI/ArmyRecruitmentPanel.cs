using UnityEngine;
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
    shootyBoyButton.available = GameManager.buildingManager.archeryRangeConstructed && GameManager.buildingManager.archeryRangeInControl;
    bruteButton.available = GameManager.buildingManager.brawlPitConstructed && GameManager.buildingManager.brawlPitInControl;
    warlockButton.available = GameManager.buildingManager.mageTowerConstructed && GameManager.buildingManager.mageTowerInControl;
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
      description = "The grunts of your army. Equipped with just a butter knife, these are the Goblins you send in to die first.";
    }

    else if (button == shootyBoyButton)
    {
      unitName = "Shooty Boy";
      description = "Armed with a crossbow, Shooty Boys are able to rain down hell on your foes from a distance.";

      if (!GameManager.buildingManager.archeryRangeConstructed)
      {
        constructMessage = "CONSTRUCT ARCHERY RANGE";
      }
    }

    else if (button == bruteButton)
    {
      unitName = "Brute";
      description = "Massive Goblins that can soak up damage and hold the frontline. Their massive reach allows them to hit multiple targets at once.";

      if (!GameManager.buildingManager.brawlPitConstructed)
      {
        constructMessage = "CONSTRUCT BRAWL PIT";
      }
    }

    else if (button == warlockButton)
    {
      unitName = "Warlock";
      description = "Practitioners of magic, your Warlocks keep your units alive in the battlefield, healing them, unless you choose to take them down a darker path....";

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
