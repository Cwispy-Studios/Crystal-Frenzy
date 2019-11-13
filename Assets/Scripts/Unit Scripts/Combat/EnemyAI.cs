using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
  private const float WAYPOINT_MARGIN = 15;
  private const float UPDATE_INTERVAL = 2f;
  private float updateCountdown = 0;

  private const float CLOSEST_PROGRESS_CHECK_INTERVAL = 2.5f;
  private static float lastCheckTime;
  private static bool firstTimeCheck = true;
  private static float furthestProgress;
  private static int waypointToWarpTo;
  private static bool validWarpPosition;

  public GameObject target;
  private bool targetIsMiner = false;

  private CrystalPath crystalPath;

  private int currentWaypointIndex;
  private bool initialWaypointSet = false;
  private float targetProgress = 1;
  private bool targetMiner = false;
  private Vector3 targetPos;

  private static GameManager gameManager = null;
  private static UnitManager unitManager = null;

  private void Awake()
  {
    if (gameManager == null)
    {
      gameManager = FindObjectOfType<GameManager>();
    }

    if (unitManager == null)
    {
      unitManager = FindObjectOfType<UnitManager>();
    }
  }

  private void Start()
  {
    updateCountdown = 0;

    // Determine how far in the path unit should warp to
    if (firstTimeCheck)
    {
      DetermineWaypointToWarp();
      lastCheckTime = Time.time;
    }

    else
    {
      if (Time.time - lastCheckTime >= CLOSEST_PROGRESS_CHECK_INTERVAL)
      {
        DetermineWaypointToWarp();
      }

      else
      {
        if (validWarpPosition)
        {
          GetComponent<NavMeshAgent>().Warp(crystalPath.GetCrystalWaypoint(waypointToWarpTo).lineCenter);

          currentWaypointIndex = waypointToWarpTo - 1;
        }
      } 
    }
  }

  private void Update()
  {
    if (target != null)
    {
      if (updateCountdown <= 0)
      {
        // Check if next waypoint has been set as a target since object creation
        if (!initialWaypointSet)
        {
          SetNextWaypointTarget(currentWaypointIndex);
        }

        else
        {
          // If target is Miner, check if its progress is later than the waypoint progress
          if (targetMiner || (targetIsMiner && target.GetComponent<BezierSolution.BezierWalkerWithSpeed>().NormalizedT >= targetProgress))
          {
            // Set attack position to target
            GetComponent<Attack>().SetAttackMovePosition(target.transform.position);
            targetMiner = true;
          }

          // Check if target has reached within distance to waypoint
          else if (Vector3.SqrMagnitude(transform.position - targetPos) <= (WAYPOINT_MARGIN * WAYPOINT_MARGIN))
          {
            // Set the next waypoint
            --currentWaypointIndex;

            // Check if we have reached the last waypoint
            if (currentWaypointIndex <= 2)
            {
              GetComponent<Attack>().SetAttackMovePosition(target.transform.position);
              target = null;
            }

            else
            {
              SetNextWaypointTarget(currentWaypointIndex);
            }
          }
        }
        
        updateCountdown = UPDATE_INTERVAL;

        if (targetMiner)
        {
          updateCountdown *= 2.5f;
        }
      }
      
      else
      {
        updateCountdown -= Time.deltaTime;
      }
    }
  }

  public void SetTarget(GameObject setTarget, CrystalPath setPath)
  {
    target = setTarget;
    crystalPath = setPath;

    // Check if the target is a node or a crystal seeker
    if (target.GetComponent<BezierSolution.BezierWalkerWithSpeed>())
    {
      targetIsMiner = true;
    }

    currentWaypointIndex = crystalPath.GetLastWaypointIndex();
  }

  private Vector3 GetClosestPointOnLineSegment(Vector3 A, Vector3 B)
  {
    Vector3 unitToStart = transform.position - A;
    Vector3 line = B - A;

    float magnitudeLine = line.sqrMagnitude;
    float dotProduct = Vector3.Dot(unitToStart, line);
    float distance = dotProduct / magnitudeLine;

    if (distance < 0)
    {
      return A;
    }

    else if (distance > 1)
    {
      return B;
    }

    else
    {
      return A + line * distance;
    }
  }

  private void SetNextWaypointTarget(int index)
  {
    // Credit: https://stackoverflow.com/questions/3120357/get-closest-point-to-a-line
    // Find the waypoint to target
    CrystalWaypoint waypoint = crystalPath.GetCrystalWaypoint(currentWaypointIndex);
    // Set the attack move position to the nearest point on the waypoint line
    targetPos = GetClosestPointOnLineSegment(waypoint.lineStart, waypoint.lineEnd);
    targetProgress = waypoint.pointProgress;
    GetComponent<Attack>().SetAttackMovePosition(targetPos);
    initialWaypointSet = true;
  }

  private void DetermineWaypointToWarp()
  {
    float minerProgress = 0;
    // Check miner's progress (if it is a miner)
    if (targetIsMiner)
    {
      minerProgress = target.GetComponent<BezierSolution.BezierWalkerWithSpeed>().NormalizedT;
    }

    // Check all the players units to find which has the furthest progress
    float furthestUnitProgress = unitManager.FindFurthestProgressOnSpline(crystalPath.GetComponent<BezierSolution.BezierSpline>());

    furthestProgress = minerProgress > furthestUnitProgress ? minerProgress : furthestUnitProgress;

    validWarpPosition = false;
    waypointToWarpTo = 0;

    // Find the way point which is closest to the furthest progress
    for (int i = 2; i < currentWaypointIndex; ++i)
    {
      // Find first waypoint from furthest to nearest waypoints if the player's progress is further than each of them
      if (crystalPath.GetCrystalWaypoint(i).pointProgress >= furthestProgress)
      {
        waypointToWarpTo = i + 2;

        // Take this waypoint + 2 to spawn from, but check if that number is not greater than the last waypoint count
        validWarpPosition = waypointToWarpTo <= crystalPath.GetLastWaypointIndex();

        break;
      }
    }

    if (validWarpPosition)
    {
      GetComponent<NavMeshAgent>().Warp(crystalPath.GetCrystalWaypoint(waypointToWarpTo).lineCenter);

      currentWaypointIndex = waypointToWarpTo - 1;
    }
  }
}
