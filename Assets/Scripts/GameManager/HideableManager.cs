using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HideableManager : MonoBehaviour
{
  public static List<FogOfWarMesh> fogOfWarMeshList = new List<FogOfWarMesh>();
  public static List<GameObject> hideableUnits = new List<GameObject>();

  private const float UPDATE_INTERVAL = 1f;
  private float updateCountdown = UPDATE_INTERVAL;

  private void Update()
  {
    if (updateCountdown >= UPDATE_INTERVAL)
    {
      updateCountdown = 0;

      for (int i = 0; i < hideableUnits.Count; ++i)
      {
        // Set every unit to invisible, if they are colliding with a FOVMesh set them as visible
        hideableUnits[i].layer = 10;
        hideableUnits[i].GetComponent<Selectable>().enabled = false;
      }

      for (int fovMesh = 0; fovMesh < fogOfWarMeshList.Count; ++fovMesh)
      {
        for (int i = 0; i < hideableUnits.Count; ++i)
        {
          // Point to circle collision
          if (fogOfWarMeshList[fovMesh].FovShape == FOV_SHAPE.CIRCLE)
          {
            if (Vector3.SqrMagnitude(hideableUnits[i].transform.position - fogOfWarMeshList[fovMesh].transform.position) <= 
                fogOfWarMeshList[fovMesh].transform.lossyScale.x * fogOfWarMeshList[fovMesh].transform.lossyScale.x)
            {
              hideableUnits[i].layer = 0;
              hideableUnits[i].GetComponent<Selectable>().enabled = true;
            }
          }

          // May not be required since rect meshes only exist during the Preparation Phase or if the path is already safe,
          // meaning no enemies will travel through that area
          //else
          //{
          //  if (hideableUnits[i].transform.position.x >= fogOfWarMeshList[fovMesh].transform.position.x - fogOfWarMeshList[fovMesh].transform.lossyScale.x &&
          //      hideableUnits[i].transform.position.x <= fogOfWarMeshList[fovMesh].transform.position.x + fogOfWarMeshList[fovMesh].transform.lossyScale.x &&
          //      hideableUnits[i].transform.position.y >= fogOfWarMeshList[fovMesh].transform.position.y - fogOfWarMeshList[fovMesh].transform.lossyScale.y &&
          //      hideableUnits[i].transform.position.y <= fogOfWarMeshList[fovMesh].transform.position.y + fogOfWarMeshList[fovMesh].transform.lossyScale.y)
          //  {
          //    hideableUnits[i].layer = 0;
          //    hideableUnits[i].GetComponent<Selectable>().enabled = true;
          //  }
          //}
        }
      } 
    }

    else
    {
      updateCountdown += Time.deltaTime;
    }
  }

  public void RemoveAllUnits()
  {
    for (int i = hideableUnits.Count - 1; i >= 0; --i)
    {
      Destroy(hideableUnits[i]);
    }
  }

  public void KillAllUnits()
  {
    for (int i = hideableUnits.Count - 1; i >= 0; --i)
    {
      hideableUnits[i].GetComponent<GoldGiver>().DisableGoldReward();

      Health health = hideableUnits[i].GetComponent<Health>();
      health.ModifyHealth(-health.CurrentHealth, health.transform.forward);
    }
  }
}
