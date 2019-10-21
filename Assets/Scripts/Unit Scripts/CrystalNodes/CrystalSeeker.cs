using UnityEngine;

public class CrystalSeeker : MonoBehaviour
{
  private GameManager gameManager;

  private void Awake()
  {
    gameManager = FindObjectOfType<GameManager>();
  }

  private void Update()
  {
    // 2 conditions, either Crystal Seeker dies or it reaches the end
    if (GetComponent<BezierSolution.BezierWalkerWithSpeed>().TargetReached())
    {
      // Player wins!
      if (GetComponent<Faction>().faction == Faction.FACTIONS.GOBLINS)
      {
        gameManager.EscortWin();
        Destroy(gameObject);
      }

      // Enemy captures your node
      else if (GetComponent<Faction>().faction == Faction.FACTIONS.FOREST)
      {

      }
    }
  }

  private void OnDestroy()
  {
    // Player's Crystal Seeker died
    if (GetComponent<Faction>().faction == Faction.FACTIONS.GOBLINS)
    {
      gameManager.EscortLose();
    }

    // Enemy's Crystal Seeker died
    else if (GetComponent<Faction>().faction == Faction.FACTIONS.FOREST)
    {

    }
  }
}
