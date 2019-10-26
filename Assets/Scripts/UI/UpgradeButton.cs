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

  private void Awake()
  {
    uiInterface = FindObjectOfType<UIInterface>();
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
      ExternalUpgrade(newButton);
    }
  }

  private void FixedUpdate()
  {
    GetComponent<Button>().interactable = (cost <= GameManager.resourceManager.Crystals) && (GameManager.upgradeManager.UpgradeIsAvailable(upgradeType));
  }

  private void Upgrade()
  {
    upgraded = true;
    nextLevelButton = GameManager.upgradeManager.UpgradeButton(upgradeType);

    if (nextLevelButton != null)
    {
      // Set the next level button's transform equal to this
      nextLevelButton.transform.position = transform.position;
      nextLevelButton.transform.SetParent(transform.parent);
      nextLevelButton.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
    }

    GameManager.resourceManager.UpgradeCost(this);

    uiInterface.HideUpgradeTooltipPopup();
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
    if (GameManager.CurrentPhase == PHASES.ESCORT || GameManager.CurrentPhase == PHASES.DEFENSE)
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
            description = "The Miner's motors are now maintained, increasing it's movement speed.";
            break;

          case 2:
            upgradeName = "Upgraded Stabby";
            description = "";
            break;

          case 3:
            upgradeName = "Upgraded Stabby";
            description = "";
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
            description = "The Miner's motors are now maintained, increasing it's movement speed.";
            break;

          case 2:
            upgradeName = "Upgraded Shooty";
            description = "";
            break;

          case 3:
            upgradeName = "Upgraded Shooty";
            description = "";
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
            description = "The Miner's motors are now maintained, increasing it's movement speed.";
            break;

          case 2:
            upgradeName = "Upgraded Brute";
            description = "";
            break;

          case 3:
            upgradeName = "Upgraded Brute";
            description = "";
            break;

          default:
            Debug.LogError("Upgrade button not found! Upgrade type is " + upgradeType + ", upgrade level is " + upgradeLevel);
            break;
        }
        break;

      case UPGRADE_TYPE.WARLOCK:
        break;

      case UPGRADE_TYPE.STABBY_CHEAP:
        switch (upgradeLevel)
        {
          case 1:
            upgradeName = "Upgraded Stabby Cheap";
            description = "The Miner's motors are now maintained, increasing it's movement speed.";
            break;

          case 2:
            upgradeName = "Upgraded Stabby Cheap";
            description = "";
            break;

          case 3:
            upgradeName = "Upgraded Stabby Cheap";
            description = "";
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
            description = "The Miner's motors are now maintained, increasing it's movement speed.";
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
    }

    UpgradeProperties upgradeProperties = GameManager.upgradeManager.RetrieveCurrentUpgradeProperties(upgradeType);

    uiInterface.ShowUpgradeTooltipPopup(upgradeName, upgradeProperties.cost, cost, upgradeProperties.health, upgradeProperties.damage, upgradeProperties.attackSpeed, description, constructMessage);
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    uiInterface.HideUpgradeTooltipPopup();
  }
}
