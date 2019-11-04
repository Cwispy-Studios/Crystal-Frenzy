using BezierSolution;
using UnityEngine;

public class CrystalSeekerSpawner : MonoBehaviour
{
  [SerializeField]
  private GameObject minerPrefab = null, bushPrefab = null;

  private GameObject crystalTarget = null;
  public GameObject CrystalTarget
  {
    get
    {
      return crystalTarget;
    }
  }

  private GameObject crystalPath = null;

  public bool CrystalSelected { get; private set; }

  private GameManager gameManager;

  private void Awake()
  {
    CrystalSelected = false;
    gameManager = FindObjectOfType<GameManager>();
  }

  private void PathIndicatorIllumination()
  {
    ParticleSystem particleSystem = crystalPath.GetComponentInChildren<ParticleSystem>();
    var emittor = particleSystem.emission;

    emittor.enabled = true;

    //if (GetComponent<Selectable>().selectStatus == Selectable.SELECT_STATUS.SELECTED)
    //{
    //  emittor.enabled = true;
    //}

    //else
    //{
    //  emittor.enabled = false;
    //  particleSystem.Clear();
    //}
  }

  public void SetCrystalTarget(GameObject target)
  {
    // Check if target is not self
    if (target != gameObject)
    {
      // Check against the connected nodes if that crystal is connected with this object and if crystal does not already belong to the player
      // Also saves the crystal path if it is valid
      if (GetComponent<CrystalNode>().CheckCrystalIsValid(target, ref crystalTarget, ref crystalPath))
      {
        CrystalSelected = true;

        ParticleSystem particleSystem = crystalPath.GetComponentInChildren<ParticleSystem>();
        var emittor = particleSystem.emission;

        emittor.enabled = true;
      }

      else
      {
        CrystalSelected = false;
      }
    }
  }

  public void SetDefendingCrystalTarget(GameObject target)
  {
    crystalTarget = target;

    // We need to retrieve the crystal spline from the target the enemy is attacking.
    crystalPath = target.GetComponent<CrystalNode>().RetrieveSpline(gameObject);
  }

  public GameObject SpawnCrystalSeeker()
  {
    GameObject crystalSeeker = null;

    BezierSpline crystalPathSpline = crystalPath.GetComponent<BezierSpline>();
    BezierWalkerWithSpeed bezierWalker = null;

    if (gameManager.CurrentPhase == PHASES.ESCORT)
    {
      crystalSeeker = Instantiate(minerPrefab);
    }

    else if (gameManager.CurrentPhase == PHASES.DEFENSE)
    {
      crystalSeeker = Instantiate(bushPrefab);
    }

    bezierWalker = crystalSeeker.GetComponent<BezierWalkerWithSpeed>();
    bezierWalker.spline = crystalPathSpline;

    if (gameManager.CurrentPhase == PHASES.ESCORT)
    {
      bezierWalker.SetStartAndEndPoints(1, bezierWalker.spline.Count - 2, true);
    }

    else if (gameManager.CurrentPhase == PHASES.DEFENSE)
    {
      bezierWalker.SetStartAndEndPoints(bezierWalker.spline.Count - 2, 1, false);
    }

    return crystalSeeker;
  }

  public void ResetCrystalSelection()
  {
    TurnOffPathIllumination();
    crystalTarget = null;
    crystalPath = null;
    CrystalSelected = false;
  }

  private void TurnOffPathIllumination()
  {
    if (crystalPath != null)
    {
      ParticleSystem particleSystem = crystalPath.GetComponentInChildren<ParticleSystem>();
      var emittor = particleSystem.emission;

      emittor.enabled = false;
      particleSystem.Clear();
    }
  }
}
