using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
  public UPGRADE_TYPE upgradeType;

  public int cost;
  public UpgradeProperties upgradeProperties;

  private UIInterface uiInterface = null;
  private UnitManager unitManager = null;

  [HideInInspector]
  public bool upgraded = false;
  [HideInInspector]
  public GameObject nextLevelButton = null;

  private GameManager gameManager;

  private void Awake()
  {
    uiInterface = FindObjectOfType<UIInterface>();
    gameManager = FindObjectOfType<GameManager>();
    unitManager = FindObjectOfType<UnitManager>();
  }

  private void Start()
  {
    GetComponent<Button>().onClick.AddListener(Upgrade);

    GetComponent<Button>().interactable = (cost <= GameManager.resourceManager.Crystals) && (GameManager.upgradeManager.UpgradeIsAvailable(upgradeType));
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
    if (!upgraded)
    {
      GetComponent<Button>().interactable = (cost <= GameManager.resourceManager.Crystals) && (GameManager.upgradeManager.UpgradeIsAvailable(upgradeType));
    }
  }

  private void Upgrade()
  {
    GetComponent<Button>().interactable = false;
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
      GameManager.resourceManager.crystalsPerUnitPoint += 7;
    }

    if (upgradeType == UPGRADE_TYPE.MINER_HEALTH)
    {
      GameManager.minerManager.UpgradeMinerHealth(upgradeProperties.health);
    }

    if (unitManager && upgradeType != UPGRADE_TYPE.CRYSTAL_MINING && upgradeType != UPGRADE_TYPE.MINER_HEALTH && upgradeType != UPGRADE_TYPE.MINER_SPEED)
    {
      unitManager.RemoveAllUnits();
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
            description = "The Miner is constructed with sturdier materials, increasing its health and gaining some regeneration.";
            break;

          case 2:
            upgradeName = "Reinforced Plating";
            description = "The Miner is outfitted with additional platings, increasing its health and gaining some regeneration. The additional platings also slightly decrease the movement speed of the Miner.";
            break;

          case 3:
            upgradeName = "Imbued Plating";
            description = "The Miner's constrution is further improved, increasing its health and gaining some regeneration. The additional constructions also slightly decrease the movement speed of the Miner.";
            break;

          case 4:
            upgradeName = "Infused Plating";
            description = "The Miner's armour is infused with Crystal essense, greatly increasing its health, gaining some regeneration and slightly increasing it's movement speed.";
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
            description = "The Miner's motors are now maintained, increasing its movement speed.";
            break;

          case 2:
            upgradeName = "Fine Motors";
            description = "The Motors have been upgraded with better materials, increasing its movement speed.";
            break;

          case 3:
            upgradeName = "Powered Motors";
            description = "The Motors are now powered with steam, increasing its movement speed.";
            break;

          case 4:
            upgradeName = "Crystal Powered Motors";
            description = "The Miner's motors have been engineered to move with the help of Crystal energy, greatly increasing its movement speed.";
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
            description = "Equip your Stabby Goblins with a sharper butter knife and some clothes.";
            break;

          case 2:
            upgradeName = "Upgraded Stabby";
            description = "Equips your Stabby Goblins with a dagger and some armour.";
            break;

          case 3:
            upgradeName = "Upgraded Stabby";
            description = "Tell your Stabbys there are ready to become stronger, increasing their motivation.";
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
            description = "Equips your Shooty with better gear.";
            break;

          case 2:
            upgradeName = "Upgraded Shooty";
            description = "Upgrades your Shooty crossbows and armour.";
            break;

          case 3:
            upgradeName = "Upgraded Shooty";
            description = "Upgrades your Shooty crossbows and armour.";
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
            description = "Equip your Brutes with lighter and sharper swords.";
            break;

          case 2:
            upgradeName = "Upgraded Brute";
            description = "Equip your Brutes with thicker swords";
            break;

          case 3:
            upgradeName = "Upgraded Brute";
            description = "Equips your Brutes with stronger muscles.";
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
            description = "Your warlocks further practice the art of magic, for some money of course. Healing is increased.";
            break;

          case 2:
            upgradeName = "Upgraded Warlock";
            description = "Your warlocks further practice the art of magic, for some money of course. Healing is increased.";
            break;

          case 3:
            upgradeName = "Upgraded Warlock";
            description = "Your warlocks further practice the art of magic, for some money of course. Healing is increased.";
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
            upgradeName = "Stabby Cheap";
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
            upgradeName = "Stabby Beefed";
            description = "Your Stabby Goblins have been beefed up, becoming significantly stronger at the cost of higher upkeep.";
            break;

          case 2:
            upgradeName = "Upgraded Stabby Beefed";
            description = "Your Stabby Goblins have been beefed up, becoming significantly stronger at the cost of higher upkeep.";
            break;

          case 3:
            upgradeName = "Upgraded Stabby Beefed";
            description = "Your Stabby Goblins have been beefed up, becoming significantly stronger at the cost of higher upkeep.";
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
            upgradeName = "Shooty Fast";
            description = "Your Shooty Goblins prioritise speed over damage, while sacrificing some range.";
            break;

          case 2:
            upgradeName = "Upgraded Shooty Fast";
            description = "Your Shooty Goblins prioritise speed over damage, while sacrificing some range.";
            break;

          case 3:
            upgradeName = "Upgraded Shooty Fast";
            description = "Your Shooty Goblins prioritise speed over damage, while sacrificing some range.";
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
            upgradeName = "Shooty Nuke";
            description = "Your Shooty Goblins are now marksmen, dealing massive damage from a further distance, at the cost of lower attack speed.";
            break;

          case 2:
            upgradeName = "Upgraded Shooty Nuke";
            description = "Your Shooty Goblins are now marksmen, dealing massive damage from a further distance, at the cost of lower attack speed.";
            break;

          case 3:
            upgradeName = "Upgraded Shooty Nuke";
            description = "Your Shooty Goblins are now marksmen, dealing massive damage from a further distance, at the cost of lower attack speed.";
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
            upgradeName = "Brute Cheap";
            description = "Your Brutes have become more subdued and peaceful. At least they are cheaper.";
            break;

          case 2:
            upgradeName = "Upgraded Brute Cheap";
            description = "Your Brutes have become more subdued and peaceful. At least they are cheaper.";
            break;

          case 3:
            upgradeName = "Upgraded Brute Cheap";
            description = "Your Brutes have become more subdued and peaceful. At least they are cheaper.";
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
            upgradeName = "Brute Tank";
            description = "Your Brutes are now on steroids and deal massive splash damage. But they also eat more.";
            break;

          case 2:
            upgradeName = "Upgraded Brute Tank";
            description = "Your Brutes are now on steroids and deal massive splash damage. But they also eat more.";
            break;

          case 3:
            upgradeName = "Upgraded Brute Tank";
            description = "Your Brutes are now on steroids and deal massive splash damage. But they also eat more.";
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
            upgradeName = "Upgraded Saintly Warlocks";
            description = "Your Warlocks now speak the word of God, and are able to heal more efficiently.";
            break;

          case 3:
            upgradeName = "Upgraded Saintly Warlocks";
            description = "Your Warlocks now speak the word of God, and are able to heal more efficiently.";
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
            description = "Your warlocks dabble into Dark Magic, robbing them of their healing abilities but giving them splash damage.";
            break;

          case 2:
            upgradeName = "Upgraded Dark Warlock";
            description = "Your warlocks dabble further into Dark Magic, allowing them to Curse their targets, decreasing their damage.";
            break;

          case 3:
            upgradeName = "Upgraded Dark Warlock";
            description = "Your warlocks dabble furhter into Dark Magic, allowing them to also Slow their targets, decreasing their movement and attack speed.";
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
            description = "Increases the amount of crystals gained per unit point by remaining by 7 at the end of Escort and Defense Phases.";
            break;

          case 2:
            upgradeName = "Efficient Whipping";
            description = "Increases the amount of crystals gained per unit point remaining by 7 at the end of Escort and Defense Phases.";
            break;

          case 3:
            upgradeName = "Capital Punishment";
            description = "Increases the amount of crystals gained per unit point remaining by 7 at the end of Escort and Defense Phases.";
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
