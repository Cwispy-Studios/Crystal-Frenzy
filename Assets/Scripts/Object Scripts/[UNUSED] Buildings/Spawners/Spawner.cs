using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{
  // Prefabs for the waypoint and for the units to spawn
  [SerializeField]
  private GameObject spawnUnitPrefab = null, waypointPrefab = null;
  [SerializeField]
  private float spawnInterval = 5f;

  private GameObject waypoint;
  private GameObject waypointOnUnit;
  private bool waypointTrackingUnit = false;

  private float spawnTimer;

  private float buildingRadius;
  private float prefabRadius;
  private float prefabSize;

  private void Awake()
  {
    prefabRadius = spawnUnitPrefab.GetComponent<NavMeshAgent>().radius * 
      ((spawnUnitPrefab.transform.lossyScale.x + spawnUnitPrefab.transform.lossyScale.z) / 2f);
    prefabSize = prefabRadius * 2f;

    waypointOnUnit = gameObject;

    InitialiseWaypoint();
  }

  private void Update()
  {
    spawnTimer += Time.deltaTime;

    if (spawnTimer >= spawnInterval)
    {
      spawnTimer -= spawnInterval;

      InstantiateUnit();
    }

    if (waypointTrackingUnit)
    {
      // Check if unit is dead
      if (waypointOnUnit != null)
      {
        // If unit type, place the waypoint on the unit's transform
        if (waypointOnUnit.GetComponent<ObjectTypes>().objectType == ObjectTypes.OBJECT_TYPES.UNIT)
        {
          Vector3 waypointPos = waypointOnUnit.transform.position;
          waypointPos.y = (waypointOnUnit.GetComponent<NavMeshAgent>().height * waypointOnUnit.transform.lossyScale.y) / 2f;
          waypoint.transform.position = waypointPos;
        }

        // Place the waypoint on the building
        else
        {
          OffsetWaypointOnBuilding(waypointOnUnit);
        }
      }

      // Unit is dead, stop tracking
      else
      {
        // Set the waypoint on the ground
        // OR set it back to itself, DISCUSS IT!
        Vector3 waypointPosOnGround = waypoint.transform.position;

        waypointPosOnGround.y = 0;
        SetWaypointAtPoint(waypointPosOnGround);

        //SetWaypointAtUnit(gameObject);
      }
    }
  }

  private void InitialiseWaypoint()
  {
    // Instantiate the waypoint
    waypoint = Instantiate(waypointPrefab);

    OffsetWaypointOnBuilding(gameObject);
  }

  private void OffsetWaypointOnBuilding(GameObject building)
  {
    // Set the waypoint to be just outside the building
    // Find the angle that the building is facing towards. We can find this by find the angle between global and local z
    // This angle is 90 degrees rightwards of where the building is facing
    float facingAngle = Vector3.SignedAngle(Vector3.forward, building.transform.forward, Vector3.up) * Mathf.Deg2Rad;
    Vector3 offset = new Vector3(Mathf.Cos(-facingAngle), 0, Mathf.Sin(-facingAngle));

    // Scale that vector by the radius of the building collider and place the waypoint there
    buildingRadius = building.GetComponent<NavMeshObstacle>().radius *
      ((building.transform.lossyScale.x + building.transform.lossyScale.z) / 2f);

    offset *= buildingRadius;

    // Set waypoint position offset by current position
    waypoint.transform.position = building.transform.position + offset;
  }

  private void InstantiateUnit()
  {
    // We will first try to place the unit's position towards the waypoint, so we need to find the vector 
    // from the building to the waypoint. That is waypoint pos minus building pos
    Vector3 spawnDir = waypoint.transform.position - transform.position;

    // Now we need to normalise the vector
    spawnDir.Normalize();

    // Now we can calculate the spawn position offsetting from the unit's navmeshagent size as well
    Vector3 spawnPos = transform.position + (spawnDir * buildingRadius) + (spawnDir * prefabSize);

    // Set the y-axis to the prefab's value
    spawnPos.y = spawnUnitPrefab.transform.position.y;

    // Now we check if there are any units in the space where we are spawning at. 
    // We don't want to get UI elements and the terrain
    int layerMask = (1 << 5) | (1 << 8);
    layerMask = ~layerMask;

    // Find if the spawn position has any objects on it already
    Collider [] colliders = Physics.OverlapSphere(spawnPos, prefabRadius, layerMask);

    // Number of times we have looped around the building
    //int numLoops = 1;

    // Loop tracker
    bool checkFront = false;
    bool checkLeft = false;
    bool checkBack = false;

    // If there are colliders, then there is a unit in that space so it can't spawn there
    while (colliders.Length > 0)
    {
      // We will go around the building in a square over 
      // and over until we find a spot that is free

      // Find the angle the building is facing towards
      float facingAngle = Vector3.SignedAngle(Vector3.forward, transform.forward, Vector3.up) * Mathf.Deg2Rad;
      facingAngle -= Mathf.PI / 2;

      // We check the outer coordinates of the building's collider
      float minX = transform.position.x - buildingRadius;
      float maxX = transform.position.x + buildingRadius;
      float minZ = transform.position.z - buildingRadius;
      float maxZ = transform.position.z + buildingRadius;

      // Try spawn pos in the direction building is facing
      if (spawnPos.x > maxX + (transform.forward.x * prefabSize) && 
          spawnPos.z < maxZ + (transform.forward.z * prefabSize))
      {
        if (checkFront && checkLeft && checkBack)
        {
          // Move the spawnPos out by the initial spawn direction
          spawnPos += spawnDir * prefabSize;

          checkFront = false;
          checkLeft = false;
          checkBack = false;

          //Debug.Log("spawn pos update" + spawnPos);

          //foreach (Collider collider in colliders)
          //  Debug.Log(collider.name);
        }

        spawnPos += transform.forward * prefabSize;
      }

      // Try spawn pos in the direction 90 degees left of the building is facing
      else if (spawnPos.z > maxZ + (-transform.right.z * prefabSize) &&
               spawnPos.x > minX + (-transform.right.x * prefabSize))
      {
        checkFront = true;
        spawnPos -= transform.right * prefabSize;
      }

      // Try spawn pos in the opposite direction the building is facing
      else if (spawnPos.x < minX + (-transform.forward.x * prefabSize) &&
               spawnPos.z > minZ + (-transform.forward.z * prefabSize))
      {
        checkLeft = true;
        spawnPos -= transform.forward * prefabSize;
      }

      // Try spawn pos in the direction 90 degrees right of the building is facing
      else if (spawnPos.z < minZ + (transform.right.z * prefabSize) &&
               spawnPos.x < maxX + (transform.right.x * prefabSize))
      {
        checkBack = true;
        spawnPos += transform.right * prefabSize;
      }

      else
      {
        Debug.Log("Weird spawn pos: " + spawnPos);
        spawnPos += transform.forward * prefabRadius;
        Debug.LogError("Spawn in unknown direction, check for blind spots!");
        Debug.Log("Spawn pos: " + spawnPos);
        Debug.Log("Check: " + (minX - (-transform.forward.x * prefabRadius)) + " " + (minZ - (-transform.forward.z * prefabRadius)));
      }

      colliders = Physics.OverlapSphere(spawnPos, prefabRadius, layerMask);
    }

    //Debug.Log("Building pos at " + transform.position);
    //Debug.Log("Instantiating unit at " + spawnPos);

    GameObject spawnedUnit = Instantiate(spawnUnitPrefab, spawnPos, new Quaternion());

    // Tell unit to go to waypoint, unless unit is self then don't do anything
    if (waypointOnUnit != gameObject)
    {
      UnitOrder unitOrder = spawnedUnit.GetComponent<UnitOrder>();

      // If waypoint is not tracking any unit, just move unit to the point
      if (waypointTrackingUnit == false)
      {
        unitOrder.IssueOrderPoint(waypoint.transform.position);
      }

      else
      {
        unitOrder.IssueOrderTarget(waypointOnUnit);
      }
    }
  }

  public void SetWaypointAtPoint(Vector3 point)
  {
    waypoint.transform.position = point;
    waypointTrackingUnit = false;
  }

  public void SetWaypointAtUnit(GameObject unit)
  {
    waypointOnUnit = unit;
    waypointTrackingUnit = true;
  }
}
