using UnityEngine;

public class Projectile : MonoBehaviour
{
  [SerializeField]
  private float speed = 0;        // How fast the projectile travels

  private int damage;         // How much damage is inflicted upon the target
  private GameObject target;  // Object to travel towards to
  private Vector3 targetPos;  // Cache position in case target dies

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
        // Damage the target
        target.GetComponent<Health>().ModifyHealth(-damage);
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

  public void SetTarget(GameObject setTarget, int setDamage)
  {
    target = setTarget;
    damage = setDamage;

    targetPos = target.transform.position;
  }
}
