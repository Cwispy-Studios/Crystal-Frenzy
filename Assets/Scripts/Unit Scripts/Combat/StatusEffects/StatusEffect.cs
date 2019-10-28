using UnityEngine;

public class StatusEffect
{
  public bool hasEffect = false;
  public bool active { get; protected set; } = false;

  [HideInInspector]
  public STATUS_EFFECTS statusType;
  protected GameObject afflictedUnit = null;

  public void Reset()
  {
    active = false;
    afflictedUnit = null;
  }
}
