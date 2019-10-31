using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
  /*********************** Move variables ***********************/
  private readonly float moveSpeed = 100f;
  private readonly float birdsEyeViewMoveSpeed = 140f;
  private readonly float moveDetectPerc = 0.01f;

  /*********************** Pan variables ***********************/
  private readonly float panSpeed = 75f;

  /*********************** Zoom variables ***********************/
  private const float MIN_ZOOM = 15f;
  public static readonly float MAX_ZOOM = 35f;
  private readonly float zoomSpeed = 1f;

  [HideInInspector]

  public bool birdsEyeViewMode = false;

  private List<GameObject> cameraBounds = new List<GameObject>();

  private void LateUpdate()
  {
    if (!birdsEyeViewMode)
    {
      Move();
      Pan();
      Zoom();
    }
    
    else
    {
      Move();
    }
  }

  private void Move()
  {
    // Check if the mouse is at the edge of the screen within the panDetectPerc
    // Calculate panDetect values
    Vector2 moveDetectThreshold = new Vector2(Screen.width * moveDetectPerc, Screen.height * moveDetectPerc);

    // Stores the old camera position in a temp value first, check if the new value is valid with all the stored camera bounds
    // If any errors, we revert to a corrected position
    Vector3 previousPosition = transform.position;

    float scrollSpeed = birdsEyeViewMode ? birdsEyeViewMoveSpeed : moveSpeed;

    // Check left side of screen but don't move it if is past the screen
    if (Input.mousePosition.x <= moveDetectThreshold.x && Input.mousePosition.x >= 0)
    {
      transform.Translate(Vector3.right * -scrollSpeed * Time.deltaTime / GetZoomPerc());
    }

    // Check right side of screen
    else if (Input.mousePosition.x >= Screen.width - moveDetectThreshold.x && Input.mousePosition.x <= Screen.width)
    {
      transform.Translate(Vector3.right * scrollSpeed * Time.deltaTime / GetZoomPerc());
    }

    // Check top side of screen
    if (Input.mousePosition.y <= moveDetectThreshold.y && Input.mousePosition.y >= 0)
    {
      transform.Translate(Quaternion.Euler(0, 90, 0) * transform.right * scrollSpeed * Time.deltaTime / GetZoomPerc(), Space.World);
    }

    // Check bottom side of screen
    else if (Input.mousePosition.y >= Screen.height - moveDetectThreshold.y && Input.mousePosition.y <= Screen.height)
    {
      transform.Translate(Quaternion.Euler(0, -90, 0) * transform.right * scrollSpeed * Time.deltaTime / GetZoomPerc(), Space.World);
    }

    if (CameraManager.cameraLerping == false)
    {
      CheckCameraInBounds(previousPosition);
    }
  }

  private void Pan()
  {
    // Check for input on left keys
    if (Input.GetKey("left"))
    {
      transform.Rotate(0, -panSpeed * Time.deltaTime, 0, Space.World);
    }

    // Check for input on right keys
    else if (Input.GetKey("right"))
    {
      transform.Rotate(0, panSpeed * Time.deltaTime, 0, Space.World);
    }
  }

  private void Zoom()
  {
    if (Input.mouseScrollDelta.y != 0)
    {
      if (Input.mouseScrollDelta.y > 0 && transform.position.y > MIN_ZOOM)
      {
        transform.Translate(Vector3.forward * Input.mouseScrollDelta.y * zoomSpeed);
      }

      else if (Input.mouseScrollDelta.y < 0 && transform.position.y < MAX_ZOOM)
      {
        transform.Translate(Vector3.forward * Input.mouseScrollDelta.y * zoomSpeed);
      }
    }
  }

  private void CheckCameraInBounds(Vector3 oldPosition)
  {
    // Check every bounds against the current camera position to know which bounds it is in
    for (int i = 0; i < cameraBounds.Count; ++i)
    {
      Bounds cameraBound = cameraBounds[i].GetComponent<Renderer>().bounds;

      // First we check if the new position is within any of the bounds
      if (transform.position.x >= cameraBound.min.x && transform.position.x <= cameraBound.max.x &&
        transform.position.z >= cameraBound.min.z && transform.position.z <= cameraBound.max.z)
      {
        return;
      }

      else
      {
        // Otherwise, it means we have moved off the camera bound, find which bounds the old camera position is in now
        if (oldPosition.x >= cameraBound.min.x && oldPosition.x <= cameraBound.max.x &&
        oldPosition.z >= cameraBound.min.z && oldPosition.z <= cameraBound.max.z)
        {
          // Check if the axis are out of bounds and correct
          if (oldPosition.x < cameraBound.min.x)
          {
            oldPosition.x = cameraBound.min.x;
          }

          else if (oldPosition.x > cameraBound.max.x)
          {
            oldPosition.x = cameraBound.max.x;
          }

          else if (oldPosition.z < cameraBound.min.z)
          {
            oldPosition.z = cameraBound.min.z;
          }

          else if (oldPosition.z > cameraBound.max.z)
          {
            oldPosition.z = cameraBound.max.z;
          }
        }
      }

      
    }

    transform.position = oldPosition;
  }

  public float GetZoomPerc()
  {
    if (birdsEyeViewMode)
    {
      return 1f;
    }

    else
    {
      return MAX_ZOOM / transform.position.y;
    }
  }

  public void AddCameraBounds(GameObject cameraBound)
  {
    if (!cameraBounds.Contains(cameraBound))
    {
      cameraBounds.Add(cameraBound);
    }
  }

  public void RemoveCameraBounds(GameObject cameraBound)
  {
    if (cameraBounds.Contains(cameraBound))
    {
      cameraBounds.Remove(cameraBound);
    }
  }
}
