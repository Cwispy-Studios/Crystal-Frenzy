using UnityEngine;

public class RecruitableUnit : MonoBehaviour
{
  public int unitPoints = 1;

  // Once set, button cannot be over written
  private GameObject unitButton = null;
  public GameObject UnitButton
  {
    get { return unitButton; }
    set { if (unitButton == null) unitButton = value; }
  }
}
