using UnityEngine;
using UnityEngine.AI;

public class Attack : MonoBehaviour
{
  private const float STOPPING_MARGIN = 1f;

  [SerializeField]
  private float attackDamage = 0;
  public float AttackDamage
  {
    get { return attackDamage; }
  }

  [SerializeField]
  private float attacksPerSecond = 1f, attackRange = 1f, enemyDetectRange = 1f;
  public float AttacksPerSecond
  {
    get { return attacksPerSecond; }
  }

  private float attackInterval;

  private float normalAttacksPerSecond;
  private float normalMoveSpeed;
  private float normalDamage;

  private float ogDamage;
  private float ogAttackSpeed;
  private float ogAttackRange;
  private float ogDetectRange;

  [SerializeField]
  private bool aoe = false;
  [SerializeField]
  private float aoeRadius = 1f, aoeDmgPct = 1f;

  [SerializeField]
  private bool isHealer = false;
  public bool IsHealer
  {
    get
    {
      return isHealer;
    }
  }
  [SerializeField]
  private float healPct = 0.5f;
  private bool isHealing = false;

  [SerializeField]
  private GameObject healParticleSystem = null;

  [SerializeField]
  private COMBATANT_TYPE preferredTarget = COMBATANT_TYPE.NORMAL;
  [SerializeField]
  private GameObject projectilePrefab = null;

  private float attackCooldown = 0;
  private float unitRadius = 0;

  private bool detectingEnemies = true;
  private GameObject detectedTarget = null;

  private GameObject attackTarget = null;

  private GameObject attackedTarget = null;

  private Vector3 attackMovePosition;
  private bool isAttackMoveOrder = false;

  private const float DETECT_UPDATE_INTERVAL = 1f;
  private float detectUpdateCountdown = 0;

  private const float ATTACK_UPDATE_INTERVAL = 0f;
  private float attackUpdateCountdown = 0;

  private bool queuedOrder = false;
  private Vector3 queuedOrderPos;
  private bool queuedHealing = false;

  private Animator animator = null;
  private NavMeshAgent agent = null;
  private NavMeshObstacle obstacle = null;
  private AnimationState animationState;

  private GameManager gameManager;

  [HideInInspector]
  public bool isAttacking = false;

  [FMODUnity.EventRef]
  public string attackSound = "", healSound = "";

  private void Awake()
  {
    unitRadius = GetComponent<NavMeshAgent>().radius * ((transform.lossyScale.x + transform.lossyScale.z) / 2f);
    ogDamage = attackDamage;
    ogAttackSpeed = attacksPerSecond;
    ogAttackRange = attackRange;
    ogDetectRange = enemyDetectRange;

    animator = GetComponent<Animator>();
    agent = GetComponent<NavMeshAgent>();
    obstacle = GetComponent<NavMeshObstacle>();
    animationState = GetComponent<AnimationState>();

    gameManager = FindObjectOfType<GameManager>();
  }

  private void Start()
  {
    attackCooldown = attacksPerSecond;
    normalAttacksPerSecond = attacksPerSecond;
    normalMoveSpeed = GetComponent<NavMeshAgent>().speed;
    normalDamage = attackDamage;

    attackInterval = 1f / attacksPerSecond;
  }

  private void Update()
  {
    // Update attack cooldown, and attack target intervals
    if (attackCooldown <= attackInterval)
    {
      attackCooldown += Time.deltaTime;
    }

    if (detectUpdateCountdown < DETECT_UPDATE_INTERVAL)
    {
      detectUpdateCountdown += Time.deltaTime;
    }

    // As long as unit is not detecting enemies, it is not under an attack move order, so we set it to false
    if (detectingEnemies == false)
    {
      isAttackMoveOrder = false;
      detectedTarget = null;
    }

    // Rotates the unit towards its attack target while its hit has not landed and target still alive
    if (isAttacking && attackTarget)
    {
      LookTowardsTarget(attackTarget);
    }

    // Rotates the unit towards its detected eney
    else if (detectedTarget)
    {
      LookTowardsTarget(detectedTarget);
    }
    
    // While it is in the middle of animation, do not perform any other logic
    if (isAttacking)
    {
      return;
    }

    // Only check queue order when unit is no longer attacking
    if (animationState.currentAnimationState != CURRENT_ANIMATION_STATE.ATTACK && queuedOrder)
    {
      obstacle.enabled = false;
      agent.enabled = true;

      queuedOrder = false;
      agent.destination = queuedOrderPos;
      isHealing = queuedHealing;

      animationState.currentAnimationState = CURRENT_ANIMATION_STATE.MOVE;
    }

    // Detect enemies
    if (detectUpdateCountdown >= DETECT_UPDATE_INTERVAL)
    {
      detectUpdateCountdown = 0f;

      // If there is no attack target and is detecting enemies, unit will constantly look out for enemies within their detect range
      if (attackTarget == null && detectingEnemies)
      {
        DetectEnemies();
      }
    }

    // Attack targeted or detected enemy
    if (attackTarget != null)
    {
      float enemyRange = Vector3.Distance(transform.position, attackTarget.transform.position);

      AttackEnemy(attackTarget, enemyRange, isHealing);
    }

    else if (detectingEnemies && detectedTarget != null)
    {
      float enemyRange = Vector3.Distance(transform.position, detectedTarget.transform.position);

      AttackEnemy(detectedTarget, enemyRange, isHealing);
    }
  }

