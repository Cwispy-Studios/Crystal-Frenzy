using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
  public UPGRADE_TYPE upgradeType;

  public int cost;
  public UpgradeProperties upgradeProperties;

  private UIInterface uiInterface = null;

  [HideInInspector]
  public bool upgraded = false;
  [HideInInspector]
  public GameObject nextLevelButton = null;

  private GameManager gameManager;

  private void Awake()
  {
    uiInterface = FindObjectOfType<UIInterface>();
    gameManager = FindObjectOfType<GameManager>();
  }

  private void Start()
  {
    GetComponent<Button>().onClick.AddListener(Upgrade);
  }

  private void Update()
  {
    // Check against the Upgrade Manager if it has been upgraded 
    GameObject newButton = GameManager.upgradeManager.CheckUpgradeLevel(upgradeType, upgradeProperties.upgradeLevel);

    if (newButton != null)
    {
      newButton.transform.position = transform.position;
      newButton.transform.SetParent(transform.parent);
      newButton.GetComponent<RectTransform>().localScale = Vector3.one;
      ExternalUpgrade(newButton);
    }
  }

  private void FixedUpdate()
  {
    GetComponent<Button>().interactable = (cost <= GameManager.resourceManager.Crystals) && (GameManager.upgradeManager.UpgradeIsAvailable(upgradeType));
  }

  private void Upgrade()
  {
    Camera.main.GetComponent<UISoundEmitter>().PlayButtonClick();
    upgraded = true;
    nextLevelButton = GameManager.upgradeManager.UpgradeButton(upgradeType);

    if (nextLevelButton != null)
    {
      // Set the next level button's transform equal to this
      nextLevelButton.transform.position = transform.position;
      nextLevelButton.transform.SetParent(transform.parent);
      nextLevelButton.GetComponent<RectTransform>().localScale = Vector3.one;
    }

    GameManager.resourceManager.UpgradeCost(this);

    uiInterface.HideUpgradeTooltipPopup();

    if (upgradeType == UPGRADE_TYPE.CRYSTAL_MINING)
    {
      GameManager.resourceManager.crystalsPerUnitPoint += 5;
    }

    if (upgradeType == UPGRADE_TYPE.MINER_HEALTH)
    {
      GameManager.minerManager.UpgradeMinerHealth(upgradeProperties.health);
    }
  }

  private void ExternalUpgrade(GameObject newButton)
  {
    upgraded = true;
    nextLevelButton = newButton;

    nextLevelButton.transform.position = transform.position;
    nextLevelButton.transform.SetParent(transform.parent);
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    if (gameManager.CurrentPhase == PHASES.ESCORT || gameManager.CurrentPhase == PHASES.DEFENSE)
    {
      return;
    }

    string upgradeName = "";
    string description = "";
    string constructMessage = "";

    int upgradeLevel = GameManager.upgradeManager.GetNextUpgradeLevel(upgradeType);

    switch (upgradeType)
    {
      case UPGRADE_TYPE.MINER_HEALTH:
        
        switch (upgradeLevel)
        {
          case 1:
            upgradeName = "Hardened Plating";
            description = "The Miner is constructed with sturdier materials, increasing it's health.";
            break;

          case 2:
            upgradeName = "Reinforced Plating";
            description = "The Miner is outfitted with additional platings, increasing it's health. The additional platings also slightly decrease the movement speed of the Miner.";
            break;

          case 3:
            upgradeName = "Imbued Plating";
            description = "The Miner's constrution is further improved, increasing it's health. The additional constructions also slightly decrease the movement speed of the Miner.";
            break;

          case 4:
            upgradeName = "Infused Plating";
            description = "The Miner's armour is infused with Crystal essense, greatly increating it's health and slightly increasing it's movement speed.";
            break;

          default:
            Debug.LogError("Upgrade button not found! Upgrade type is " + upgradeType + ", upgrade level is " + upgradeLevel);
            break;
        }

        break;

      case UPGRADE_TYPE.MINER_SPEED:
        switch (upgradeLevel)
        {
          case 1:
            upgradeName = "Greased Motors";
            description = "The Miner's motors are now maintained, increasing it's movement speed.";
            break;

          case 2:
            upgradeName = "Fine Motors";
            description = "";
            break;

          case 3:
            upgradeName = "Advanced Plating";
            description = "";
            break;

          case 4:
            upgradeName = "Crystal Powered Motors";
            description = "The Miner's motors have been engineered to move with the help of Crystal energy, greatly increasing it's movement speed.";
            break;

          default:
            Debug.LogError("Upgrade button not found! Upgrade type is " + upgradeType + ", upgrade level is " + upgradeLevel);
            break;
        }

        break;

      case UPGRADE_TYPE.STABBY:
        switch (upgradeLevel)
        {
          case 1:
            upgradeName = "Upgraded Stabby";
            description = "Equip your Stabby Goblins with a sharper butter knife and some clothes";
            break;

          case 2:
            upgradeName = "Upgraded Stabby";
            description = "Equips your Stabby Goblins with a dagger and some armour";
            break;

          case 3:
            upgradeName = "Upgraded Stabby";
            description = "Equips your Stabby Goblins w";
            break;

          default:
            Debug.LogError("Upgrade button not found! Upgrade type is " + upgradeType + ", upgrade level is " + upgradeLevel);
            break;
        }
        break;

      case UPGRADE_TYPE.SHOOTY:
        switch (upgradeLevel)
        {
          case 1:
            upgradeName = "Upgraded Shooty";
            description = "Increased Damage and Health.";
            break;

          case 2:
            upgradeName = "Upgraded Shooty";
            description = "Increased Damage and Health";
            break;

          case 3:
            upgradeName = "Upgraded Shooty";
            description = "Increased Damage and Health";
            break;

          default:
            Debug.LogError("Upgrade button not found! Upgrade type is " + upgradeType + ", upgrade level is " + upgradeLevel);
            break;
        }
        break;

      case UPGRADE_TYPE.BRUTE:
        switch (upgradeLevel)
        {
          case 1:
            upgradeName = "Upgraded Brute";
            description = "Increased Damage and Health.";
            break;

          case 2:
            upgradeName = "Upgraded Brute";
            description = "Increased Damage and Health";
            break;

          case 3:
            upgradeName = "Upgraded Brute";
            description = "Increased Damage and Health";
            break;

          default:
            Debug.LogError("Upgrade button not found! Upgrade type is " + upgradeType + ", upgrade level is " + upgradeLevel);
            break;
        }
        break;

      case UPGRADE_TYPE.WARLOCK:
        switch (upgradeLevel)
        {
          case 1:
            upgradeName = "Upgraded Warlock";
            description = "Increased Damage and Health, your Warlocks heal more";
            break;

          case 2:
            upgradeName = "Upgraded Warlock";
            description = "Increased Damage and Health, your Warlocks heal more";
            break;

          case 3:
            upgradeName = "Upgraded Warlock";
            description = "Increased Damage and Health, your Warlocks heal more";
            break;

          default:
            Debug.LogError("Upgrade button not found! Upgrade type is " + upgradeType + ", upgrade level is " + upgradeLevel);
            break;
        }
        break;

      case UPGRADE_TYPE.STABBY_CHEAP:
        switch (upgradeLevel)
        {
          case 1:
            upgradeName = "Upgraded Stabby Cheap";
            description = "Your Stabby Goblins are as ill equipped as before. At least they are still cheap.";
            break;

          case 2:
            upgradeName = "Upgraded Stabby Cheap";
            description = "Your Stabby Goblins are as ill equipped as before. At least they are still cheap.";
            break;

          case 3:
            upgradeName = "Upgraded Stabby Cheap";
            description = "Your Stabby Goblins are as ill equipped as before. At least they are still cheap.";
            break;

          default:
            Debug.LogError("Upgrade button not found! Upgrade type is " + upgradeType + ", upgrade level is " + upgradeLevel);
            break;
        }
        break;

      case UPGRADE_TYPE.STABBY_BEEFED:
        switch (upgradeLevel)
        {
          case 1:
            upgradeName = "Upgraded Stabby Beefed";
            description = "Your Stabby Goblins have been beefed up, becoming significantly stronger at the cost of higher upkeep.";
            break;

          case 2:
            upgradeName = "Upgraded Stabby Beefed";
            description = "";
            break;

          case 3:
            upgradeName = "Upgraded Stabby Beefed";
            description = "";
            break;

          default:
            Debug.LogError("Upgrade button not found! Upgrade type is " + upgradeType + ", upgrade level is " + upgradeLevel);
            break;
        }
        break;

      case UPGRADE_TYPE.SHOOTY_FAST:
        switch (upgradeLevel)
        {
          case 1:
            upgradeName = "Upgraded Shooty Fast";
            description = "Your Shooty Goblins prioritise speed over damage, while sacrificing some range.";
            break;

          case 2:
            upgradeName = "Upgraded Shooty Fast";
            description = "";
            break;

          case 3:
            upgradeName = "Upgraded Shooty Fast";
            description = "";
            break;

          default:
            Debug.LogError("Upgrade button not found! Upgrade type is " + upgradeType + ", upgrade level is " + upgradeLevel);
            break;
        }
        break;

      case UPGRADE_TYPE.SHOOTY_NUKE:
        switch (upgradeLevel)
        {
          case 1:
            upgradeName = "Upgraded Shooty Nuke";
            description = "Your Shooty Goblins are now marksmen, dealing massive damage from a further distance, at the cost of lower attack speed.";
            break;

          case 2:
            upgradeName = "Upgraded Shooty Nuke";
            description = "";
            break;

          case 3:
            upgradeName = "Upgraded Shooty Nuke";
            description = "";
            break;

          default:
            Debug.LogError("Upgrade button not found! Upgrade type is " + upgradeType + ", upgrade level is " + upgradeLevel);
            break;
        }
        break;

      case UPGRADE_TYPE.BRUTE_CHEAP:
        switch (upgradeLevel)
        {
          case 1:
            upgradeName = "Upgraded Brute Cheap";
            description = "Your Brutes are now less ";
            break;

          case 2:
            upgradeName = "Upgraded Brute Cheap";
            description = "";
            break;

          case 3:
            upgradeName = "Upgraded Brute Cheap";
            description = "";
            break;

          default:
            Debug.LogError("Upgrade button not found! Upgrade type is " + upgradeType + ", upgrade level is " + upgradeLevel);
            break;
        }
        break;

      case UPGRADE_TYPE.BRUTE_TANK:
        switch (upgradeLevel)
        {
          case 1:
            upgradeName = "Upgraded Brute Tank";
            description = "The Miner's motors are now maintained, increasing it's movement speed.";
            break;

          case 2:
            upgradeName = "Upgraded Brute Tank";
            description = "";
            break;

          case 3:
            upgradeName = "Upgraded Brute Tank";
            description = "";
            break;

          default:
            Debug.LogError("Upgrade button not found! Upgrade type is " + upgradeType + ", upgrade level is " + upgradeLevel);
            break;
        }
        break;

      case UPGRADE_TYPE.WARLOCK_HEALER:
        switch (upgradeLevel)
        {
          case 1:
            upgradeName = "Saintly Warlocks";
            description = "Your Warlocks now speak the word of God, and are able to heal more efficiently.";
            break;

          case 2:
            upgradeName = "Saintly Warlocks";
            description = "";
            break;

          case 3:
            upgradeName = "Upgraded Brute Cheap";
            description = "";
            break;

          default:
            Debug.LogError("Upgrade button not found! Upgrade type is " + upgradeType + ", upgrade level is " + upgradeLevel);
            break;
        }
        break;

      case UPGRADE_TYPE.WARLOCK_OFFENSIVE:
        switch (upgradeLevel)
        {
          case 1:
            upgradeName = "Dark Warlock";
            description = "Your warlocks dabble into Dark Magic, giving them splash damage.";
            break;

          case 2:
            upgradeName = "Dark Warlock";
            description = "";
            break;

          case 3:
            upgradeName = "Upgraded Brute Tank";
            description = "";
            break;

          default:
            Debug.LogError("Upgrade button not found! Upgrade type is " + upgradeType + ", upgrade level is " + upgradeLevel);
            break;
        }
        break;

      case UPGRADE_TYPE.CRYSTAL_MINING:
        switch (upgradeLevel)
        {
          case 1:
            upgradeName = "Efficient Manpower";
            description = "Increases the amount of crystals gained per unit point remaining at the end of Escort and Defense Phases.";
            break;

          case 2:
            upgradeName = "Efficient Whipping";
            description = "Increases the amount of crystals gained per unit point remaining at the end of Escort and Defense Phases.";
            break;

          case 3:
            upgradeName = "Capital Punishment";
            description = "Increases the amount of crystals gained per unit point remaining at the end of Escort and Defense Phases.";
            break;

          default:
            Debug.LogError("Upgrade button not found! Upgrade type is " + upgradeType + ", upgrade level is " + upgradeLevel);
            break;
        }
        break;
    }

    UpgradeProperties upgradeProperties = GameManager.upgradeManager.RetrieveCurrentUpgradeProperties(upgradeType);

    uiInterface.ShowUpgradeTooltipPopup(upgradeName, upgradeProperties.cost, cost, upgradeProperties.health, upgradeProperties.damage, upgradeProperties.attackSpeed, description, constructMessage);
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    uiInterface.HideUpgradeTooltipPopup();
  }
}
