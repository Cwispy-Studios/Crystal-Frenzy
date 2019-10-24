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

  public void KillUnit()
  {
    unitButton.GetComponent<UnitButton>().DestroyButton();
    Destroy(unitButton);
  }

  public void SetUpgradedProperties(UpgradeProperties[] upgradeProperties)
  {
    for (int i = 0; i < upgradeProperties.Length; ++i)
    {
      unitPoints += upgradeProperties[i].cost;
    }
  }
}
