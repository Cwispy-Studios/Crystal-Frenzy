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

  [SerializeField]
  private COMBATANT_TYPE preferredTarget = COMBATANT_TYPE.NORMAL;
  [SerializeField]
  private GameObject projectilePrefab = null;

  private float attackCooldown = 0;
  private float unitRadius = 0;

  private bool detectingEnemies = true;

  private GameObject attackTarget = null;

  private Vector3 attackMovePosition;
  private bool isAttackMoveOrder = false;

  private void Awake()
  {
    unitRadius = GetComponent<NavMeshAgent>().radius * ((transform.lossyScale.x + transform.lossyScale.z) / 2f);
  }

  private void Start()
  {
    attackCooldown = attacksPerSecond;
  }

  private void Update()
  {
    // As long as unit is not detecting enemies, it is not under an attack move order, so we set it to false
    if (detectingEnemies == false)
    {
      isAttackMoveOrder = false;
    }

    // If there is no attack target and is detecting enemies, unit will constantly look out for enemies within their detect range
    if (attackTarget == null && detectingEnemies)
    {
      Collider[] unitsInRange = Physics.OverlapSphere(transform.position, enemyDetectRange);
      GameObject detectedEnemy = null;

      float closestEnemyRange = 9999f;

      bool preferredTargetTypeInRange = false;

      for (int i = 0; i < unitsInRange.Length; ++i)
      {
        bool unitIsEnemy = CheckUnitIsTargetableEnemy(unitsInRange[i].gameObject);

        // Check if the unit in range is an enemy and has a health component
        if (unitIsEnemy && unitsInRange[i].GetComponent<Health>())
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
                detectedEnemy = unitsInRange[i].gameObject;
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
              detectedEnemy = unitsInRange[i].gameObject;
            }
          }
        }
      }

      // Check if any enemy found
      if (detectedEnemy != null)
      {
        AttackEnemy(detectedEnemy, closestEnemyRange);
      }

      // No enemies found, check if there is an attack order position to move towards to
      else
      {
        if (isAttackMoveOrder)
        {
          GetComponent<NavMeshAgent>().enabled = true;
          GetComponent<NavMeshObstacle>().enabled = false;

          GetComponent<NavMeshAgent>().stoppingDistance = 0;
          GetComponent<NavMeshAgent>().destination = attackMovePosition;
        }
      }
    }

    else if (attackTarget != null)
    {
      float enemyRange = Vector3.Distance(transform.position, attackTarget.transform.position);

      AttackEnemy(attackTarget, enemyRange);
    }

    if (attackCooldown <= attacksPerSecond)
    {
      attackCooldown += Time.deltaTime;
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

  private void AttackEnemy(GameObject enemy, float enemyRange)
  {
    // Try to attack them, check the distance between them. The distance from their position + their radius to your position + your radius 
    // must be smaller than the attack range
    float enemyRadius = GetEnemyRadius(enemy);

    Animator animator = GetComponent<Animator>();

    // If enemy is near enough, try to attack and hold position. Otherwise set the NavMeshAgent to move towards it
    if (enemyRange <= (attackRange + unitRadius + enemyRadius))
    {
      // Rotates the unit towards its target, it does not matter if it is not facing the target yet, it can still attack.
      LookTowardsTarget(enemy);

      if (GetComponent<NavMeshAgent>().enabled)
      {
        GetComponent<NavMeshAgent>().stoppingDistance = 0;
        GetComponent<NavMeshAgent>().destination = transform.position;
      }
      
      GetComponent<NavMeshAgent>().enabled = false;
      GetComponent<NavMeshObstacle>().enabled = true;

      // Check if ready to attack
      if (attackCooldown >= attacksPerSecond)
      {
        // Check if there is projectile
        if (projectilePrefab != null)
        {
          // Create the projectile at half height of the unit
          Vector3 projectilePos = transform.position;
          projectilePos.y = (GetComponent<NavMeshAgent>().height / 2) * transform.lossyScale.y;
          GameObject projectile = Instantiate(projectilePrefab, projectilePos, new Quaternion());

          projectile.GetComponent<Projectile>().SetTarget(enemy, attackDamage);

          if (animator && animator.enabled)
          {
            animator.SetTrigger("Ranged Attack");
          }
          
        }

        // No projectile means attack hits immediately
        else
        {
          enemy.GetComponent<Health>().ModifyHealth(-attackDamage);

          if (animator && animator.enabled)
          {
            animator.SetTrigger("Melee Attack");
          }
        }

        attackCooldown -= attacksPerSecond;
      }
    }

    else if (animator && animator.enabled && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || animator.GetCurrentAnimatorStateInfo(0).IsName("Move"))
    {
      GetComponent<NavMeshAgent>().enabled = true;
      GetComponent<NavMeshObstacle>().enabled = false;

      GetComponent<NavMeshAgent>().stoppingDistance = attackRange + unitRadius + enemyRadius - STOPPING_MARGIN;
      GetComponent<NavMeshAgent>().destination = enemy.transform.position;
    }
  }

  public void SetAttackTarget(GameObject target)
  {
    GetComponent<NavMeshAgent>().enabled = true;
    GetComponent<NavMeshObstacle>().enabled = false;

    attackTarget = target;
    detectingEnemies = false;
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
    attackMovePosition = attackMovePos;
    detectingEnemies = true;
    isAttackMoveOrder = true;
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
    }
  }

  public void SetBoostedValues(BoostValues boostValues)
  {
    attackDamage += (GameManager.CurrentRound - 1) * boostValues.damageModifier * attackDamage;
    attacksPerSecond += (GameManager.CurrentRound - 1) * boostValues.attackSpeedModifier * attacksPerSecond;
    attackRange += (GameManager.CurrentRound - 1) * boostValues.attackRangeModifier * attackRange;
    enemyDetectRange += (GameManager.CurrentRound - 1) * boostValues.detectRangeModifier * enemyDetectRange;
  }
}