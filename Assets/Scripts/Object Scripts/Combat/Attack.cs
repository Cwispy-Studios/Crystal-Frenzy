using UnityEngine;
using UnityEngine.AI;

public class Attack : MonoBehaviour
{
  [SerializeField]
  private int attackDamage = 0;
  [SerializeField]
  private float attacksPerSecond = 1f, attackRange = 1f, enemyDetectRange = 1f;

  private float attackCooldown = 0;
  private float unitRadius = 0;

  private bool detectingEnemies = true;

  private GameObject attackTarget = null;
  //private GameObject detectedTarget;

  private void Awake()
  {
    attackCooldown = attacksPerSecond;

    unitRadius = GetComponent<NavMeshAgent>().radius * ((transform.lossyScale.x + transform.lossyScale.z) / 2f);
  }

  private void Update()
  {
    // If there is no attack target and is detecting enemies, unit will constantly look out for enemies within their detect range
    if (attackTarget == null && detectingEnemies)
    {
      Collider[] unitsInRange = Physics.OverlapSphere(transform.position, enemyDetectRange);
      GameObject detectedEnemy = null;

      float closestEnemyRange = 9999f;

      for (int i = 0; i < unitsInRange.Length; ++i)
      {
        bool unitIsEnemy = CheckUnitIsTargetableEnemy(unitsInRange[i].gameObject);

        // Check if the unit in range is an enemy and has a health component
        if (unitIsEnemy && unitsInRange[i].GetComponent<Health>())
        {
          float distance = Vector3.Distance(transform.position, unitsInRange[i].transform.position);

          if (distance < closestEnemyRange)
          {
            closestEnemyRange = distance;
            detectedEnemy = unitsInRange[i].gameObject;
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
          enemyRadius = GetComponent<NavMeshObstacle>().radius * ((transform.lossyScale.x + transform.lossyScale.z) / 2f);
        }

        else
        {
          Debug.LogError("Attacking a unit without a NavMesh! Targeted unit is " + detectedEnemy.name);
        }

        // If enemy is near enough, try to attack. Otherwise set the NavMeshAgent to move towards it
        if (closestEnemyRange <= (attackRange + unitRadius + enemyRadius))
        {
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
}