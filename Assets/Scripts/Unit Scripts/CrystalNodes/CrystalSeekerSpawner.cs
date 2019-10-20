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
    if (crystalTarget != null && crystalPath != null)
    {
      crystalSelected = true;

      PathIndicatorIllumination();
    }

    else
    {
      crystalSelected = false;
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
      GetComponent<CrystalNode>().CheckCrystalIsValid(target, ref crystalTarget, ref crystalPath);
    }
  }

  public GameObject SpawnCrystalSeeker()
  {
    GameObject crystalSeeker = null;

    if (GameManager.CurrentPhase == PHASES.ESCORT)
    {
      crystalSeeker = Instantiate(minerPrefab);
    }
    
    //else if (GameManager.currentPhase == PHASES.DEFENSE)
    //{
    //  crystalSeeker = Instantiate(bushPrefab);
    //}

    BezierSpline crystalPathSpline = crystalPath.GetComponent<BezierSpline>();
    BezierWalkerWithSpeed bezierWalker = crystalSeeker.GetComponent<BezierWalkerWithSpeed>();

    bezierWalker.spline = crystalPathSpline;
    bezierWalker.SetStartAndEndPoints(1, bezierWalker.spline.Count - 2);

    return crystalSeeker;
  }
}
