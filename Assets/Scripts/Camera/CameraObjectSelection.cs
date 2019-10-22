using System.Collections.Generic; // List
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraObjectSelection : MonoBehaviour
{
  public static List<GameObject> SelectedUnitsList { get; private set; }

  private bool isSelecting = false;
  private Vector3 originalMousePosition;
  public static List<GameObject> MouseHoverUnitsList { get; private set; }
  private bool friendlyObjectsInList = false;
  private bool friendlyUnitsInList = false;
  private bool nonFriendlyUnitsPurgedFromList = false;

  // DEBUG
  //private Rect rect2;

  private void Awake()
  {
    SelectedUnitsList = new List<GameObject>();
    MouseHoverUnitsList = new List<GameObject>();
  }

  private void Update()
  {
    // TODO: UPDATE SELECTED AND HOVER LIST TO CHECK FOR NULL OBJECTS

    // Store every selectable in a list
    Selectable[] selectables = FindObjectsOfType<Selectable>();

    // Update the selection colour of every unit
    foreach (Selectable selectableObject in selectables)
    {
      // Set the colour
      selectableObject.CheckFactionColour(GetComponent<Faction>().faction);
    }

    if (CameraProperties.selectionDisabled)
    {
      isSelecting = false;
      CameraProperties.selectionDisabled = false;
      return;
    }
    
    // When LMB is pressed down
    if (Input.GetMouseButtonDown(0) && !CameraProperties.mouseOverUI)
    {
      // Begin selection and save this mouse position
      isSelecting = true;
      originalMousePosition = Input.mousePosition;
    }

    // When LMB is released, or checks if LMB is already released but the game still registers it as selecting
    if (isSelecting == true && (Input.GetMouseButtonUp(0) || Input.GetMouseButton(0) == false))
    {
      isSelecting = false;

      // Mouse is in the same position as when the LMB was clicked, so process this as a click.
      // Find if the mouse clicked over any selectable object
      if (originalMousePosition == Input.mousePosition)
      {
        Click();
      }

      else
      {
        // Clear the list of existing selected units if left shift not held
        if (!Input.GetKey(KeyCode.LeftShift))
        {
          ClearSelectionList();

          // If there are friendly units, only add Unit types to selectables
          if (friendlyUnitsInList)
          {
            foreach (GameObject hoverObject in MouseHoverUnitsList)
            {
              if (hoverObject.GetComponent<ObjectTypes>().objectType == ObjectTypes.OBJECT_TYPES.UNIT)
              {
                AddObjectToSelectionList(hoverObject);
              }

              else
              {
                hoverObject.GetComponent<Selectable>().selectStatus = Selectable.SELECT_STATUS.UNSELECTED;
              }
            }
          }

          // There are only buildings, only select the first object
          else
          {
            for (int i = 0; i < MouseHoverUnitsList.Count; ++i)
            {
              if (i == 0)
              {
                AddObjectToSelectionList(MouseHoverUnitsList[i]);
              }

              else
              {
                MouseHoverUnitsList[i].GetComponent<Selectable>().selectStatus = Selectable.SELECT_STATUS.UNSELECTED;
              }
            }
          }
        }

        // Left shift held, do not clear existing list and add on from hoverUnits list
        else
        {
          foreach (GameObject hoverObject in MouseHoverUnitsList)
          {
            AddObjectToSelectionList(hoverObject);
          } // foreach end
        } // else Left Shift end
      }

      // Reset the hover list
      ClearHoverList(false);
    }

    // While selecting and dragging but LMB is not yet released, only stores MOVABLE units from your side
    // This list here is built until the LMB is released, so we have to keep updating and maintaining the list
    if (isSelecting)
    {
      DragSelection(selectables);
    } 

    // Mouse is just hovering over the map
    else if (!CameraProperties.mouseOverUI)
    {
      // Clear the current hover list
      if (MouseHoverUnitsList.Count > 1)
        Debug.LogError("HOVER UNITS LIST HAS MORE THAN 1 OBJECT WHEN IT IS NOT DRAGGING!");

      ClearHoverList(true);

      // Check if the mouse is hovering over any selectable
      GameObject hoveredObject = Utils.CheckMouseIsOverSelectable();

      if (hoveredObject != null && hoveredObject.GetComponent<Selectable>() != null)
      {
        AddObjectToHoverList(hoveredObject.gameObject);
      }
    }
  }

  public static void RemoveDeadSelectedUnit(GameObject deadSelectedUnit)
  {
    SelectedUnitsList.Remove(deadSelectedUnit);
  }

  public static void RemoveDeadHoverUnit(GameObject deadHoverUnit)
  {
    MouseHoverUnitsList.Remove(deadHoverUnit);
  }

  private void Click()
  {
    // Clear selection if left shift not held
    if (!Input.GetKey(KeyCode.LeftShift))
    {
      ClearSelectionList();
      ClearHoverList(true);
    }

    GameObject check = Utils.CheckMouseIsOverSelectable();

    if (check != null)
    {
      Selectable clickedObject = check.GetComponent<Selectable>();

      if (clickedObject != null)
      {
        SelectedUnitsList.Add(clickedObject.gameObject);
        clickedObject.selectStatus = Selectable.SELECT_STATUS.SELECTED;
      }
    }
  }

  private void DragSelection(Selectable[] selectables)
  {
    // Loop through every selectable in the game and check if they are within the selection bounds of your drag selection
    foreach (Selectable selectableObject in selectables)
    {
      if (IsWithinSelectionBounds(selectableObject.gameObject))
      {
        // Check if this selectable is a friendly, if it is then there is now a friendly in the list
        bool selectableIsFriendly = selectableObject.gameObject.GetComponent<Faction>().faction == GetComponent<Faction>().faction;

        if (selectableIsFriendly)
        {
          friendlyObjectsInList = true;

          // Once a friendly has been detected, non friendlies only have to be purged from the list once since any subsequent
          // non friendlies would not be added to the list anymore
          if (!nonFriendlyUnitsPurgedFromList)
          {
            PurgeHoverListOfNonFriendlyUnits();
          }

          // Check if there a friendly units in the list so we know not to select buildings
          if (!friendlyUnitsInList)
          {
            if (selectableObject.gameObject.GetComponent<ObjectTypes>().objectType == ObjectTypes.OBJECT_TYPES.UNIT)
            {
              friendlyUnitsInList = true;
            }
          }
        }

        // If there is no friendly unit in the list, any selectable can be added to the list
        // If there are friendly units, only friendly units can be added to the list
        if (!friendlyObjectsInList || (friendlyObjectsInList && selectableIsFriendly))
        {
          AddObjectToHoverList(selectableObject.gameObject);
        }
      }

      // Object not within bounds
      else
      {
        if (selectableObject.selectStatus == Selectable.SELECT_STATUS.HOVER)
        {
          selectableObject.selectStatus = Selectable.SELECT_STATUS.UNSELECTED;

          // Remove the unit from the hover list
          if (!MouseHoverUnitsList.Remove(selectableObject.gameObject))
          {
            Debug.LogError("Hover unit not found when removing from list!" + selectableObject.gameObject);
          }
        }
      }
    } // foreach
  }

  private bool IsWithinSelectionBounds(GameObject checkObject)
  {
    Camera camera = GetComponent<Camera>();

    // Set up the collider bounds of the game object we are checking
    // Since we are checking in a 2D space, we have to convert the collider bounds
    // to screen point then remove the z-axis
    Bounds colliderBounds = checkObject.GetComponent<Collider>().bounds;
    Vector3 cMin = camera.WorldToScreenPoint(colliderBounds.min);
    Vector3 cMax = camera.WorldToScreenPoint(colliderBounds.max);
    cMin.z = 0;
    cMax.z = 0;
    colliderBounds.SetMinMax(Vector3.Min(cMin, cMax), Vector3.Max(cMin, cMax));

    // Set up the selection bounds based on the mouse positions
    Bounds selectionBounds = new Bounds();
    selectionBounds.SetMinMax(Vector3.Min(originalMousePosition, Input.mousePosition),
                              Vector3.Max(originalMousePosition, Input.mousePosition));

    // Check if they intersect
    return selectionBounds.Intersects(colliderBounds);
  }

  private void PurgeHoverListOfNonFriendlyUnits()
  {
    for (int i = 0; i < MouseHoverUnitsList.Count; ++i)
    {
      // Check if the unit is non friendly
      if (MouseHoverUnitsList[i].GetComponent<Faction>().faction != GetComponent<Faction>().faction)
      {
        MouseHoverUnitsList[i].GetComponent<Selectable>().selectStatus = Selectable.SELECT_STATUS.UNSELECTED;

        // Remove the unit from the hover list
        if (!MouseHoverUnitsList.Remove(MouseHoverUnitsList[i]))
        {
          Debug.LogError("Hover unit not found when purging non friendlies from list!" + MouseHoverUnitsList[i].name);
        }
      }
    }
  }

  public void ClearSelectionList()
  {
    foreach (GameObject selectedObject in SelectedUnitsList)
    {
      // Only clear if selected status is hover
      Selectable selectableObject = selectedObject.GetComponent<Selectable>();
      
      selectableObject.selectStatus = Selectable.SELECT_STATUS.UNSELECTED;
    }

    SelectedUnitsList.Clear();
  }

  private void ClearHoverList(bool resetSelection)
  {
    if (resetSelection)
    {
      foreach (GameObject hoverObject in MouseHoverUnitsList)
      {
        if (hoverObject.GetComponent<Selectable>().selectStatus == Selectable.SELECT_STATUS.HOVER)
        {
          hoverObject.GetComponent<Selectable>().selectStatus = Selectable.SELECT_STATUS.UNSELECTED;
        }
      }
    }

    MouseHoverUnitsList.Clear();
    friendlyObjectsInList = false;
    friendlyUnitsInList = false;
    nonFriendlyUnitsPurgedFromList = false;
  }

  public void AddObjectToHoverList(GameObject hoverObject)
  {
    if (!MouseHoverUnitsList.Contains(hoverObject))
    {
      MouseHoverUnitsList.Add(hoverObject);
      
    }

    // If object is already selected, the hover selection should not overwrite that
    if (hoverObject.GetComponent<Selectable>().selectStatus != Selectable.SELECT_STATUS.SELECTED)
    {
      hoverObject.GetComponent<Selectable>().selectStatus = Selectable.SELECT_STATUS.HOVER;
    }
  }

  public void AddObjectToSelectionList(GameObject selectedObject)
  {
    if (!SelectedUnitsList.Contains(selectedObject))
    {
      SelectedUnitsList.Add(selectedObject);

      selectedObject.GetComponent<Selectable>().selectStatus = Selectable.SELECT_STATUS.SELECTED;
    }
  }

  private void OnGUI()
  {
    if (isSelecting)
    {
      Rect rect = Utils.GetScreenRect(originalMousePosition, Input.mousePosition);
      Utils.DrawScreenRectBorder(rect, 2);
      //DrawScreenRect(rect2);
    }
  }

  //private void OnMouseDown()
  //{
  //  EventSystem eventSys = GameObject.Find("EventSystem").GetComponent<EventSystem>();

  //  if (eventSys.IsPointerOverGameObject())
  //  {
  //    eventSys.
  //    Debug.Log("OVER!");
  //    return; // exit out of OnMouseDown() because its over the uGUI
  //  }
  //}
}
