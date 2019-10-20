using System.Collections.Generic; // List
using UnityEngine;

public class CameraIssueOrdering : MonoBehaviour
{
  private const KeyCode attackMoveCommand = KeyCode.A;

  private List<GameObject> selectableObjects;

  public bool AttackMoveOrder { get; private set; }

  private void Awake()
  {
    AttackMoveOrder = false;
  }

  void Update()
  {
    selectableObjects = GetComponent<CameraObjectSelection>().SelectedUnitsList;

    // Check if there are friendly units in that list
    bool friendlyUnitsInList = FriendlyUnitsInList(selectableObjects);

    if (selectableObjects.Count == 0)
    {
      AttackMoveOrder = false;
    }

    else
    {
      // During attack move order, left clicking on the ground issues an attack move order, where units constantly seek out enemies
      // while moving to the destination. Right clicking cancels the order
      if (AttackMoveOrder)
      {
        CameraProperties.selectionDisabled = true;

        if (Input.GetMouseButtonDown(0) && !CameraProperties.mouseOverUI)
        {
          Order(true);
        }

        else if (Input.GetMouseButtonDown(1))
        {
          AttackMoveOrder = false;
        }
      }

      else
      {
        // When RMB clicked and there are units selected
        if (Input.GetMouseButtonDown(1))
        {
          Order();
        }

        if (Input.GetKeyDown(attackMoveCommand) && friendlyUnitsInList)
        {
          AttackMoveOrder = true;
        }
      }
    }
  }

  private void Order(bool attackMoveOrder = false)
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

            if (attackMoveOrder)
            {
              selectedObject.GetComponent<Attack>().SetAttackMovePosition(hit.point);
            }
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

              if (attackMoveOrder)
              {
                selectedObject.GetComponent<Attack>().SetAttackMovePosition(hit.point);
              }
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

  private bool FriendlyUnitsInList(List<GameObject> selectedList)
  {
    foreach (GameObject selectedObject in selectedList)
    {
      if (selectedObject.GetComponent<ObjectTypes>().objectType != ObjectTypes.OBJECT_TYPES.UNIT && 
          selectedObject.GetComponent<Faction>().faction != Faction.FACTIONS.GOBLINS)
      {
        return false;
      }
    }

    return true;
  }
}