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

  private void Awake()
  {
    uiInterface = FindObjectOfType<UIInterface>();
  }

  private void Start()
  {
    GetComponent<Button>().onClick.AddListener(AddToUnitPanel);
  }

  private void Update()
  {
    if (GameManager.resourceManager.ArmySize + recruitableUnit.GetComponent<RecruitableUnit>().unitPoints > GameManager.resourceManager.UnitCap)
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
    uiInterface.ShowUnitTooltipPopup(recruitableUnit.name,
                                     recruitableUnit.GetComponent<RecruitableUnit>().unitPoints,
                                     recruitableUnit.GetComponent<Health>().MaxHealth,
                                     recruitableUnit.GetComponent<Attack>().AttackDamage,
                                     recruitableUnit.GetComponent<Attack>().AttacksPerSecond);
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    uiInterface.HideUnitTooltipPopup();
  }
}
