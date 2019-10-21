using UnityEngine;
using UnityEngine.AI;

public class Attack : MonoBehaviour
{
  [SerializeField]
  private int attackDamage = 0;
  [SerializeField]
  private float attacksPerSecond = 1f, attackRange = 1f, enemyDetectRange = 1f;
  [SerializeField]
  private COMBATANT_TYPE preferredTarget = COMBATANT_TYPE.NORMAL;

  private float attackCooldown = 0;
  private float unitRadius = 0;

  private bool detectingEnemies = true;

  private GameObject attackTarget = null;

  private Vector3 attackMovePosition;
  private bool isAttackMoveOrder = false;

  private void Awake()
  {
    attackCooldown = attacksPerSecond;

    unitRadius = GetComponent<NavMeshAgent>().radius * ((transform.lossyScale.x + transform.lossyScale.z) / 2f);
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
        // Try to attack them, check the distance between them. The distance from their position + their radius to your position + your radius 
        // must be smaller than the attack range
        float enemyRadius = 0;

        // Check if enemy has navmeshagent or navmeshobstacle
        if (detectedEnemy.GetComponent<NavMeshAgent>() != null)
        {
          enemyRadius = detectedEnemy.GetComponent<NavMeshAgent>().radius * ((transform.lossyScale.x + transform.lossyScale.z) / 2f);
        }

        else if (detectedEnemy.GetComponent<NavMeshObstacle>() != null)
        {
          enemyRadius = detectedEnemy.GetComponent<NavMeshObstacle>().radius * ((transform.lossyScale.x + transform.lossyScale.z) / 2f);
        }

        else
        {
          Debug.LogError("Attacking a unit without a NavMesh! Targeted unit is " + detectedEnemy.name);
        }

        // If enemy is near enough, try to attack and hold position. Otherwise set the NavMeshAgent to move towards it
        if (closestEnemyRange <= (attackRange + unitRadius + enemyRadius))
        {
          // Rotates the unit towards its target, it does not matter if it is not facing the target yet, it can still attack.
          Vector3 direction = detectedEnemy.transform.position - transform.position;
          Quaternion toRotation = Quaternion.FromToRotation(transform.forward, direction);
          transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, GetComponent<NavMeshAgent>().angularSpeed * Mathf.Deg2Rad * Time.deltaTime);

          GetComponent<NavMeshAgent>().stoppingDistance = 0;
          GetComponent<NavMeshAgent>().destination = transform.position;

          if (attackCooldown >= attacksPerSecond)
          {
            detectedEnemy.GetComponent<Health>().ModifyHealth(-attackDamage);
            attackCooldown -= attacksPerSecond;
          }
        }

        else
        {
          GetComponent<NavMeshAgent>().stoppingDistance = attackRange + unitRadius + enemyRadius;
          GetComponent<NavMeshAgent>().destination = detectedEnemy.transform.position;
        }
      }

      // No enemies found, check if there is an attack order position to move towards to
      else
      {
        if (isAttackMoveOrder)
        {
          GetComponent<NavMeshAgent>().stoppingDistance = 0;
          GetComponent<NavMeshAgent>().destination = attackMovePosition;
        }
      }
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

  private void AttackTarget()
  {

  }

  public void SetDetectingEnemies(bool value)
  {
    detectingEnemies = value;
  }

  public void SetAttackMovePosition(Vector3 attackMovePos)
  {
    attackMovePosition = attackMovePos;
    detectingEnemies = true;
    isAttackMoveOrder = true;
  }
}