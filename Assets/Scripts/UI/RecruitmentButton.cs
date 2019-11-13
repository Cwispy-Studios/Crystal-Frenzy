using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RecruitmentButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
  [SerializeField]
  private GameObject recruitableUnit = null;
  [SerializeField]
  private UnitManager unitManager = null;

  private UIInterface uiInterface = null;

  [HideInInspector]
  public bool available = true;

  private GameManager gameManager;

  private void Awake()
  {
    uiInterface = FindObjectOfType<UIInterface>();
    gameManager = FindObjectOfType<GameManager>();
  }

  private void Start()
  {
    GetComponent<Button>().onClick.AddListener(AddToUnitPanel);
  }

  private void FixedUpdate()
  {
    int unitPoints = recruitableUnit.GetComponent<RecruitableUnit>().unitPoints;

    UPGRADE_TYPE[] affectedByUpgrades = recruitableUnit.GetComponent<Upgradable>().affectedByUpgrades;

    for (int i = 0; i < affectedByUpgrades.Length; ++i)
    {
      // Retrieve the ugrade properties
      UpgradeProperties[] upgradeProperties = GameManager.upgradeManager.RetrieveUpgradeProperties(affectedByUpgrades[i]);

      if (upgradeProperties != null)
      {
        for (int up = 0; up < upgradeProperties.Length; ++up)
        {
          unitPoints += upgradeProperties[up].cost;
        }
      }
    }

    if (!available || GameManager.resourceManager.ArmySize + unitPoints > GameManager.resourceManager.UnitCap)
    {
      GetComponent<Button>().interactable = false;
    }

    else
    {
      GetComponent<Button>().interactable = true;
    }
  }

  private void AddToUnitPanel()
  {
    unitManager.AddUnitToPanel(recruitableUnit, GetComponent<Image>().sprite);
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    if (gameManager.CurrentPhase == PHASES.ESCORT || gameManager.CurrentPhase == PHASES.DEFENSE)
    {
      return;
    }

    GetComponentInParent<ArmyRecruitmentPanel>().SetText(this, recruitableUnit);
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    uiInterface.HideUnitTooltipPopup();
  }
}
