using UnityEngine;

public class UnitButton : MonoBehaviour
{
  private UnitManager unitManager;

  // Once set, unit cannot be over written
  private GameObject unit = null;
  public GameObject Unit
  {
    get { return unit; }
    set { if (unit == null) unit = value; }
  }

  private void Awake()
  {
    unitManager = FindObjectOfType<UnitManager>();
  }

  public void DestroyButton()
  {
    unitManager.KillUnit(gameObject, unit.GetComponent<RecruitableUnit>().unitPoints);
  }
}
