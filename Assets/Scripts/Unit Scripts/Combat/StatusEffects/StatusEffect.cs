using UnityEngine;

public class StatusEffect
{
  public bool hasEffect = false;
  public bool Active { get; protected set; } = false;

  [HideInInspector]
  public STATUS_EFFECTS statusType;
  protected GameObject afflictedUnit = null;

  public void Reset()
  {
    Active = false;
    afflictedUnit = null;
  }
}
