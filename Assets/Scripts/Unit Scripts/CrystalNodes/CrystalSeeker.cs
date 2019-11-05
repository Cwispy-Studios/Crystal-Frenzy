using UnityEngine;

public class CrystalSeeker : MonoBehaviour
{
  private GameManager gameManager;

  private FMODUnity.StudioEventEmitter musicEmitter;

  private void Awake()
  {
    gameManager = FindObjectOfType<GameManager>();

    musicEmitter = Camera.main.GetComponent<FMODUnity.StudioEventEmitter>();
  }

  private void Update()
  {
    // 2 conditions, either Crystal Seeker dies or it reaches the end
    if (GetComponent<BezierSolution.BezierWalkerWithSpeed>().TargetReached())
    {
      // Player wins!
      if (GetComponent<Faction>().faction == Faction.FACTIONS.GOBLINS)
      {
        GetComponent<BezierSolution.BezierWalkerWithSpeed>().enabled = false;

        // Start win cut scene
        gameManager.EscortWinCutscene(gameObject);
        enabled = false;
      }

      // Enemy captures your node
      else if (GetComponent<Faction>().faction == Faction.FACTIONS.FOREST)
      {
        GetComponent<BezierSolution.BezierWalkerWithSpeed>().enabled = false;

        // Start lose cut scene
        gameManager.DefenseLoseCutscene(gameObject);
        enabled = false;
      }
    }

    // Defense phase, update music based on bush health and check your army size
    else if (gameManager.CurrentPhase == PHASES.DEFENSE)
    {
      musicEmitter.SetParameter("Enemy Crystal Seeker Health", GetComponent<Health>().CurrentHealth / GetComponent<Health>().MaxHealth);

      // All your units die
      if (GameManager.resourceManager.ArmySize == 0)
      {
        GetComponent<BezierSolution.BezierWalkerWithSpeed>().enabled = false;
        GameManager.bushManager.UpdateProgressPct(1);
        // Start lose cut scene
        gameManager.DefenseLoseCutscene(gameObject);
        enabled = false;
      }
    }
  }

  public void CrystalSeekerDead(GameObject ragdoll)
  {
    // Player's Crystal Seeker died
    if (GetComponent<Faction>().faction == Faction.FACTIONS.GOBLINS)
    {
      // Start win cut scene
      gameManager.EscortLoseCutscene(ragdoll);
    }

    // Enemy's Crystal Seeker died
    else if (GetComponent<Faction>().faction == Faction.FACTIONS.FOREST)
    {
      gameManager.DefenseWinCutscene(ragdoll);
    }
  }

  public void SetUpgradedProperties(UpgradeProperties[] upgradeProperties)
  {
    for (int i = 0; i < upgradeProperties.Length; ++i)
    {
      GetComponent<BezierSolution.BezierWalkerWithSpeed>().speed += upgradeProperties[i].speed;
    }
  }

  public void SetBoostedValues(BoostValues boostValues)
  {
    GetComponent<BezierSolution.BezierWalkerWithSpeed>().speed += (gameManager.CurrentRound - 1) * boostValues.speedModifier * GetComponent<BezierSolution.BezierWalkerWithSpeed>().speed;
  }
}
