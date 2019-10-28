using UnityEngine;

[System.Serializable]
public class Poison : StatusEffect
{
  [Header("Poison")]
  public float dps = 0;
  public float statusDuration = 1;

  public void SetAffliction(StatusEffect poison, GameObject afflictedTarget)
  {
    Poison poisonEffect = (Poison)poison;
    statusType = STATUS_EFFECTS.POISON;

    if (active)
    {
      if (dps < poisonEffect.dps)
      {
        dps = poisonEffect.dps;
      }

      if (statusDuration < poisonEffect.statusDuration)
      {
        statusDuration = poisonEffect.statusDuration;
      }
    }

    else
    {
      active = true;

      dps = poisonEffect.dps;
      statusDuration = poisonEffect.statusDuration;

      afflictedUnit = afflictedTarget;
    }
  }

  public void Update()
  {
    if (!active)
    {
      return;
    }

    if (statusDuration >= 0)
    {
      statusDuration -= Time.deltaTime;

      float damage = Time.deltaTime * dps;

      if (statusDuration < 0)
      {
        damage -= statusDuration * dps;
      }

      if (afflictedUnit != null)
      {
        afflictedUnit.GetComponent<Health>().ModifyHealth(-damage);
      }

      else
      {
        active = false;
        return;
      }
    }

    else
    {
      active = false;
      return;
    }
  }
}
