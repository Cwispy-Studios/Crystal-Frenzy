using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class Slow : StatusEffect
{
  [Header("Slow")]
  public float attackSlowPct = 0.2f;
  public float moveSlowPct = 0.2f;

  public float statusDuration = 1f;

  public Slow(GameObject unit)
  {
    Active = false;
    afflictedUnit = unit;
  }

  public void SetAffliction(StatusEffect slow)
  {
    Slow slowEffect = (Slow)slow;
    statusType = STATUS_EFFECTS.POISON;

    if (Active)
    {
      if (attackSlowPct < slowEffect.attackSlowPct)
      {
        attackSlowPct = slowEffect.attackSlowPct;
      }

      if (moveSlowPct < slowEffect.moveSlowPct)
      {
        moveSlowPct = slowEffect.moveSlowPct;
      }

      if (statusDuration < slowEffect.statusDuration)
      {
        statusDuration = slowEffect.statusDuration;
      }
    }

    else
    {
      Active = true;

      attackSlowPct = slowEffect.attackSlowPct;
      moveSlowPct = slowEffect.moveSlowPct;
      statusDuration = slowEffect.statusDuration;
    }

    afflictedUnit.GetComponent<Attack>().SetSlowAffliction(attackSlowPct, moveSlowPct);
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
      afflictedUnit.GetComponent<Attack>().SetSlowAffliction(0, 0);

      return;
    }
  }
}
