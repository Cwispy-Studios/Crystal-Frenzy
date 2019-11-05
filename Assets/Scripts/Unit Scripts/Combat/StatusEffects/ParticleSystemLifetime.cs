using UnityEngine;

public class ParticleSystemLifetime : MonoBehaviour
{
  [SerializeField]
  private float particleSystemLifetime = 0.5f;

  private float OBJECT_LIFETIME = 5f;
  private float timer = 0f;
  private ParticleSystem particles = null;
  private GameObject attachedObject = null;

  private void Awake()
  {
    OBJECT_LIFETIME = particleSystemLifetime * 10f;
    particles = GetComponent<ParticleSystem>();
  }

  private void Update()
  {
    if (attachedObject)
    {
      transform.position = attachedObject.transform.position;
    }

    if (timer >= particleSystemLifetime)
    {
      if (particles.isPlaying)
      {
        particles.Stop();
      }
    }

    if (timer >= OBJECT_LIFETIME)
    {
      Destroy(gameObject);
    }

    timer += Time.deltaTime;
  }

  public void SetAttachedObject(GameObject attached)
  {
    attachedObject = attached;
  }
}
