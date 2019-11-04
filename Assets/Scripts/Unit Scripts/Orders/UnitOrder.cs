using UnityEngine.AI;

using UnityEngine;

public class UnitOrder : Order
{
  private float unitRadius;
  private bool followTarget = false;
  private Vector3 followTargetOldPos;

  private bool queuedOrder = false;
  private Vector3 queuedOrderPos;

  private float updateCountdown = 0;

  private Animator animator = null;
  private NavMeshAgent agent = null;
  private NavMeshObstacle obstacle = null;
  private Attack attack;
  private AnimationState animationState;

  private void Awake()
  {
    unitRadius = GetComponent<NavMeshAgent>().radius * ((transform.lossyScale.x + transform.lossyScale.z) / 2f);

    animator = GetComponent<Animator>();
    agent = GetComponent<NavMeshAgent>();
    obstacle = GetComponent<NavMeshObstacle>();
    attack = GetComponent<Attack>();
    animationState = GetComponent<AnimationState>();
  }

  void Update()
  {
    if (animationState.currentAnimationState == CURRENT_ANIMATION_STATE.ATTACK)
    {
      return;
    }

    // Set the move and idle state if agent is enabled (not attacking)
    if (agent.enabled && animationState.currentAnimationState != CURRENT_ANIMATION_STATE.ATTACK)
    {
      if (agent.velocity != Vector3.zero)
      {
        animator.SetBool("Move", true);
        animationState.currentAnimationState = CURRENT_ANIMATION_STATE.MOVE;
      }

      else
      {
        animator.SetBool("Move", false);
        animationState.currentAnimationState = CURRENT_ANIMATION_STATE.IDLE;
      }
    }

    if (queuedOrder)
    {
      // Only move when not attacking
      if (animationState.currentAnimationState != CURRENT_ANIMATION_STATE.ATTACK)
      {
        obstacle.enabled = false;
        agent.enabled = true;

        agent.destination = queuedOrderPos;
        agent.stoppingDistance = 0f;
        queuedOrder = false;

        animationState.currentAnimationState = CURRENT_ANIMATION_STATE.MOVE;
      }
    }

    if (updateCountdown < 1f)
    {
      updateCountdown += Time.deltaTime;
      return;
    }

    else
    {
      updateCountdown = 0;
    }

    // If following target and attack animation completed
    if (followTarget && animationState.currentAnimationState != CURRENT_ANIMATION_STATE.ATTACK)
    {
      obstacle.enabled = false;
      agent.enabled = true;

      // Target still alive
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
      agent.stoppingDistance = 0f;
      followTarget = false;
    }

    // If not attacking
    if (agent.enabled)
    {
      // Check if we have arrived yet, make unit detect enemies
      if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f))
      {
        attack.SetDetectingEnemies(true);
        animationState.currentAnimationState = CURRENT_ANIMATION_STATE.IDLE;
      }

      // Not arrived, move
      else
      {
        GetComponent<NavMeshObstacle>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = true;

        animationState.currentAnimationState = CURRENT_ANIMATION_STATE.MOVE;
      }
    }
  }

  public override void IssueOrderPoint(Vector3 destinationOrder)
  {
    followTarget = false;

    // Check if unit's attack status is done attacking (hit has landed), then we can override the animation
    if (!attack.isAttacking)
    {
      obstacle.enabled = false;
      agent.enabled = true;

      agent.destination = destinationOrder;
      agent.stoppingDistance = 0f;

      animator.StopPlayback();

      animationState.currentAnimationState = CURRENT_ANIMATION_STATE.MOVE;
    }

    // Unit is in the middle of attacking, queue the order
    else
    {
      queuedOrder = true;
      queuedOrderPos = destinationOrder;
    }
    
    attack.SetDetectingEnemies(false);
  }

  public override void IssueOrderTarget(GameObject targetUnit)
  {
    followTarget = true;
    destinationUnit = targetUnit;

    // Check if target is friendly or enemy
    Faction faction = GetComponent<Faction>();
    Faction targetFaction = targetUnit.GetComponent<Faction>();

    // Friendly/neutral unit
    if (targetFaction.faction == Faction.FACTIONS.NEUTRAL || faction.faction == targetFaction.faction)
    {
      // Check if is healer and unit is not at full health and is not crystal seeker
      if (attack.IsHealer && !targetUnit.GetComponent<Health>().AtMaxHealth() && targetUnit.GetComponent<Health>().CombatantType == COMBATANT_TYPE.NORMAL)
      {
        // Heal target
        attack.SetAttackTarget(targetUnit, true);
      }

      // Move to target
      else
      {
        SetTargetAsDestination();
      }
    }

    // Enemy unit
    else
    {
      attack.SetAttackTarget(targetUnit, false);
    }
  }

  private void SetTargetAsDestination()
  {
    // Update the cached position to follow
    followTargetOldPos = destinationUnit.transform.position;

    // Check if unit's attack status is done attacking (hit has landed), then we can override the animation
    if (!attack.isAttacking)
    {
      obstacle.enabled = false;
      agent.enabled = true;

      agent.destination = destinationUnit.transform.position;

      animationState.currentAnimationState = CURRENT_ANIMATION_STATE.MOVE;
    }

    else
    {
      queuedOrder = true;
      queuedOrderPos = destinationUnit.transform.position;
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
