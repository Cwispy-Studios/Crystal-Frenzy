using System.Collections.Generic; // List
using UnityEngine;

public class CameraIssueOrdering : MonoBehaviour
{
  private List<GameObject> selectableObjects;

  void Update()
  {
    selectableObjects = GetComponent<CameraObjectSelection>().SelectedUnitsList;

    // When RMB clicked and there are units selected
    if (Input.GetMouseButtonDown(1) && selectableObjects.Count > 0)
    {
      RightClick();
    }
  }

  private void RightClick()
  {
    Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

    // Ignore UI elements
    //int layerMask = 1 << 5;

    if (Physics.Raycast(ray, out RaycastHit hit))
    {
      // Right click on ground, order units to that point
      if (hit.collider.tag == "Ground")
      {
        foreach (GameObject selectedObject in selectableObjects)
        {
          Order order = selectedObject.GetComponent<Order>();

          if (order != null)
          {
            // Issue an order to the object 
            selectedObject.GetComponent<Order>().IssueOrderPoint(hit.point);
          }
        }
      }

      // Right clicked on unit, check if unit is selectable
      else
      {
        if (hit.collider.GetComponent<Selectable>() != null)
        {
          hit.collider.GetComponent<Selectable>().StartBlinking();

          foreach (GameObject selectedObject in selectableObjects)
          {
            Order order = selectedObject.GetComponent<Order>();

            if (order != null)
            {
              // Issue an order to the object 
              selectedObject.GetComponent<Order>().IssueOrderTarget(hit.collider.gameObject);
            }
          }
        }

        // If not selectable, just move units to the point
        else
        {
          foreach (GameObject selectedObject in selectableObjects)
          {
            Order order = selectedObject.GetComponent<Order>();

            if (order != null)
            {
              // Issue an order to the object 
              selectedObject.GetComponent<Order>().IssueOrderPoint(hit.point);
            }
          }
        }
      }
    }

    else
    {
      Debug.LogWarning("Clicked on nothing");
    }
  }
}
