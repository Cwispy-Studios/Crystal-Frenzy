using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class Curse : StatusEffect
{
  [Header("Curse")]
  public float attackReducPct = 0.2f;

  public float statusDuration = 1f;

  public Curse(GameObject unit)
  {
    Active = false;
    afflictedUnit = unit;
  }

  public void SetAffliction(StatusEffect curse)
  {
    Curse curseEffect = (Curse)curse;
    statusType = STATUS_EFFECTS.POISON;

    if (Active)
    {
      if (attackReducPct < curseEffect.attackReducPct)
      {
        attackReducPct = curseEffect.attackReducPct;
      }

      if (statusDuration < curseEffect.statusDuration)
      {
        statusDuration = curseEffect.statusDuration;
      }
    }

    else
    {
      Active = true;

      attackReducPct = curseEffect.attackReducPct;
      statusDuration = curseEffect.statusDuration;
    }

    afflictedUnit.GetComponent<Attack>().SetCurseAffliction(attackReducPct);
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
    }

    else
    {
      Active = false;
      afflictedUnit.GetComponent<Attack>().SetCurseAffliction(0);

      return;
    }
  }
}
