using UnityEngine;

public class RagdollBody : MonoBehaviour
{
  private float LIFETIME = 5f;
  private float lifetimeCountdown = 0;

  private void Update()
  {
    if (lifetimeCountdown < LIFETIME)
    {
      lifetimeCountdown += Time.deltaTime;
    }

    else
    {
      Destroy(gameObject);
    }
  }
}
