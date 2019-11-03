using UnityEngine;

public class RagdollBody : MonoBehaviour
{
  private float LIFETIME = 5f;
  private float lifetimeCountdown = 0;

  private float shrinkSpeed = 1.5f;

  private void Update()
  {
    if (lifetimeCountdown < LIFETIME)
    {
      lifetimeCountdown += Time.deltaTime;
    }

    else
    {
      transform.localScale -= Vector3.one * shrinkSpeed * Time.deltaTime;

      //GetComponentInChildren<Rigidbody>().ResetCenterOfMass;

      if (transform.localScale.x <= 0)
      {
        Destroy(gameObject);
      }
    }
  }
}
