using UnityEngine;

public class Faction : MonoBehaviour
{
  public enum FACTIONS
  {
    NONE = 0,
    GOBLINS,
    FOREST,
    NEUTRAL
  }

  public FACTIONS faction;

  public void SetSide(FACTIONS faction_)
  {
    faction = faction_;
  }
}
