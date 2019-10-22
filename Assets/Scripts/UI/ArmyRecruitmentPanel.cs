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

  private void Update()
  {
    shootyBoyButton.GetComponent<Button>().interactable = GameManager.buildingManager.archeryRangeConstructed;
    shootyBoyButton.available = GameManager.buildingManager.archeryRangeConstructed;
    bruteButton.GetComponent<Button>().interactable = GameManager.buildingManager.brawlPitConstructed;
    bruteButton.available = GameManager.buildingManager.brawlPitConstructed;
    warlockButton.GetComponent<Button>().interactable = GameManager.buildingManager.mageTowerConstructed;
    warlockButton.available = GameManager.buildingManager.mageTowerConstructed;
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

    uiInterface.ShowUnitTooltipPopup(unitName,
                                     recruitableUnit.GetComponent<RecruitableUnit>().unitPoints,
                                     recruitableUnit.GetComponent<Health>().MaxHealth,
                                     recruitableUnit.GetComponent<Attack>().AttackDamage,
                                     recruitableUnit.GetComponent<Attack>().AttacksPerSecond,
                                     description,
                                     constructMessage);
  }
}
