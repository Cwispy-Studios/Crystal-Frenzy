using UnityEngine;

[System.Serializable]
public class BoostValues
{
  [Header("Health components")]
  // Health components
  public float healthModifier;
  public float regeneratonModifier;

  [Header("Attack components")]
  // Attack components
  public float damageModifier;
  public float attackSpeedModifier;
  public float attackRangeModifier;
  public float detectRangeModifier;

  [Header("Navigation components")]
  // Navigation components
  public float speedModifier;
}
