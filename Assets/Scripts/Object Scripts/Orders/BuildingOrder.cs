using UnityEngine;

public class BuildingOrder : Order
{
  public override void IssueOrderPoint(Vector3 destinationOrder)
  {
    GetComponent<Spawner>().SetWaypointAtPoint(destinationOrder);
  }

  public override void IssueOrderTarget(GameObject targetUnit)
  {
    GetComponent<Spawner>().SetWaypointAtUnit(targetUnit);
  }
}
