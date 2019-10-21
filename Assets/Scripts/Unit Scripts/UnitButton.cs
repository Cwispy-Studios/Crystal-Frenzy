using UnityEngine;

public class UnitButton : MonoBehaviour
{
  public UnitManager unitManager { private get; set; }

  // Once set, unit cannot be over written
  private GameObject unit = null;
  public GameObject Unit
  {
    get { return unit; }
    set { if (unit == null) unit = value; }
  }

  public void DestroyButton()
  {
    unitManager.KillUnit(gameObject, unit.GetComponent<RecruitableUnit>().unitPoints);
  }
}
