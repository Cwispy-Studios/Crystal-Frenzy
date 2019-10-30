using UnityEngine;

public class Projectile : MonoBehaviour
{
  [SerializeField]
  private float speed = 0;                // How fast the projectile travels

  private float damage;                   // How much damage is inflicted upon the target
  private GameObject target;              // Object to travel towards to
  private Vector3 targetPos;              // Cache position in case target dies
  private StatusEffects statusEffects;    // The status effects of the object who shot the projectile
  private bool aoe;
  private float aoeRadius;

  private void Update()
  {
    Vector3 direction = targetPos - transform.position;

    float distanceCovered = speed * Time.deltaTime;

    // Check if the distance left between target and projectile is less than distance covered this frame
    if (direction.sqrMagnitude <= (distanceCovered * distanceCovered))
    {
      // Check if target is stil alive
      if (target != null)
      {
        if (aoe)
        {
          int layerMask = 0;

          Collider[] colliders = Physics.OverlapSphere(transform.position, aoeRadius, ~layerMask);

          for (int i = 0; i < colliders.Length; ++i)
          {
            if (colliders[i].GetComponent<Faction>() != null && colliders[i].GetComponent<Health>() != null)
            {
              // Check if is unfriendly unit and has health
              if ((GetComponent<Faction>().faction == Faction.FACTIONS.GOBLINS && colliders[i].GetComponent<Faction>().faction == Faction.FACTIONS.FOREST) ||
                  (GetComponent<Faction>().faction == Faction.FACTIONS.FOREST && colliders[i].GetComponent<Faction>().faction == Faction.FACTIONS.GOBLINS))
              {
                colliders[i].GetComponent<Health>().ModifyHealth(-damage, transform.position);

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
          // Damage the target
          target.GetComponent<Health>().ModifyHealth(-damage, transform.position);

          if (statusEffects)
          {
            statusEffects.AfflictStatusEffects(target);
          }
        }        
      }

      // Destroy projectile since it has reached destination
      Destroy(gameObject);
    }

    else
    {
      // Move towards the target
      transform.Translate(direction.normalized * distanceCovered, Space.World);
      transform.LookAt(targetPos);

      // If target is not dead yet, update the target position
      // Otherwise projectile just hits the last position of the target
      if (target != null)
      {
        targetPos = target.transform.position;
      }
    }
  }

  public void SetTarget(GameObject setTarget, float setDamage, StatusEffects statusEffects, bool isAoe = false, float aoeRad = 0)
  {
    target = setTarget;
    damage = setDamage;
    this.statusEffects = statusEffects;
    aoe = isAoe;
    aoeRadius = aoeRad;

    targetPos = target.transform.position;
  }
}
