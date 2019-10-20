using UnityEngine;

public class CrystalOrder : Order
{
  public override void IssueOrderPoint(Vector3 destinationOrder)
  {
  }

  public override void IssueOrderTarget(GameObject targetUnit)
  {
    CrystalSeekerSpawner crystalSeekerSpawner = GetComponent<CrystalSeekerSpawner>();

    if (crystalSeekerSpawner != null)
    {
      crystalSeekerSpawner.SetCrystalTarget(targetUnit);
    }
  }
}
