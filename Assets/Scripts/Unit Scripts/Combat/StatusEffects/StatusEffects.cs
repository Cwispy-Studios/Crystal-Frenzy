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

  public void AfflictStatusEffects(GameObject target)
  {
    if (target.GetComponent<Afflictable>())
    {
      if (poisonEffect.hasEffect)
      {
        target.GetComponent<Afflictable>().posionAffliction.SetAffliction(poisonEffect);
      }
    }
  }
}