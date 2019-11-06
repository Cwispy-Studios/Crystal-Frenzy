using UnityEngine;

public class RecruitableUnit : MonoBehaviour
{
  public int unitPoints = 1;

  private float doubleClickInterval = 0.3f;
  private float timeSinceLastClick = 0f;

  // Once set, button cannot be over written
  private GameObject unitButton = null;
  public GameObject UnitButton
  {
    get { return unitButton; }
    set { if (unitButton == null) unitButton = value; }
  }

  private void Update()
  {
    timeSinceLastClick += Time.deltaTime;
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

  public bool ClickedOnUnit()
  {
    // It is a double click, tell the unit manager to add all of this unit type to its selection
    if (timeSinceLastClick <= 0.3f)
    {
      return true;
    }

    timeSinceLastClick = 0f;

    return false;
  }
}
