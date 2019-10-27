using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
  /*********************** Move variables ***********************/
  private readonly float moveSpeed = 80f;
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

    // Stores the new camera position in a temp value first, check if this temp value is valid with all the stored camera bounds
    // After checking and correcting, then we move the actual camera position
    Transform tempCamTransform = transform;

    // Check left side of screen but don't move it if is past the screen
    if (Input.mousePosition.x <= moveDetectThreshold.x && Input.mousePosition.x >= 0)
    {
      tempCamTransform.Translate(Vector3.right * -moveSpeed * Time.deltaTime / GetZoomPerc());
    }

    // Check right side of screen
    else if (Input.mousePosition.x >= Screen.width - moveDetectThreshold.x && Input.mousePosition.x <= Screen.width)
    {
      tempCamTransform.Translate(Vector3.right * moveSpeed * Time.deltaTime / GetZoomPerc());
    }

    // Check top side of screen
    if (Input.mousePosition.y <= moveDetectThreshold.y && Input.mousePosition.y >= 0)
    {
      tempCamTransform.Translate(Quaternion.Euler(0, 90, 0) * transform.right * moveSpeed * Time.deltaTime / GetZoomPerc(), Space.World);
    }

    // Check bottom side of screen
    else if (Input.mousePosition.y >= Screen.height - moveDetectThreshold.y && Input.mousePosition.y <= Screen.height)
    {
      tempCamTransform.Translate(Quaternion.Euler(0, -90, 0) * transform.right * moveSpeed * Time.deltaTime / GetZoomPerc(), Space.World);
    }

    CheckCameraInBounds(tempCamTransform.position);
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

  private void CheckCameraInBounds(Vector3 tempCamPosition)
  {
    // First we do a check of the first and last camera bounds, checking if the z-axis is lower or greater than the min and max z-axis.
    // If it is, then we know where the camera is and we can check the x-axis for that bound
    // Camera is in the first bounds
    if (tempCamPosition.z < cameraBounds[0].GetComponent<Renderer>().bounds.min.z)
    {
      Bounds cameraBound = cameraBounds[0].GetComponent<Renderer>().bounds;
      tempCamPosition.z = cameraBound.min.z;

      // Check the x-axis
      if (tempCamPosition.x < cameraBound.min.x)
      {
        tempCamPosition.x = cameraBound.min.x;
      }

      else if (tempCamPosition.x > cameraBound.max.x)
      {
        tempCamPosition.x = cameraBound.max.x;
      }

      transform.position = tempCamPosition;
    }

    // Camera is in the last bounds
    else if (tempCamPosition.z > cameraBounds[cameraBounds.Count - 1].GetComponent<Renderer>().bounds.max.z)
    {
      Bounds cameraBound = cameraBounds[cameraBounds.Count - 1].GetComponent<Renderer>().bounds;
      tempCamPosition.z = cameraBound.max.z;

      // Check the x-axis
      if (tempCamPosition.x < cameraBound.min.x)
      {
        tempCamPosition.x = cameraBound.min.x;
      }

      else if (tempCamPosition.x > cameraBound.max.x)
      {
        tempCamPosition.x = cameraBound.max.x;
      }

      transform.position = tempCamPosition;
    }

    // Check every bounds against the current camera position to know which bounds it is in
    else
    {
      // Every bounds we check, if out of bounds we save a corrected value to clamp the position to
      float correctedX = 0;
      float closestCorrection = 99999f;

      for (int i = 0; i < cameraBounds.Count; ++i)
      {
        Bounds cameraBound = cameraBounds[i].GetComponent<Renderer>().bounds;

        // Check which bounds the camera is currently in
        if (transform.position.z >= cameraBound.min.z && transform.position.z <= cameraBound.max.z)
        {
          // Check if the x-axis is within the bounds and set a correctedX. We do not correct immediately because the bounds overlap
          // and the x-axis may be valid for the next one, if it is then we do not need to correct
          if (tempCamPosition.x < cameraBound.min.x)
          {
            if (Mathf.Abs(tempCamPosition.x - cameraBound.min.x) < closestCorrection)
            {
              correctedX = cameraBound.min.x;
            }
          }

          else if (tempCamPosition.x > cameraBound.max.x)
          {
            if (Mathf.Abs(tempCamPosition.x - cameraBound.min.x) < closestCorrection)
            {
              correctedX = cameraBound.max.x;
            }
          }

          // The x-axis is in bounds, so the move is valid. We can then stop the function since we know the move is valid
          else
          {
            transform.position = tempCamPosition;

            return;
          }
        }
      } // for

      // If we get here, it means that the camera has moved out of the x-axis, and we simply need to correct that
      tempCamPosition.x = correctedX;

      transform.position = tempCamPosition;
    } // else 
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
}