  private void DetectEnemies()
  {
    Collider[] unitsInRange = Physics.OverlapSphere(transform.position, enemyDetectRange);

    float closestEnemyRange = 9999f;

    bool preferredTargetTypeInRange = false;

    float lowestHpPct = 100f;
    isHealing = false;

    for (int i = 0; i < unitsInRange.Length; ++i)
    {
      bool unitIsEnemy = CheckUnitIsTargetableEnemy(unitsInRange[i].gameObject);

      // Check if this unit is a healer and the unit in range is a friend and has a health component and is not itself
      if (isHealer && !unitIsEnemy && unitsInRange[i].GetComponent<Health>() && unitsInRange[i].gameObject != gameObject)
      {
        // Check if unit can be healed and is the preferred type
        if (!unitsInRange[i].GetComponent<Health>().AtMaxHealth() && unitsInRange[i].GetComponent<Health>().CombatantType == preferredTarget)
        {
          isHealing = true;

          // Save the distance
          float distance = Vector3.Distance(transform.position, unitsInRange[i].transform.position);

          // Prioritise lowest hp
          if (unitsInRange[i].GetComponent<Health>().HealthPct() < lowestHpPct)
          {
            closestEnemyRange = distance;
            detectedTarget = unitsInRange[i].gameObject;
          }
        }
      }

      // Check if the unit in range is an enemy and has a health component
      else if (!isHealing && unitIsEnemy && unitsInRange[i].GetComponent<Health>())
      {
        // First we check if the enemy has already found their preferred target type. If not found, they can target anyone.
        // If found, we only search for enemies of that target type
        if (preferredTargetTypeInRange)
        {
          if (unitsInRange[i].GetComponent<Health>().CombatantType == preferredTarget)
          {
            // Check if the target is the preferred type
            float distance = Vector3.Distance(transform.position, unitsInRange[i].transform.position);

            if (distance < closestEnemyRange)
            {
              closestEnemyRange = distance;
              detectedTarget = unitsInRange[i].gameObject;
            }
          }
        }

        else
        {
          if (unitsInRange[i].GetComponent<Health>().CombatantType == preferredTarget)
          {
            preferredTargetTypeInRange = true;

            // We reset the distance so that it prioritises this new found preferred target over what it already has 
            closestEnemyRange = 9999f;
          }

          // Check if the target is the preferred type
          float distance = Vector3.Distance(transform.position, unitsInRange[i].transform.position);

          if (distance < closestEnemyRange)
          {
            closestEnemyRange = distance;
            detectedTarget = unitsInRange[i].gameObject;
          }
        }
      }
    }

    // Check if any enemy found
    if (detectedTarget != null)
    {
      AttackEnemy(detectedTarget, closestEnemyRange, isHealing);
    }

    // No enemies found, check if there is an attack order position to move towards to
    else
    {
      if (isAttackMoveOrder)
      {
        obstacle.enabled = false;
        agent.enabled = true;

        agent.stoppingDistance = 0;
        agent.destination = attackMovePosition;
      }
    }
  }

  private bool CheckUnitIsTargetableEnemy(GameObject checkUnit)
  {
    if (checkUnit.GetComponent<Faction>() != null)
    {
      if (GetComponent<Faction>().faction == Faction.FACTIONS.GOBLINS)
      {
        if (checkUnit.GetComponent<Faction>().faction == Faction.FACTIONS.FOREST)
        {
          return true;
        }
      }

      else if (GetComponent<Faction>().faction == Faction.FACTIONS.FOREST)
      {
        if (checkUnit.GetComponent<Faction>().faction == Faction.FACTIONS.GOBLINS)
        {
          return true;
        }
      }
    }

    return false;    
  }

