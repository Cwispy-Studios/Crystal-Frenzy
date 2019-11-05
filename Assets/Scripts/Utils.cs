using UnityEngine;

public class Utils
{
  static Texture2D _whiteTexture;
  public static Texture2D WhiteTexture
  {
    get
    {
      if (_whiteTexture == null)
      {
        _whiteTexture = new Texture2D(1, 1);
        _whiteTexture.SetPixel(0, 0, Color.white);
        _whiteTexture.Apply();
      }

      return _whiteTexture;
    }
  }

  public static Rect GetScreenRect(Vector3 mousePos1, Vector3 mousePos2)
  {
    // Move origin from bottom left to top left
    mousePos1.y = Screen.height - mousePos1.y;
    mousePos2.y = Screen.height - mousePos2.y;

    // Calculate corners
    Vector3 topLeft = Vector3.Min(mousePos1, mousePos2);
    Vector3 bottomRight = Vector3.Max(mousePos1, mousePos2);

    // Create Rect
    return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
  }

  public static void DrawScreenRect(Rect rect)
  {
    GUI.color = Color.green;
    GUI.DrawTexture(rect, WhiteTexture);
  }

  public static void DrawScreenRectBorder(Rect rect, float thickness)
  {
    // Top
    DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness));
    // Left
    DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height));
    // Right
    DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height));
    // Bottom
    DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness));
  }

  public static GameObject CheckMouseIsOverSelectable(Camera playerCamera)
  {
    Ray ray;

    // Retrieve the ray from the mouse to camera
    ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    // Do not count UI, Terrain, FOV and Invisible objects
    int layerMask = (1 << 5) | (1 << 8) | (1 << 9) | (1 << 10);

    float distance = playerCamera.GetComponent<CameraControls>().birdsEyeViewMode ? 1200f : 150f;

    // Check if the mouse was over any collider when clicked
    if (Physics.Raycast(ray, out RaycastHit hit, distance, ~layerMask))
    {
      // Retrieve selectable component
      GameObject selectable = hit.collider.gameObject;

      // Check if the object is selectable
      if (selectable != null)
      {
        return selectable;
      }
    }

    return null;
  }

  public 
    float RandomGaussian()
  {
    return 1f;
  }
}
