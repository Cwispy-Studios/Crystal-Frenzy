using UnityEngine;

[System.Serializable]
public class Poison : StatusEffect
{
  [Header("Poison")]
  public float dps = 0;
  public float statusDuration = 1;

  public Poison(GameObject unit)
  {
    Active = false;
    afflictedUnit = unit;
  }

  public void SetAffliction(StatusEffect poison)
  {
    Poison poisonEffect = (Poison)poison;
    statusType = STATUS_EFFECTS.POISON;

    if (Active)
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
      Active = true;

      dps = poisonEffect.dps;
      statusDuration = poisonEffect.statusDuration;
    }
  }

  public void Update()
  {
    if (!Active)
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
        afflictedUnit.GetComponent<Health>().ModifyHealth(-damage, Vector3.zero);
      }

      else
      {
        Active = false;
        return;
      }
    }

    else
    {
      Active = false;
      return;
    }
  }
}
