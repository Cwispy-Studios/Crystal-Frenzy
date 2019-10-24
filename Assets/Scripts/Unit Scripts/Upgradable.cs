using UnityEngine;

public class Upgradable : MonoBehaviour
{
  public UPGRADE_TYPE[] affectedByUpgrades;

  private void Awake()
  {
    SetUpgradedValues();
  }

  public void SetUpgradedValues()
  {
    for (int i = 0; i < affectedByUpgrades.Length; ++i)
    {
      // Retrieve the ugrade properties
      UpgradeProperties[] upgradeProperties = GameManager.upgradeManager.RetrieveUpgradeProperties(affectedByUpgrades[i]);

      if (upgradeProperties != null)
      {
        if (GetComponent<Health>())
        {
          GetComponent<Health>().SetUpgradedProperties(upgradeProperties);
        }

        if (GetComponent<Attack>())
        {
          GetComponent<Attack>().SetUpgradedProperties(upgradeProperties);
        }

        if (GetComponent<FieldOfVision>())
        {
          GetComponent<FieldOfVision>().SetUpgradedProperties(upgradeProperties);
        }

        if (GetComponent<RecruitableUnit>())
        {
          GetComponent<RecruitableUnit>().SetUpgradedProperties(upgradeProperties);
        }

        if (GetComponent<UnitOrder>())
        {
          GetComponent<UnitOrder>().SetUpgradedProperties(upgradeProperties);
        }

        if (GetComponent<CrystalSeeker>())
        {
          GetComponent<CrystalSeeker>().SetUpgradedProperties(upgradeProperties);
        }
      }     
    }
  }
}
