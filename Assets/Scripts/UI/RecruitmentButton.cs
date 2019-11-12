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
    if (!available || GameManager.resourceManager.ArmySize + recruitableUnit.GetComponent<RecruitableUnit>().unitPoints  > GameManager.resourceManager.UnitCap)
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
