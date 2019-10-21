using UnityEngine;

public class CrystalSeeker : MonoBehaviour
{
  private void Update()
  {
    // 2 conditions, either Crystal Seeker dies or it reaches the end
    if (GetComponent<BezierSolution.BezierWalkerWithSpeed>().NormalizedT == 1f)
    {
      // Player wins!
      if (GetComponent<Faction>().faction == Faction.FACTIONS.GOBLINS)
      {
        
      }

      // Enemy captures your node
      else if (GetComponent<Faction>().faction == Faction.FACTIONS.FOREST)
      {

      }
    }

    if (GetComponent<Health>().CurrentHealth == 0)
    {
      // Player's Crystal Seeker died
      if (GetComponent<Faction>().faction == Faction.FACTIONS.GOBLINS)
      {

      }

      // Enemy's Crystal Seeker died
      else if (GetComponent<Faction>().faction == Faction.FACTIONS.FOREST)
      {

      }
    }
  }
}
