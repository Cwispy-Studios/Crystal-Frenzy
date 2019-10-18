using UnityEngine;

public class UnitButton : MonoBehaviour
{
  // Once set, unit cannot be over written
  private GameObject unit;
  public GameObject Unit
  {
    get { return unit; }
    set { if (unit != null) unit = value; }
  }
}