  private void AttackEnemy(GameObject enemy, float enemyRange, bool healing)
  {
    attackedTarget = enemy;

    // Try to attack them, check the distance between them. The distance from their position + their radius to your position + your radius 
    // must be smaller than the attack range
    float enemyRadius = GetEnemyRadius(attackedTarget);

    // If enemy is near enough, try to attack and hold position. Otherwise set the NavMeshAgent to move towards it
    if (animationState.currentAnimationState != CURRENT_ANIMATION_STATE.ATTACK && enemyRange <= (attackRange + unitRadius + enemyRadius))
    {
      // Check if ready to attack
      if (attackCooldown >= attackInterval)
      {
        isAttacking = true;
        // Set the animation status to attacking
        animationState.currentAnimationState = CURRENT_ANIMATION_STATE.ATTACK;

        // Within range so don't move
        agent.enabled = false;
        obstacle.enabled = true;

        animator.SetBool("Move", false);

        // Check if there is projectile
        if (isHealing || projectilePrefab != null)
        {
          animator.SetTrigger("Ranged Attack");
        }

        // No projectile means attack hits immediately
        else
        {
          animator.SetTrigger("Melee Attack");
        }

        attackCooldown -= attackInterval;
      }
    }

    // Only move automatically once animation is complete 
    else if (animationState.currentAnimationState != CURRENT_ANIMATION_STATE.ATTACK)
    {
      obstacle.enabled = false;
      agent.enabled = true;

      agent.stoppingDistance = attackRange + (unitRadius + enemyRadius);
      agent.destination = attackedTarget.transform.position;

      animationState.currentAnimationState = CURRENT_ANIMATION_STATE.MOVE;
    }
  }

  private void MeleeAttack()
  {
    isAttacking = false;

    if (attackedTarget == null)
    {
      return;
    }

    if (aoe)
    {
      Collider[] colliders = Physics.OverlapSphere(attackedTarget.transform.position, aoeRadius, 1 << 0);

      for (int i = 0; i < colliders.Length; ++i)
      {
        if (colliders[i].GetComponent<Faction>() != null && colliders[i].GetComponent<Health>() != null)
        {
          // Check if is unfriendly unit and has health
          if ((GetComponent<Faction>().faction == Faction.FACTIONS.GOBLINS && colliders[i].GetComponent<Faction>().faction == Faction.FACTIONS.FOREST) ||
              (GetComponent<Faction>().faction == Faction.FACTIONS.FOREST && colliders[i].GetComponent<Faction>().faction == Faction.FACTIONS.GOBLINS))
          {
            if (colliders[i].gameObject == attackedTarget)
            {
              attackedTarget.GetComponent<Health>().ModifyHealth(-attackDamage, transform.position);
            }

            else
            {
              colliders[i].GetComponent<Health>().ModifyHealth(-attackDamage * aoeDmgPct, attackedTarget.transform.position);
            }

            if (GetComponent<StatusEffects>())
            {
              GetComponent<StatusEffects>().AfflictStatusEffects(colliders[i].gameObject);
            }
          }
        }
      }
    }

    else
    {
      attackedTarget.GetComponent<Health>().ModifyHealth(-attackDamage, transform.position);
    }

    if (GetComponent<StatusEffects>())
    {
      GetComponent<StatusEffects>().AfflictStatusEffects(attackedTarget);
    }

    FMODUnity.RuntimeManager.PlayOneShot(attackSound, transform.position);
  }

  private void RangedAttack()
  {
    isAttacking = false;

    if (attackedTarget == null)
    {
      return;
    }

    if (isHealing)
    {
      Heal();

      return;
    }

    // Create the projectile at half height of the unit
    Vector3 projectilePos = transform.position;
    projectilePos.y = (GetComponent<NavMeshAgent>().height / 2) * transform.lossyScale.y;
    GameObject projectile = Instantiate(projectilePrefab, projectilePos, new Quaternion());

    projectile.GetComponent<Projectile>().SetTarget(attackedTarget, attackDamage, GetComponent<StatusEffects>(), aoe, aoeRadius, aoeDmgPct, GetComponent<Faction>().faction);

    FMODUnity.RuntimeManager.PlayOneShot(attackSound, transform.position);
  }

