using UnityEngine;
using UnityEngine.UI;

public class UnitButton : MonoBehaviour
{
  private const float Y_OFFSET_UNSELECTED = 40f;
  private const float Y_OFFSET_SELECTED = 45f;

  [SerializeField]
  private Image healthBarPrefab = null;

  private UnitManager unitManager;

  // Once set, unit cannot be over written
  private GameObject unit = null;
  public GameObject Unit
  {
    get { return unit; }
    set { if (unit == null) unit = value; }
  }

  private Image healthBar = null;

  private void Awake()
  {
    unitManager = FindObjectOfType<UnitManager>();
    healthBar = Instantiate(healthBarPrefab, transform, false);
  }

  private void FixedUpdate()
  {
    unit.GetComponent<Health>().OnHealthChanged += HandleHealthChanged;
  }

  private void LateUpdate()
  {
    Afflictable afflictable = unit.GetComponent<Afflictable>();

    if (afflictable)
    {
      int numAfflictions = 0;

      healthBar.color = HealthBarController.blankColour;

      if (afflictable.posionAffliction.Active)
      {
        healthBar.color += HealthBarController.poisonColour;
        ++numAfflictions;
      }

      if (afflictable.slowAffliction.Active)
      {
        healthBar.color += HealthBarController.slowColour;
        ++numAfflictions;
      }

      if (afflictable.curseAffliction.Active)
      {
        healthBar.color += HealthBarController.curseColour;
        ++numAfflictions;
      }

      if (numAfflictions == 0)
      {
        healthBar.color = HealthBarController.normalColour;
      }

      else
      {
        healthBar.color /= numAfflictions;
      }
    }
  }

  public void DestroyButton()
  {
    unit.GetComponent<Health>().OnHealthChanged -= HandleHealthChanged;
    unitManager.KillUnit(gameObject, unit.GetComponent<RecruitableUnit>().unitPoints);
  }

  public void SetHealthBarTransform(Vector3 iconSize, bool selected)
  {
    Vector3 healthBarSize = healthBar.GetComponent<RectTransform>().sizeDelta;
    healthBarSize.x = iconSize.x;

    healthBar.GetComponent<RectTransform>().sizeDelta = healthBarSize;

    Vector3 healthBarPos = healthBar.GetComponent<RectTransform>().anchoredPosition;

    if (selected)
    {
      healthBarPos.y = Y_OFFSET_SELECTED;
    }

    else
    {
      healthBarPos.y = Y_OFFSET_UNSELECTED;
    }

    healthBar.GetComponent<RectTransform>().anchoredPosition = healthBarPos;
  }

  private void HandleHealthChanged(float pct)
  {
    healthBar.fillAmount = pct;
  }
}
