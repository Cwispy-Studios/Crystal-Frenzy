using UnityEngine;

public class Order : MonoBehaviour
{
  protected Vector3 destination;
  protected GameObject destinationUnit;
  protected bool executeOrderPoint = false;

  private void Awake()
  {
    destination = new Vector3();
  }

  public virtual void IssueOrderPoint(Vector3 destinationOrder)
  {

  }

  public virtual void IssueOrderTarget(GameObject targetUnit)
  {

  }
}
