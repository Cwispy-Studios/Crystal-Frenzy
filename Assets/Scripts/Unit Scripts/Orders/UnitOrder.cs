using UnityEngine.AI;

using UnityEngine;

public class UnitOrder : Order
{
  private float unitRadius;
  private bool followTarget = false;
  private Vector3 followTargetOldPos;

  private bool queuedOrder = false;
  private Vector3 queuedOrderPos = new Vector3();

  private float updateCountdown = 0;

  private void Awake()
  {
    unitRadius = GetComponent<NavMeshAgent>().radius * ((transform.lossyScale.x + transform.lossyScale.z) / 2f);
  }

  void Update()
  {
    if (updateCountdown < 1f)
    {
      ++updateCountdown;
      return;
    }

    else
    {
      updateCountdown = 0;
    }

    if (queuedOrder)
    {
      Animator animator = GetComponent<Animator>();

      if (animator)
      {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || animator.GetCurrentAnimatorStateInfo(0).IsName("Move"))
        {
          GetComponent<NavMeshAgent>().enabled = true;
          GetComponent<NavMeshObstacle>().enabled = false;
          GetComponent<NavMeshAgent>().destination = queuedOrderPos;
          queuedOrder = false;
        }
      }
    }

    if (followTarget)
    {
      GetComponent<NavMeshAgent>().enabled = true;
      GetComponent<NavMeshObstacle>().enabled = false;

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
    }

    NavMeshAgent agent = GetComponent<NavMeshAgent>();

    if (agent.enabled)
    {
      // Check if we have arrived yet
      if (agent.enabled && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f))
      {
        if (GetComponent<Attack>() != null)
        {
          GetComponent<Attack>().SetDetectingEnemies(true);
        }

        Animator animator = GetComponent<Animator>();

        if (animator && animator.enabled)
        {
          animator.SetBool("Move", false);
        }
      }

      else
      {
        Animator animator = GetComponent<Animator>();

        if (animator && animator.enabled)
        {
          animator.SetBool("Move", true);
        }

        GetComponent<NavMeshAgent>().enabled = true;
        GetComponent<NavMeshObstacle>().enabled = false;
      }
    }

    else
    {
      Animator animator = GetComponent<Animator>();

      if (animator && animator.enabled)
      {
        animator.SetBool("Move", false);
      }
    }
  }

  public override void IssueOrderPoint(Vector3 destinationOrder)
  {
    followTarget = false;

    GetComponent<NavMeshAgent>().enabled = true;
    GetComponent<NavMeshObstacle>().enabled = false;

    GetComponent<NavMeshAgent>().stoppingDistance = 0f;

    Animator animator = GetComponent<Animator>();

    if (animator && animator.enabled)
    {
      if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || animator.GetCurrentAnimatorStateInfo(0).IsName("Move"))
      {
        GetComponent<NavMeshAgent>().destination = destinationOrder;
      }

      else
      {
        queuedOrder = true;
        queuedOrderPos = destinationOrder;
      }
    }

    else
    {
      GetComponent<NavMeshAgent>().destination = destinationOrder;
    }
    

    if (GetComponent<Attack>() != null)
    {
      GetComponent<Attack>().SetDetectingEnemies(false);
    }
  }

  public override void IssueOrderTarget(GameObject targetUnit)
  {
    followTarget = true;
    destinationUnit = targetUnit;

    GetComponent<NavMeshAgent>().enabled = true;
    GetComponent<NavMeshObstacle>().enabled = false;

    if (GetComponent<Attack>() != null)
    {
      // Check if target is friendly or enemy
      Faction faction = GetComponent<Faction>();
      Faction targetFaction = targetUnit.GetComponent<Faction>();

      if (faction != null && targetFaction != null)
      {
        // Friendly/neutral unit
        if (targetFaction.faction == Faction.FACTIONS.NEUTRAL || faction.faction == targetFaction.faction)
        {
          if (GetComponent<Attack>().IsHealer && targetUnit.GetComponent<Faction>().faction == targetFaction.faction && !targetUnit.GetComponent<Health>().AtMaxHealth()
            && targetUnit.GetComponent<Health>().CombatantType == COMBATANT_TYPE.NORMAL)
          {
            GetComponent<Attack>().SetAttackTarget(targetUnit, true);
          }

          else
          {
            SetTargetAsDestination();
          }
        }

        // Enemy unit
        else
        {
          GetComponent<Attack>().SetAttackTarget(targetUnit, false);
        }
      }
      

      else
      {
        SetTargetAsDestination();
      }
    }
    
    else
    {
      SetTargetAsDestination();
    }
  }

  private void SetTargetAsDestination()
  {
    NavMeshAgent agent = GetComponent<NavMeshAgent>();

    Animator animator = GetComponent<Animator>();

    followTargetOldPos = destinationUnit.transform.position;

    if (animator != null && animator.enabled)
    {
      if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || animator.GetCurrentAnimatorStateInfo(0).IsName("Move"))
      {
        agent.destination = destinationUnit.transform.position;
      }

      else
      {
        queuedOrder = true;
        queuedOrderPos = destinationUnit.transform.position;
      }
    }

    else
    {
      agent.destination = destinationUnit.transform.position;
    }

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
      Debug.LogWarning("Unit Order destination unit has no NavMesh! Name is " + destinationUnit.name);
    }

    stoppingDistance += unitRadius;
    stoppingDistance *= 1.5f;

    agent.stoppingDistance = stoppingDistance;
  }

  public void SetUpgradedProperties(UpgradeProperties[] upgradeProperties)
  {
    for (int i = 0; i < upgradeProperties.Length; ++i)
    {
      GetComponent<NavMeshAgent>().speed += upgradeProperties[i].speed;
      GetComponent<NavMeshAgent>().acceleration = GetComponent<NavMeshAgent>().speed * 1.5f;
    }
  }

  public void SetBoostedValues(BoostValues boostValues)
  {
    GetComponent<NavMeshAgent>().speed += (GameManager.CurrentRound - 1) * boostValues.speedModifier * GetComponent<NavMeshAgent>().speed;
    GetComponent<NavMeshAgent>().acceleration = GetComponent<NavMeshAgent>().speed * 1.5f;
  }
} // class end
