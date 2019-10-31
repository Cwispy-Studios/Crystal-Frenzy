﻿using System.Collections.Generic; // List
using UnityEngine;

public class CameraIssueOrdering : MonoBehaviour
{
  [SerializeField]
  private GameObject moveCirclePrefab = null;

  private const KeyCode attackMoveCommand = KeyCode.A;

  private List<GameObject> selectableObjects;

  public bool AttackMoveOrder { get; private set; }

  private bool orderToGround = false;
  private bool attackToGround = false;
  private Vector3 orderToGroundPos;

  private void Awake()
  {
    AttackMoveOrder = false;
  }

  void Update()
  {
    selectableObjects = CameraObjectSelection.SelectedUnitsList;

    // Check if there are friendly units in that list
    bool friendlyUnitsInList = true;
    bool friendlyObjectsInList = true;

    FriendlyUnitsInList(selectableObjects, ref friendlyUnitsInList, ref friendlyObjectsInList);

    if (!friendlyObjectsInList)
    {
      AttackMoveOrder = false;
      return;
    }

    if (!friendlyUnitsInList)
    {
      AttackMoveOrder = false;
    }

    // During attack move order, left clicking on the ground issues an attack move order, where units constantly seek out enemies
    // while moving to the destination. Right clicking cancels the order
    if (AttackMoveOrder)
    {
      CameraProperties.selectionDisabled = true;

      if (Input.GetMouseButtonDown(0) && !CameraProperties.mouseOverUI)
      {
        Order(true);
        AttackMoveOrder = false;
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

    if (orderToGround)
    {
      orderToGround = false;
      GameObject moveCircle = Instantiate(moveCirclePrefab, orderToGroundPos + (Vector3.up * 0.001f), Quaternion.Euler(90, 0, 0));

      if (attackToGround)
      {
        Color redCircleColor = new Color(1, 0, 0, 0.8f);

        moveCircle.GetComponent<Renderer>().material.SetColor("_Color", redCircleColor);

        attackToGround = false;
      }
    }
  }

  private void Order(bool attackMoveOrder = false)
  {
    Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

    // Ignore UI elements
    int layerMask = (1 << 5) | (1 << 9) | (1 << 10);

    if (Physics.Raycast(ray, out RaycastHit hit, 750f, ~layerMask))
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

            if (selectedObject.GetComponent<UnitOrder>())
            {
              orderToGround = true;
              orderToGroundPos = hit.point;

              if (attackMoveOrder)
              {
                attackToGround = true;
              }
            }

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
        if (hit.collider.GetComponent<Selectable>() != null && hit.collider.GetComponent<Selectable>().enabled)
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

              if (selectedObject.GetComponent<UnitOrder>())
              {
                orderToGround = true;
                orderToGroundPos = hit.point;

                if (attackMoveOrder)
                {
                  attackToGround = true;
                }
              }

              if (attackMoveOrder)
              {
                selectedObject.GetComponent<Attack>().SetAttackMovePosition(hit.point);
              }
            }
          }
        }
      }
    }
  }

  private void FriendlyUnitsInList(List<GameObject> selectedList, ref bool friendlyUnits, ref bool friendlyObjects)
  {
    if (selectedList.Count == 0)
    {
      friendlyUnits = false;
      friendlyObjects = false;

      return;
    }

    for (int i = 0; i < selectedList.Count; ++i)
    {
      GameObject selectedObject = selectedList[i];

      if (selectedObject.GetComponent<Faction>().faction != Faction.FACTIONS.GOBLINS)
      {
        friendlyUnits = false;
        friendlyObjects = false;

        return;
      }

      else if (selectedObject.GetComponent<Faction>().faction == Faction.FACTIONS.GOBLINS)
      {
        if (selectedObject.GetComponent<ObjectTypes>().objectType != ObjectTypes.OBJECT_TYPES.UNIT)
        {
          friendlyUnits = false;
        }
      }
    }
  }
}