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

  private void FixedUpdate()
  {
    // Check against the Upgrade Manager if it has been upgraded 
    GameObject newButton = GameManager.upgradeManager.CheckUpgradeLevel(upgradeType, upgradeProperties.upgradeLevel);
    if (newButton != null)
    {
      ExternalUpgrade(newButton);
    }

    GetComponent<Button>().interactable = (cost <= GameManager.resourceManager.Crystals);
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
        break;

      case UPGRADE_TYPE.SHOOTY:
        break;

      case UPGRADE_TYPE.BRUTE:
        break;

      case UPGRADE_TYPE.WARLOCK:
        break;
    }

    UpgradeProperties upgradeProperties = GameManager.upgradeManager.RetrieveCurrentUpgradeProperties(upgradeType);

    uiInterface.ShowUpgradeTooltipPopup(upgradeName, cost, upgradeProperties.health, upgradeProperties.damage, upgradeProperties.attackSpeed, description, constructMessage);
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    uiInterface.HideUpgradeTooltipPopup();
  }
}
