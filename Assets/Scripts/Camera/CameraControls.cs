using UnityEngine;

public class CameraControls : MonoBehaviour
{
  /*********************** Move variables ***********************/
  private readonly float moveSpeed = 80f;
  private readonly float moveDetectPerc = 0.01f;

  /*********************** Pan variables ***********************/
  private readonly float panSpeed = 75f;

  /*********************** Zoom variables ***********************/
  private const float MIN_ZOOM = 10f;
  private const float MAX_ZOOM = 20f;
  private readonly float zoomSpeed = 1f;

  // Update is called once per frame
  private void Update()
  {
    Move();
    Pan();
    Zoom();
  }

  private void Move()
  {
    // Check if the mouse is at the edge of the screen within the panDetectPerc
    // Calculate panDetect values
    Vector2 moveDetectThreshold = new Vector2(Screen.width * moveDetectPerc, Screen.height * moveDetectPerc);

    // Check left side of screen but don't move it if is past the screen
    if (Input.mousePosition.x <= moveDetectThreshold.x && Input.mousePosition.x >= 0)
    {
      gameObject.transform.Translate(Vector3.right * -moveSpeed * Time.deltaTime / GetZoomPerc());
    }

    // Check right side of screen
    else if (Input.mousePosition.x >= Screen.width - moveDetectThreshold.x && Input.mousePosition.x <= Screen.width)
    {
      gameObject.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime / GetZoomPerc());
    }

    // Check top side of screen
    if (Input.mousePosition.y <= moveDetectThreshold.y && Input.mousePosition.y >= 0)
    {
      gameObject.transform.Translate(Quaternion.Euler(0, 90, 0) * transform.right * moveSpeed * Time.deltaTime / GetZoomPerc(), Space.World);
    }

    // Check bottom side of screen
    else if (Input.mousePosition.y >= Screen.height - moveDetectThreshold.y && Input.mousePosition.y <= Screen.height)
    {
      gameObject.transform.Translate(Quaternion.Euler(0, -90, 0) * transform.right * moveSpeed * Time.deltaTime / GetZoomPerc(), Space.World);
    }
  }

  private void Pan()
  {
    // Check for input on left keys
    if (Input.GetKey("a") || Input.GetKey("left"))
    {
      gameObject.transform.Rotate(0, -panSpeed * Time.deltaTime, 0, Space.World);
    }

    // Check for input on right keys
    else if (Input.GetKey("d") || Input.GetKey("right"))
    {
      gameObject.transform.Rotate(0, panSpeed * Time.deltaTime, 0, Space.World);
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

  public float GetZoomPerc()
  {
    return MAX_ZOOM / transform.position.y;
  }
}
