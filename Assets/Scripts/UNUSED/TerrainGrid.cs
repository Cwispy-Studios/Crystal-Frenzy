using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGrid : MonoBehaviour
{
  public static bool CheckGridOccupied(Collider gridCollider)
  {
    // Get the list of all objects that have a collider
    Collider[] allColliders = FindObjectsOfType<Collider>();

    foreach (Collider checkCollider in allColliders)
    {
      // Does not check for other grids and the ground
      if (checkCollider.name != gridCollider.name && checkCollider.tag != "Ground" && gridCollider.bounds.Intersects(checkCollider.bounds))
      {
        return true;
      }
    }

    return false;

    // Move the position to the center of the grid
    //Vector3 centerOfGrid = new Vector3(position.x + 0.5f, 0, position.z + 0.5f);
    //Debug.DrawRay(centerOfGrid, Vector3.up);

    //if (Physics.Raycast(centerOfGrid, Vector3.up, out RaycastHit hitInfo))
    //{
    //  if (hitInfo.collider.tag != "Ground")
    //  {
    //    return true;
    //  }

    //  else return false;
    //}

    //else return false;
  }

  public static Vector3 GetNearestPointOnGrid(Vector3 position, float gridSize)
  {
    float xCount = Mathf.Floor(position.x / gridSize);
    float yCount = Mathf.Floor(position.y / gridSize);
    float zCount = Mathf.Floor(position.z / gridSize);

    Vector3 result = new Vector3(
            xCount * gridSize,
            yCount * gridSize,
            zCount * gridSize);

    return result;
  }
}
