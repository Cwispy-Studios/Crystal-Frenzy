using UnityEngine;

public class EnemyBoostPerRound : MonoBehaviour
{
  [SerializeField]
  private BoostValues boostValues = null;

  private void Awake()
  {
    SetBoostedValues();
  }

  public void SetBoostedValues()
  {
    if (GetComponent<Health>())
    {
      GetComponent<Health>().SetBoostedValues(boostValues);
    }

    if (GetComponent<Attack>())
    {
      GetComponent<Attack>().SetBoostedValues(boostValues);
    }

    if (GetComponent<UnitOrder>())
    {
      GetComponent<UnitOrder>().SetBoostedValues(boostValues);
    }

    if (GetComponent<CrystalSeeker>())
    {
      GetComponent<CrystalSeeker>().SetBoostedValues(boostValues);
    }

    if (GetComponent<StatusEffects>())
    {

    }
  }
}
