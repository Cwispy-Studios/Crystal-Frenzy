using UnityEngine;

public class BuildingManager : MonoBehaviour
{
  [HideInInspector]
  public bool
    archeryRangeConstructed = false, archeryRangeInControl = false,
    blacksmithConstructed = false, blacksmithInControl = false,
    mageTowerConstructed = false, mageTowerInControl = false,
    brawlPitConstructed = false, brawlPitInControl = false;
}
