using UnityEngine;

public class CrystalOrder : Order
{
  // To add disable component in inspector
  private void Start() { }

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
