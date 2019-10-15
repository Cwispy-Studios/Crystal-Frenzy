using UnityEngine.AI;

using UnityEngine;

public class UnitOrder : Order
{
  private float unitRadius;
  private bool followTarget = false;
  private Vector3 followTargetOldPos;

  private void Awake()
  {
    unitRadius = GetComponent<NavMeshAgent>().radius * ((transform.lossyScale.x + transform.lossyScale.z) / 2f);
  }

  // Update is called once per frame
  void Update()
  {
    if (followTarget)
    {
      // Target still exists
      if (destinationUnit != null)
      {
        if (followTargetOldPos != destinationUnit.transform.position)
        {
          SetTargetAsDestination();
        }
      }
    }

    // Target is dead
    else
    {
      GetComponent<NavMeshAgent>().stoppingDistance = 0f;
      followTarget = false;

      return;
    }
  }

  public override void IssueOrderPoint(Vector3 destinationOrder)
  {
    followTarget = false;

    GetComponent<NavMeshAgent>().stoppingDistance = 0f;
    GetComponent<NavMeshAgent>().destination = destinationOrder;
  }

  public override void IssueOrderTarget(GameObject targetUnit)
  {
    followTarget = true;
    destinationUnit = targetUnit;

    SetTargetAsDestination();
  }

  private void SetTargetAsDestination()
  {
    NavMeshAgent agent = GetComponent<NavMeshAgent>();

    followTargetOldPos = destinationUnit.transform.position;
    agent.destination = destinationUnit.transform.position;

    float stoppingDistance = 0f;

    // Find the size of the target unit and set the stopping distance accordingly.
    if (destinationUnit.GetComponent<NavMeshAgent>() != null)
    {
      stoppingDistance = destinationUnit.GetComponent<NavMeshAgent>().radius *
        ((destinationUnit.transform.lossyScale.x + destinationUnit.transform.lossyScale.z) / 2f);
    }

    else if (destinationUnit.GetComponent<NavMeshObstacle>() != null)
    {
      stoppingDistance = destinationUnit.GetComponent<NavMeshObstacle>().radius *
        ((destinationUnit.transform.lossyScale.x + destinationUnit.transform.lossyScale.z) / 2f);
    }

    else
    {
      Debug.LogError("Unit Order destination unit has no NavMesh! Name is " + destinationUnit.name);
    }

    stoppingDistance += unitRadius;
    stoppingDistance *= 1.5f;

    agent.stoppingDistance = stoppingDistance;

    // Check if we have arrived yet
    //if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f))
    //{
    //  return;
    //}
  }
} // class end
