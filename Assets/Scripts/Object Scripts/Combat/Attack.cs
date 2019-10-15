using UnityEngine;
using UnityEngine.AI;

public class Attack : MonoBehaviour
{
  [SerializeField]
  private float attackDamage, attacksPerSecond, attackRange, enemyDetectRange;

  private float attackCooldown = 0f;

  private GameObject attackTarget;
  //private GameObject detectedTarget;

  private void Update()
  {
    // If there is no attack target, unit will constantly look out for enemies within their detect range
    if (attackTarget == null)
    {
      Collider[] unitsInRange = Physics.OverlapSphere(transform.position, enemyDetectRange);

      float closestEnemyRange = 9999f;

      for (int i = 0; i < unitsInRange.Length; ++i)
      {
        // Check if the unit in range is an enemy
        if (unitsInRange[i].GetComponent<Faction>() != GetComponent<Faction>())
        {

        }
      }
    }

    if (attackCooldown <= attacksPerSecond)
    {
      attackCooldown += Time.deltaTime;
    }
  }

  private void AttackTarget()
  {

  }
}