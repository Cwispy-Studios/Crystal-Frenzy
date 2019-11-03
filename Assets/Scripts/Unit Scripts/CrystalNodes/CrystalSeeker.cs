﻿using UnityEngine;

public class CrystalSeeker : MonoBehaviour
{
  private GameManager gameManager;
  private bool crystalSeekerReachedTarget = false;

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
        crystalSeekerReachedTarget = true;
        GetComponent<BezierSolution.BezierWalkerWithSpeed>().enabled = false;
        // Start win cut scene
        gameManager.EscortWinCutscene(gameObject);
        enabled = false;
        //gameManager.EscortWin();
        //Destroy(gameObject);
      }

      // Enemy captures your node
      else if (GetComponent<Faction>().faction == Faction.FACTIONS.FOREST)
      {
        crystalSeekerReachedTarget = true;
        gameManager.DefenseLose();
        Destroy(gameObject);
      }
    }

    // Defense phase
    else if (GameManager.CurrentPhase == PHASES.DEFENSE)
    {
      musicEmitter.SetParameter("Enemy Crystal Seeker Health", GetComponent<Health>().CurrentHealth / GetComponent<Health>().MaxHealth);

      // All your units die
      if (GameManager.resourceManager.ArmySize == 0)
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

        // Set the miner manager health to 10%
        GameManager.minerManager.MinerDestroyed();
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

  public void SetBoostedValues(BoostValues boostValues)
  {
    GetComponent<BezierSolution.BezierWalkerWithSpeed>().speed += (GameManager.CurrentRound - 1) * boostValues.speedModifier * GetComponent<BezierSolution.BezierWalkerWithSpeed>().speed;
  }
}
