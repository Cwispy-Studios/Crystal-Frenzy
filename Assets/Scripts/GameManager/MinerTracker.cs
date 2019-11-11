using UnityEngine;
using UnityEngine.UI;

public class MinerTracker : MonoBehaviour
{
  [SerializeField]
  private Camera minerTracker = null;
  [SerializeField]
  private Image minerTrackerFrame = null;
  [SerializeField]
  private RawImage minerTrackerImage = null;

  [SerializeField]
  private Image topPanel = null, bottomPanel = null;

  private GameObject miner;
  private Camera playerCamera;

  private float screenWidth;
  private float screenHeight;

  private float maxWidthOffset;
  private float maxHeightOffset;

  private float spriteWidthOffset;
  private float spriteHeightOffset;

  private float topViewportLimit;
  private float bottomViewportLimit;
  private float topScreenLimit;
  private float bottomScreenLimit;

  private void Awake()
  {
    playerCamera = Camera.main;

    // Calculate the playing screen size based on the height of the top and bottom panels
    screenWidth = Screen.width;
    screenHeight = Screen.height - topPanel.sprite.rect.height - bottomPanel.sprite.rect.height;

    maxWidthOffset = (screenWidth / 2f);
    maxHeightOffset = (screenHeight / 2f);

    spriteWidthOffset = (minerTrackerFrame.sprite.rect.width / 2f);
    spriteHeightOffset = (minerTrackerFrame.sprite.rect.height / 2f);

    topViewportLimit = 1f - (topPanel.sprite.rect.height / Screen.height);
    bottomViewportLimit = bottomPanel.sprite.rect.height / Screen.height;

    topScreenLimit = Screen.height - topPanel.sprite.rect.height;
    bottomScreenLimit = bottomPanel.sprite.rect.height;

    //centerHeight = (Screen.height / 2f) + (bottomPanel.sprite.rect.height / 2f) - (topPanel.sprite.rect.height / 2f);
  }

