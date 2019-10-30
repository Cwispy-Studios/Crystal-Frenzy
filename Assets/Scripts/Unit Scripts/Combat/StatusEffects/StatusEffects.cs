using UnityEngine;

public enum STATUS_EFFECTS
{
  POISON = 0,
  SLOW,
  CURSED,
  LAST
}

public class StatusEffects : MonoBehaviour
{
  public Poison poisonEffect = null;
  public Slow slowEffect = null;
  public Curse curseEffect = null;

  public void AfflictStatusEffects(GameObject target)
  {
    if (target.GetComponent<Afflictable>())
    {
      if (poisonEffect.hasEffect)
      {
        target.GetComponent<Afflictable>().posionAffliction.SetAffliction(poisonEffect);
      }

      if (slowEffect.hasEffect)
      {
        target.GetComponent<Afflictable>().slowAffliction.SetAffliction(slowEffect);
      }

      if (curseEffect.hasEffect)
      {
        target.GetComponent<Afflictable>().curseAffliction.SetAffliction(curseEffect);
      }
    }
  }

  public void SetUpgradedProperties(UpgradeProperties[] upgradeProperties)
  {
    for (int i = 0; i < upgradeProperties.Length; ++i)
    {
      slowEffect.hasEffect = upgradeProperties[i].addsSlow;
      curseEffect.hasEffect = upgradeProperties[i].addsCurse;

      slowEffect.attackSlowPct = upgradeProperties[i].attackSlowPct;
      slowEffect.moveSlowPct = upgradeProperties[i].moveSlowPct;
      slowEffect.statusDuration = upgradeProperties[i].slowDuration;

      curseEffect.attackReducPct = upgradeProperties[i].attackReducPct;
      curseEffect.statusDuration = upgradeProperties[i].curseDuration;
    }
  }
}