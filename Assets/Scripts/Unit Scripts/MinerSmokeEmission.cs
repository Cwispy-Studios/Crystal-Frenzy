using UnityEngine;

public class MinerSmokeEmission : MonoBehaviour
{
  [SerializeField]
  private ParticleSystem minorSmoke = null, majorSmoke = null, majorSparks = null;

  private Health minerHealth = null;

  private void Awake()
  {
    minorSmoke.Stop();
    majorSmoke.Stop();
    majorSparks.Stop();

    minerHealth = GetComponent<Health>();
  }

  private void Update()
  {
    float healthPct = minerHealth.CurrentHealth / minerHealth.MaxHealth;

    if (healthPct >= 0.6f)
    {
      minorSmoke.Stop();
      majorSmoke.Stop();
      majorSparks.Stop();
    }

    else if (healthPct >= 0.25f && healthPct < 0.6f)
    {
      if (!minorSmoke.isPlaying)
      {
        minorSmoke.Play();
      }
      
      majorSmoke.Stop();
      majorSparks.Stop();
    }

    else
    {
      minorSmoke.Stop();

      if (!majorSmoke.isPlaying)
      {
        majorSmoke.Play();
      }

      if (!majorSparks.isPlaying)
      {
        majorSparks.Play();
      }
    }
  }
}