  private void Update()
  {
    // In Escort Phase and miner still alive
    if (miner != null)
    {
      // Point the camera at the miner
      PointMinerTrackerAtMiner();

      // Check if the miner is off the screen
      Vector3 minerViewportPoint = playerCamera.WorldToViewportPoint(miner.transform.position);

      //Vector3 bottomLeft = playerCamera.ScreenToWorldPoint(Vector3.zero);
      //Debug.Log(bottomLeft);
      //Vector3 bottomRight = playerCamera.ScreenToWorldPoint(new Vector2(Screen.width, 0));
      //Vector3 topLeft = playerCamera.ScreenToWorldPoint(Vector3.zero);
      //Vector3 topRight = playerCamera.ScreenToWorldPoint(new Vector2(Screen.width, 0));

      // Miner is out of viewport
      if (minerViewportPoint.x < 0 || minerViewportPoint.x > 1 || minerViewportPoint.y < bottomViewportLimit || minerViewportPoint.y > topViewportLimit)
      {
        // Show the tracker frame
        minerTrackerFrame.gameObject.SetActive(true);

        // Find the center of the camera
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f + bottomViewportLimit - topViewportLimit, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, 1 << 8))
        {
          Vector3 centerOfScreen = hit.point;
          centerOfScreen.y = 0;
          Vector3 minerPos = miner.transform.position;
          minerPos.y = 0;

          Vector3 bottomLeftOfScreen = Vector3.zero;
          Vector3 bottomRightOfScreen = Vector3.zero;
          Vector3 topLeftOfScreen = Vector3.zero;
          Vector3 topRightOfScreen = Vector3.zero;

          // Find the bottom left of the camera
          ray = playerCamera.ViewportPointToRay(new Vector3(0, bottomViewportLimit, 0));

          if (Physics.Raycast(ray, out hit, 100f, 1 << 8))
          {
            bottomLeftOfScreen = hit.point;
          }

          // Find the bottom right of the camera
          ray = playerCamera.ViewportPointToRay(new Vector3(1, bottomViewportLimit, 0));

          if (Physics.Raycast(ray, out hit, 100f, 1 << 8))
          {
            bottomRightOfScreen = hit.point;
          }

          // Find the top left of the camera
          ray = playerCamera.ViewportPointToRay(new Vector3(0, topViewportLimit, 0));

          if (Physics.Raycast(ray, out hit, 100f, 1 << 8))
          {
            topLeftOfScreen = hit.point;
          }

          // Find the top right of the camera
          ray = playerCamera.ViewportPointToRay(new Vector3(1, topViewportLimit, 0));

          if (Physics.Raycast(ray, out hit, 100f, 1 << 8))
          {
            topRightOfScreen = hit.point;
          }

          bottomLeftOfScreen.y = 0;
          bottomRightOfScreen.y = 0;
          topLeftOfScreen.y = 0;
          topRightOfScreen.y = 0;

          Debug.Log("Bottom left: " + bottomLeftOfScreen + "Bottom right: " + bottomRightOfScreen + "Top left: " + topLeftOfScreen + "Top Right: " + topRightOfScreen + "Center: " + centerOfScreen);

          Vector3 intersectionPoint = Vector3.zero;

          bool intersectionPointFound = false;

          Vector2 trackerFramePos = Vector3.zero;

          // Check against every border of the screen for collision
          // Check bottom border
          if (minerPos.z < centerOfScreen.z)
          {
            if (LineLineIntersection(out intersectionPoint, bottomLeftOfScreen, bottomRightOfScreen - bottomLeftOfScreen, centerOfScreen, minerPos - centerOfScreen) && intersectionPoint.x >= bottomLeftOfScreen.x && intersectionPoint.x <= bottomRightOfScreen.x)
            {
              intersectionPointFound = true;
              float xAxisAway = intersectionPoint.x - bottomLeftOfScreen.x;
              float xAxisLength = bottomRightOfScreen.x - bottomLeftOfScreen.x;
              trackerFramePos.x = (xAxisAway / xAxisLength) * (maxWidthOffset * 2f) - maxWidthOffset;
              trackerFramePos.y = bottomScreenLimit;
              Debug.Log("Bottom");
            }
          }

          // Check top border
          else if (minerPos.z > centerOfScreen.z)
          {
            if (LineLineIntersection(out intersectionPoint, topRightOfScreen, topLeftOfScreen - topRightOfScreen, centerOfScreen, minerPos - centerOfScreen) && intersectionPoint.x >= topLeftOfScreen.x && intersectionPoint.x <= topRightOfScreen.x)
            {
              intersectionPointFound = true;
              trackerFramePos.y = topScreenLimit;
              Debug.Log("Top");
            }
          }

          // Check x-axis borders if intersection not found yet
          if (!intersectionPointFound)
          {
            if (minerPos.x > centerOfScreen.x)
            {
              // Check right border
              if (LineLineIntersection(out intersectionPoint, bottomRightOfScreen, topRightOfScreen - bottomRightOfScreen, centerOfScreen, minerPos - centerOfScreen))
              {
                intersectionPointFound = true;
                trackerFramePos.x = maxWidthOffset;
                trackerFramePos.y = centerOfScreen.z;
                Debug.Log("Right");
              }
            }
            
            else if (minerPos.x < centerOfScreen.x)
            {
              // Check left border
              if (LineLineIntersection(out intersectionPoint, topLeftOfScreen, bottomLeftOfScreen - topLeftOfScreen, centerOfScreen, minerPos - centerOfScreen))
              {
                intersectionPointFound = true;
                trackerFramePos.x = -maxWidthOffset;
                trackerFramePos.y = centerOfScreen.z;
                Debug.Log("Left");
              }
            }
          }

          Debug.Log(intersectionPoint);

          Vector2 minerPos2D = new Vector2 { x = minerPos.x - centerOfScreen.x , y = minerPos.z - centerOfScreen.z };

          float angle = Vector2.SignedAngle(Vector2.right, minerPos2D);
          float angleRad = angle * Mathf.Deg2Rad;

          // Set the frame's angle
          minerTrackerFrame.transform.eulerAngles = new Vector3(0, 0, angle);

          minerTrackerFrame.GetComponent<RectTransform>().anchoredPosition = trackerFramePos;
          minerTrackerImage.transform.eulerAngles = Vector2.zero;
        }
      }

      else
      {
        minerTrackerFrame.gameObject.SetActive(false);
      }
    }

    else
    {
      minerTrackerFrame.gameObject.SetActive(false);
    }
  }

  public void TrackMiner(GameObject newMiner)
  {
    miner = newMiner;
  }

  private void PointMinerTrackerAtMiner()
  {
    Vector3 toPos = miner.transform.position;

    toPos.y = CameraControls.DEFAULT_ZOOM;
    toPos.z -= toPos.y / Mathf.Tan(CameraManager.DEFAULT_ROT.x * Mathf.Deg2Rad);

    minerTracker.transform.position = toPos;
  }

  private bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
  {
    intersection = Vector3.zero;

    Vector3 lineVec3 = linePoint2 - linePoint1;
    Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
    Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

    float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

    //is coplanar, and not parallel
    if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
    {
      float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
      intersection = linePoint1 + (lineVec1 * s);

      return true;
    }

    else
    {
      intersection = Vector3.zero;

      return false;
    }
  }

  // Distance to point (p) from line segment (end points a b)
  private float DistanceSqrLineSegmentPoint(Vector3 a, Vector3 b, Vector3 p)
  {
    // If a == b line segment is a point and will cause a divide by zero in the line segment test.
    // Instead return distance from a
    if (a == b)
      return Vector3.Distance(a, p);

    // Line segment to point distance equation
    Vector3 ba = b - a;
    Vector3 pa = a - p;
    Vector3 c = ba * (Vector3.Dot(ba, pa) / Vector3.Dot(ba, ba));
    Vector3 d = pa - c;

    float d2 = Vector3.Dot(d, d);
    return d2;
  }

}
