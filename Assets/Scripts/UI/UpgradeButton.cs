using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpgradeButton : MonoBehaviour
{
  public UPGRADE_TYPE upgradeType;

  [SerializeField]
  private GameObject targetPrefab;

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
            description = "The Miner's motors are now regularly maintained, increasing it's movement speed.";
            break;

          case 2:
            upgradeName = "Reinforced Plating";
            description = "The Miner is outfitted with additional platings, increasing it's health. The additional platings also slow down the Miner.";
            break;

          case 3:
            upgradeName = "Imbued Plating";
            description = "The Miner's constrution is further improved, increasing it's health. The additional constructions also slow down the Miner.";
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

      case UPGRADE_TYPE.STABBY:
        break;

      case UPGRADE_TYPE.SHOOTY:
        break;

      case UPGRADE_TYPE.BRUTE:
        break;

      case UPGRADE_TYPE.WARLOCK:
        break;
    }

    //GetComponentInParent<ArmyRecruitmentPanel>().SetText(this, recruitableUnit);
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    uiInterface.HideUnitTooltipPopup();
  }
}
