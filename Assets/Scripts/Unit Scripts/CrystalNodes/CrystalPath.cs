using System.Collections.Generic;
using BezierSolution;
using UnityEngine;

public struct CrystalWaypoint
{
  public Vector3 lineStart;
  public Vector3 lineEnd;
  public Vector3 lineCenter;
  public float pointProgress;
}

public class CrystalPath : MonoBehaviour
{
  private const float WAYPOINT_LENGTH = 15f;

  public GameObject testCube;

  private BezierSpline crystalPath;
  private Dictionary<int, CrystalWaypoint> waypoints;

  private void Start()
  {
    crystalPath = GetComponent<BezierSpline>();

    waypoints = new Dictionary<int, CrystalWaypoint>();

    // Start from third waypoint and end at third last waypoint
    for (int i = 2; i < crystalPath.Count - 2; ++i)
    {
      Vector3 normalPointOne = new Vector3 {
        x = -crystalPath[i].precedingControlPointLocalPosition.z,
        y = crystalPath[i].precedingControlPointLocalPosition.y,
        z = crystalPath[i].precedingControlPointLocalPosition.x
      };

      Vector3 normalPointTwo = new Vector3
      {
        x = -normalPointOne.x,
        y = normalPointOne.y,
        z = -normalPointOne.z
      };

      Vector3 midpoint = crystalPath[i].position;

      // Find normalised direction vectors from center to end points of normal points
      // Rotates by the path's rotation and the point's rotation, otherwise the normal will be rotated at an angle
      Vector3 directionVectorOne = Quaternion.Euler(0, transform.eulerAngles.y, 0) * normalPointOne.normalized;
      Vector3 directionVectorTwo = Quaternion.Euler(0, transform.eulerAngles.y, 0) * normalPointTwo.normalized;

      directionVectorOne = Quaternion.Euler(0, crystalPath[i].rotation.eulerAngles.y, 0) * normalPointOne.normalized;
      directionVectorTwo = Quaternion.Euler(0, crystalPath[i].rotation.eulerAngles.y, 0) * normalPointTwo.normalized;

      CrystalWaypoint waypoint = new CrystalWaypoint {
        lineStart = midpoint + (directionVectorOne * (WAYPOINT_LENGTH / 2f)),
        lineEnd = midpoint + (directionVectorTwo * (WAYPOINT_LENGTH / 2f)),
        lineCenter = midpoint };

      crystalPath.FindNearestPointTo(waypoint.lineCenter, out waypoint.pointProgress);

      waypoints.Add(i, waypoint);

      // Used to check the line
      //Instantiate(testCube, waypoints[i].lineStart, new Quaternion());
      //Instantiate(testCube, waypoints[i].lineEnd, new Quaternion());
    }
  }

  public CrystalWaypoint GetLastCrystalWaypoint()
  {
    return waypoints[crystalPath.Count - 3];
  }

  public CrystalWaypoint GetCrystalWaypoint(int index)
  {
    return waypoints[index];
  }

  public int GetLastWaypointIndex()
  {
    return crystalPath.Count - 3;
  }
}