  private void Heal()
  {
    isHealing = false;

    attackedTarget.GetComponent<Health>().ModifyHealth(attackDamage * healPct, Vector3.zero);
    var particleSystem = Instantiate(healParticleSystem, attackedTarget.transform.position, new Quaternion());
    particleSystem.GetComponent<ParticleSystemLifetime>().SetAttachedObject(attackedTarget);
    FMODUnity.RuntimeManager.PlayOneShot(healSound, attackedTarget.transform.position);

    attackedTarget = null;
    detectedTarget = null;
  }

  public void SetAttackTarget(GameObject target, bool healing)
  {
    attackTarget = target;
    detectingEnemies = false;

    // Check if in the middle of an attack
    if (!isAttacking)
    {
      obstacle.enabled = false;
      agent.enabled = true;

      isHealing = healing;

      animationState.currentAnimationState = CURRENT_ANIMATION_STATE.MOVE;
    }

    
    else
    {
      queuedOrder = true;
      queuedOrderPos = attackTarget.transform.position;
      queuedHealing = healing;
    }
  }

  public void SetDetectingEnemies(bool value)
  {
    detectingEnemies = value;

    if (value == false)
    {
      attackTarget = null;
    }
  }

  public void SetAttackMovePosition(Vector3 attackMovePos)
  {
    attackTarget = null;
    detectingEnemies = true;
    isAttackMoveOrder = true;

    // Check if in the middle of an attack
    if (!isAttacking)
    {
      attackMovePosition = attackMovePos;
    }

    else
    {
      queuedOrder = true;
      queuedOrderPos = attackMovePos;
    }
  }

  private float GetEnemyRadius(GameObject enemy)
  {
    // Check if enemy has navmeshagent or navmeshobstacle
    if (enemy.GetComponent<NavMeshAgent>() != null)
    {
      return enemy.GetComponent<NavMeshAgent>().radius * ((transform.lossyScale.x + transform.lossyScale.z) / 2f);
    }

    else if (enemy.GetComponent<NavMeshObstacle>() != null)
    {
      return enemy.GetComponent<NavMeshObstacle>().radius * ((transform.lossyScale.x + transform.lossyScale.z) / 2f);
    }

    else
    {
      Debug.LogError("Attacking a unit without a NavMesh! Targeted unit is " + enemy.name);
      return 0;
    }
  }

  private void LookTowardsTarget(GameObject target)
  {
    Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, GetComponent<NavMeshAgent>().angularSpeed * Mathf.Deg2Rad * Time.deltaTime);
  }

  public void SetUpgradedProperties(UpgradeProperties[] upgradeProperties)
  {
    for (int i = 0; i < upgradeProperties.Length; ++i)
    {
      attackDamage += upgradeProperties[i].damage;
      attacksPerSecond += upgradeProperties[i].attackSpeed;
      attackRange += upgradeProperties[i].attackRange;
      enemyDetectRange += upgradeProperties[i].detectRange;
      aoe = upgradeProperties[i].isAoe;
      aoeRadius = upgradeProperties[i].aoeRadius;
      aoeDmgPct = upgradeProperties[i].aoeDmgPct;

      isHealer = upgradeProperties[i].isHealer;
      healPct = upgradeProperties[i].healPct;
    }

    attackInterval = 1f / attacksPerSecond;
  }

  public void SetBoostedValues(BoostValues boostValues)
  {
    attackDamage += (gameManager.CurrentRound - 1) * boostValues.damageModifier * ogAttackRange;
    attacksPerSecond += (gameManager.CurrentRound - 1) * boostValues.attackSpeedModifier * ogAttackSpeed;
    attackRange += (gameManager.CurrentRound - 1) * boostValues.attackRangeModifier * ogAttackRange;
    enemyDetectRange += (gameManager.CurrentRound - 1) * boostValues.detectRangeModifier * ogDetectRange;

    attackInterval = 1f / attacksPerSecond;
  }

  public void SetSlowAffliction(float attackSlowAmount, float moveSlowAmount)
  {
    attacksPerSecond = normalAttacksPerSecond * (1f + attackSlowAmount);
    agent.speed = normalMoveSpeed * (1f - moveSlowAmount);

    attackInterval = 1f / attacksPerSecond;
  }

  public void SetCurseAffliction(float attackReducAmount)
  {
    attackDamage = normalDamage * (1f - attackReducAmount);
  }

  private void EndAttackAnimation()
  {
    animationState.currentAnimationState = CURRENT_ANIMATION_STATE.IDLE;
  }
}