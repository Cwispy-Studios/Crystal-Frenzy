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

  public bool crystalSelected { get; private set; }

  private void Awake()
  {
    crystalSelected = false;
  }

  private void Update()
  {
    if (crystalSelected)
    {
      PathIndicatorIllumination();
    }
  }

  private void PathIndicatorIllumination()
  {
    ParticleSystem particleSystem = crystalPath.GetComponentInChildren<ParticleSystem>();
    var emittor = particleSystem.emission;

    if (GetComponent<Selectable>().selectStatus == Selectable.SELECT_STATUS.SELECTED)
    {
      emittor.enabled = true;
    }

    else
    {
      emittor.enabled = false;
      particleSystem.Clear();
    }
  }

  public void SetCrystalTarget(GameObject target)
  {
    // Check if target is a crystal and is not self
    // Crystals are identified if they have CrystalSeekerSpawner
    if (target != gameObject && target.GetComponent<CrystalOrder>() != null)
    {
      // Check against the connected nodes if that crystal is connected with this object and if crystal does not already belong to the player
      // Also saves the crystal path if it is valid
      if (GetComponent<CrystalNode>().CheckCrystalIsValid(target, ref crystalTarget, ref crystalPath))
      {
        crystalSelected = true;
      }

      else
      {
        crystalSelected = false;
      }
    }
  }

  public void SetDefendingCrystalTarget(GameObject target)
  {
    crystalTarget = target;
  }

  public GameObject SpawnCrystalSeeker()
  {
    GameObject crystalSeeker = null;

    BezierSpline crystalPathSpline = crystalPath.GetComponent<BezierSpline>();
    BezierWalkerWithSpeed bezierWalker = null;

    if (GameManager.CurrentPhase == PHASES.ESCORT)
    {
      crystalSeeker = Instantiate(minerPrefab);
      bezierWalker = crystalSeeker.GetComponent<BezierWalkerWithSpeed>();

      bezierWalker.spline = crystalPathSpline;
    }

    else if (GameManager.CurrentPhase == PHASES.DEFENSE)
    {
      crystalSeeker = Instantiate(bushPrefab);
      bezierWalker = crystalSeeker.GetComponent<BezierWalkerWithSpeed>();

      // Create an inverted spline for the enemy crystal seeker to travel along
      BezierSpline invertedSpline = new GameObject().AddComponent<BezierSpline>();
      invertedSpline.Initialize(crystalPathSpline.Count);

      for (int i = 0, opp = crystalPathSpline.Count - 1; i < crystalPathSpline.Count; ++i, --opp)
      {
        invertedSpline[i].position = crystalPathSpline[opp].position;
        invertedSpline[i].handleMode = BezierPoint.HandleMode.Mirrored;
        invertedSpline[i].followingControlPointLocalPosition = -crystalPathSpline[opp].followingControlPointLocalPosition;
        invertedSpline[i].precedingControlPointLocalPosition = -crystalPathSpline[opp].precedingControlPointLocalPosition;
      }

      bezierWalker.spline = invertedSpline;
    }

    
    bezierWalker.SetStartAndEndPoints(1, bezierWalker.spline.Count - 2);

    return crystalSeeker;
  }

  public void ResetCrystalSelection()
  {
    TurnOffPathIllumination();
    crystalTarget = null;
    crystalPath = null;
    crystalSelected = false;
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
