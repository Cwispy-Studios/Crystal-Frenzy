using UnityEngine;
using UnityEngine.AI;

public class Projectile : MonoBehaviour
{
  [SerializeField]
  private float speed = 0;                // How fast the projectile travels

  private float damage;                   // How much damage is inflicted upon the target
  private GameObject target;              // Object to travel towards to
  private Vector3 targetPos;              // Cache position in case target dies
  private StatusEffects statusEffects;    // The status effects of the object who shot the projectile
  private float targetRadius;
  private bool aoe;
  private float aoeRadius;
  private float aoeDmgPct;
  private Faction.FACTIONS faction;

  [FMODUnity.EventRef]
  public string hitSoundEvent = "";
  FMOD.Studio.EventInstance hitSound;
  FMOD.Studio.PARAMETER_ID hitParamaterId;

  private void Start()
  {
    hitSound = FMODUnity.RuntimeManager.CreateInstance(hitSoundEvent);

    FMODUnity.RuntimeManager.AttachInstanceToGameObject(hitSound, transform, GetComponent<Rigidbody>());

    FMOD.Studio.EventDescription hitEventDescription;
    hitSound.getDescription(out hitEventDescription);
    FMOD.Studio.PARAMETER_DESCRIPTION hitEventParameterDescription;
    hitEventDescription.getParameterDescriptionByName("hitType", out hitEventParameterDescription);
    hitParamaterId = hitEventParameterDescription.id;
  }

  private void Update()
  {
    Vector3 direction = targetPos - transform.position;

    float distanceCovered = speed * Time.deltaTime;

    // Check if the distance left between target and projectile is less than distance covered this frame
    if (direction.sqrMagnitude <= (distanceCovered * distanceCovered) + (targetRadius * targetRadius))
    {
      // Check if target is stil alive
      if (target != null)
      {
        hitSound.setParameterByID(hitParamaterId, 0);

        if (aoe)
        {
          Collider[] colliders = Physics.OverlapSphere(transform.position, aoeRadius, 1 << 0);

          for (int i = 0; i < colliders.Length; ++i)
          {
            if (colliders[i].GetComponent<Faction>() != null && colliders[i].GetComponent<Health>() != null)
            {
              // Check if is unfriendly unit and has health
              if ((faction == Faction.FACTIONS.GOBLINS && colliders[i].GetComponent<Faction>().faction == Faction.FACTIONS.FOREST) ||
                  (faction == Faction.FACTIONS.FOREST && colliders[i].GetComponent<Faction>().faction == Faction.FACTIONS.GOBLINS))
              {
                if (colliders[i].gameObject == target)
                {
                  colliders[i].GetComponent<Health>().ModifyHealth(-damage, transform.position);
                }

                else
                {
                  colliders[i].GetComponent<Health>().ModifyHealth(-damage * aoeDmgPct, transform.position);
                }

                if (statusEffects)
                {
                  statusEffects.AfflictStatusEffects(colliders[i].gameObject);
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

      else
      {
        hitSound.setParameterByID(hitParamaterId, 1);
      }

      FMODUnity.RuntimeManager.AttachInstanceToGameObject(hitSound, transform, GetComponent<Rigidbody>());
      hitSound.start();
      hitSound.release();

      // Destroy projectile since it has reached destination
      Destroy(gameObject);
    }

    else
    {
      // Move towards the target
      transform.Translate(direction.normalized * distanceCovered, Space.World);
      transform.LookAt(targetPos);

      // For arrows
      transform.Rotate(0, 90, 0);

      // If target is not dead yet, update the target position
      // Otherwise projectile just hits the last position of the target
      if (target != null)
      {
        targetPos = target.transform.position;
      }
    }
  }

  public void SetTarget(GameObject setTarget, float setDamage, StatusEffects statusEffects, bool isAoe, float aoeRad, float aoeDamagePct, Faction.FACTIONS projectileFaction)
  {
    target = setTarget;
    damage = setDamage;
    this.statusEffects = statusEffects;
    aoe = isAoe;
    aoeRadius = aoeRad;
    aoeDmgPct = aoeDamagePct;
    faction = projectileFaction;

    targetPos = target.transform.position;

    targetRadius = target.GetComponent<NavMeshObstacle>().radius * target.transform.lossyScale.x;
  }
}
