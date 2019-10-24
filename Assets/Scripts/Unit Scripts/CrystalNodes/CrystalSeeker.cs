using UnityEngine;

public class CrystalSeeker : MonoBehaviour
{
  private GameManager gameManager;
  private bool crystalSeekerReachedTarget = false;

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
        crystalSeekerReachedTarget = true;
        gameManager.EscortWin();
        Destroy(gameObject);
      }

      // Enemy captures your node
      else if (GetComponent<Faction>().faction == Faction.FACTIONS.FOREST)
      {
        crystalSeekerReachedTarget = true;
        gameManager.DefenseLose();
        Destroy(gameObject);
      }
    }
  }

  private void OnDestroy()
  {
    if (crystalSeekerReachedTarget == false)
    {
      // Player's Crystal Seeker died
      if (GetComponent<Faction>().faction == Faction.FACTIONS.GOBLINS)
      {
        gameManager.EscortLose();
      }

      // Enemy's Crystal Seeker died
      else if (GetComponent<Faction>().faction == Faction.FACTIONS.FOREST)
      {
        gameManager.DefenseWin();
      }
    }
  }

  public void SetUpgradedProperties(UpgradeProperties[] upgradeProperties)
  {
    for (int i = 0; i < upgradeProperties.Length; ++i)
    {
      GetComponent<BezierSolution.BezierWalkerWithSpeed>().speed += upgradeProperties[i].speed;
    }
  }
}
