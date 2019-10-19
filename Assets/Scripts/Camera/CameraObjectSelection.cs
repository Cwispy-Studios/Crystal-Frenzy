using System.Collections.Generic; // List
using UnityEngine;

public class CameraObjectSelection : MonoBehaviour
{
  private List<GameObject> selectedUnitsList;
  public List<GameObject> SelectedUnitsList
  {
    get { return selectedUnitsList; }
  }

  private bool isSelecting = false;
  private Vector3 originalMousePosition;
  private List<GameObject> mouseHoverUnitsList;
  private bool friendlyObjectsInList = false;
  private bool friendlyUnitsInList = false;
  private bool nonFriendlyUnitsPurgedFromList = false;

  // DEBUG
  //private Rect rect2;

  private void Awake()
  {
    selectedUnitsList = new List<GameObject>();
    mouseHoverUnitsList = new List<GameObject>();
  }

  private void Update()
  {
    // Store every selectable in a list
    Selectable[] selectables = FindObjectsOfType<Selectable>();

    // Update the selection colour of every unit
    foreach (Selectable selectableObject in selectables)
    {
      // Set the colour
      selectableObject.CheckFactionColour(GetComponent<Faction>().faction);
    }

    // If player is in attack move command, do not perform any selection/deselection actions
    if (GetComponent<CameraIssueOrdering>().AttackMoveOrder)
    {
      return;
    }
    
    // When LMB is pressed down
    if (Input.GetMouseButtonDown(0))
    {
      // Begin selection and save this mouse position
      isSelecting = true;
      originalMousePosition = Input.mousePosition;
    }

    // When LMB is released, or checks if LMB is already released but the game still registers it as selecting
    if (Input.GetMouseButtonUp(0) || (isSelecting == true && Input.GetMouseButton(0) == false))
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
            foreach (GameObject hoverObject in mouseHoverUnitsList)
            {
              if (hoverObject.GetComponent<ObjectTypes>().objectType == ObjectTypes.OBJECT_TYPES.UNIT)
              {
                selectedUnitsList.Add(hoverObject);
                hoverObject.GetComponent<Selectable>().selectStatus = Selectable.SELECT_STATUS.SELECTED;
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
            for (int i = 0; i < mouseHoverUnitsList.Count; ++i)
            {
              if (i == 0)
              {
                selectedUnitsList.Add(mouseHoverUnitsList[i]);
                mouseHoverUnitsList[i].GetComponent<Selectable>().selectStatus = Selectable.SELECT_STATUS.SELECTED;
              }

              else
              {
                mouseHoverUnitsList[i].GetComponent<Selectable>().selectStatus = Selectable.SELECT_STATUS.UNSELECTED;
              }
            }
          }
        }

        // Left shift held, do not clear existing list and add on from hoverUnits list
        else
        {
          foreach (GameObject hoverObject in mouseHoverUnitsList)
          {
            Selectable selectedObject = hoverObject.GetComponent<Selectable>();

            if (!selectedUnitsList.Contains(hoverObject))
            {
              selectedUnitsList.Add(hoverObject);

              selectedObject.selectStatus = Selectable.SELECT_STATUS.SELECTED;
            }
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
    else
    {
      // Clear the current hover list
      if (mouseHoverUnitsList.Count > 1)
        Debug.LogError("HOVER UNITS LIST HAS MORE THAN 1 OBJECT WHEN IT IS NOT DRAGGING!");

      ClearHoverList(true);

      // Check if the mouse is hovering over any selectable
      GameObject hoveredObject = Utils.CheckMouseIsOverSelectable();
      Selectable hoveredSelectable = null;

      if (hoveredObject != null)
      {
        hoveredSelectable = hoveredObject.GetComponent<Selectable>();
      }

      if (hoveredSelectable != null)
      {
        // If object is already selected, the hover selection should not overwrite that
        if (hoveredSelectable.selectStatus != Selectable.SELECT_STATUS.SELECTED)
        {
          mouseHoverUnitsList.Add(hoveredObject.gameObject);
          hoveredSelectable.selectStatus = Selectable.SELECT_STATUS.HOVER;
        }
      }
    }
  }

  private void Click()
  {
    // Clear selection if left shift not held
    if (!Input.GetKey(KeyCode.LeftShift))
    {
      ClearSelectionList();
    }

    Selectable clickedObject = Utils.CheckMouseIsOverSelectable().GetComponent<Selectable>();

    if (clickedObject != null)
    {
      selectedUnitsList.Add(clickedObject.gameObject);
      clickedObject.selectStatus = Selectable.SELECT_STATUS.SELECTED;
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
          // Only change the status if object is already unselected
          if (selectableObject.selectStatus == Selectable.SELECT_STATUS.UNSELECTED)
          {
            selectableObject.selectStatus = Selectable.SELECT_STATUS.HOVER;
          }

          // Check if this unit already exists in the list
          if (!mouseHoverUnitsList.Contains(selectableObject.gameObject))
          {
            mouseHoverUnitsList.Add(selectableObject.gameObject);
          }
        }
      }

      // Object not within bounds
      else
      {
        if (selectableObject.selectStatus == Selectable.SELECT_STATUS.HOVER)
        {
          selectableObject.selectStatus = Selectable.SELECT_STATUS.UNSELECTED;

          // Remove the unit from the hover list
          if (!mouseHoverUnitsList.Remove(selectableObject.gameObject))
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
    //Debug.Log(cMin.z);
    //Debug.Log(cMax.z);
    //Debug.Log("");
    cMin.z = 0;
    cMax.z = 0;
    colliderBounds.SetMinMax(Vector3.Min(cMin, cMax), Vector3.Max(cMin, cMax));

    //cMin.y = Screen.height - cMin.y;
    //cMax.y = Screen.height - cMax.y;
    //rect2 = Rect.MinMaxRect(cMin.x, cMin.y, cMax.x, cMax.y);
    //Debug.Log(colliderBounds.min);
    //Debug.Log(colliderBounds.max);

    // Set up the selection bounds based on the mouse positions
    Bounds selectionBounds = new Bounds();
    selectionBounds.SetMinMax(Vector3.Min(originalMousePosition, Input.mousePosition),
                              Vector3.Max(originalMousePosition, Input.mousePosition));

    //Debug.Log(colliderBounds);
    //Debug.Log(selectionBounds);

    // Check if they intersect
    return selectionBounds.Intersects(colliderBounds);
  }

  private void PurgeHoverListOfNonFriendlyUnits()
  {
    for (int i = 0; i < mouseHoverUnitsList.Count; ++i)
    {
      // Check if the unit is non friendly
      if (mouseHoverUnitsList[i].GetComponent<Faction>().faction != GetComponent<Faction>().faction)
      {
        mouseHoverUnitsList[i].GetComponent<Selectable>().selectStatus = Selectable.SELECT_STATUS.UNSELECTED;

        // Remove the unit from the hover list
        if (!mouseHoverUnitsList.Remove(mouseHoverUnitsList[i]))
        {
          Debug.LogError("Hover unit not found when purging non friendlies from list!" + mouseHoverUnitsList[i].name);
        }
      }
    }
  }

  private void ClearSelectionList()
  {
    foreach (GameObject selectedObject in selectedUnitsList)
    {
      Selectable selectableObject = selectedObject.GetComponent<Selectable>();

      selectableObject.selectStatus = Selectable.SELECT_STATUS.UNSELECTED;
    }

    selectedUnitsList.Clear();
  }

  private void ClearHoverList(bool resetSelection)
  {
    if (resetSelection)
    {
      foreach (GameObject hoverObject in mouseHoverUnitsList)
      {
        hoverObject.GetComponent<Selectable>().selectStatus = Selectable.SELECT_STATUS.UNSELECTED;
      }
    }

    mouseHoverUnitsList.Clear();
    friendlyObjectsInList = false;
    friendlyUnitsInList = false;
    nonFriendlyUnitsPurgedFromList = false;
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
}
