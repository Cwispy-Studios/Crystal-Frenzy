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

  [SerializeField]
  private UIInterface uiInterface = null;

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

  private enum INTERSECTION_BORDER
  {
    BOTTOM = 0,
    TOP,
    RIGHT,
    LEFT,
    LAST
  }

  private void Awake()
  {
    Vector2 resolution = uiInterface.GetComponent<CanvasScaler>().referenceResolution;
    playerCamera = Camera.main;

    spriteWidthOffset = (minerTrackerFrame.GetComponent<RectTransform>().sizeDelta.x / 2f);
    spriteHeightOffset = (minerTrackerFrame.GetComponent<RectTransform>().sizeDelta.y / 2f);

    // Calculate the playing screen size based on the height of the top and bottom panels
    screenWidth = resolution.x;
    screenHeight = resolution.y - topPanel.GetComponent<RectTransform>().sizeDelta.y - bottomPanel.GetComponent<RectTransform>().sizeDelta.y;

    maxWidthOffset = (screenWidth / 2f) - spriteWidthOffset;
    maxHeightOffset = screenHeight;

    topViewportLimit = 1f - (topPanel.GetComponent<RectTransform>().sizeDelta.y / resolution.y);
    bottomViewportLimit = bottomPanel.GetComponent<RectTransform>().sizeDelta.y / resolution.y;

    topScreenLimit = resolution.y - topPanel.GetComponent<RectTransform>().sizeDelta.y;
    bottomScreenLimit = bottomPanel.GetComponent<RectTransform>().sizeDelta.y;
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

      // Miner is out of viewport
      if (minerViewportPoint.x < 0 || minerViewportPoint.x > 1 || minerViewportPoint.y < bottomViewportLimit || minerViewportPoint.y > topViewportLimit)
      {
        // Show the tracker frame
        minerTrackerFrame.gameObject.SetActive(true);

        // Find the center of the camera
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f + bottomViewportLimit - (1f - topViewportLimit), 0));

        if (Physics.Raycast(ray, out RaycastHit hit, 150f, 1 << 11))
        {
          Vector3 centerOfScreen = hit.point;
          centerOfScreen.y = 0;
          Vector3 minerPos = miner.transform.position;
          minerPos.y = 0;

          Vector2 minerPos2D = new Vector2 { x = minerPos.x - centerOfScreen.x, y = minerPos.z - centerOfScreen.z };

          float angle = Vector2.SignedAngle(Vector2.right, minerPos2D) + playerCamera.transform.eulerAngles.y;
          float angleRad = angle * Mathf.Deg2Rad;

          // Set the frame's angle
          minerTrackerFrame.transform.eulerAngles = new Vector3(0, 0, angle);
          minerTrackerImage.transform.eulerAngles = Vector2.zero;

          Vector3 bottomLeftOfScreen = Vector3.zero;
          Vector3 bottomRightOfScreen = Vector3.zero;
          Vector3 topLeftOfScreen = Vector3.zero;
          Vector3 topRightOfScreen = Vector3.zero;

          // Find the bottom left of the camera
          ray = playerCamera.ViewportPointToRay(new Vector3(0, bottomViewportLimit, 0));

          if (Physics.Raycast(ray, out hit, 100f, 1 << 11))
          {
            bottomLeftOfScreen = hit.point;
          }

          // Find the bottom right of the camera
          ray = playerCamera.ViewportPointToRay(new Vector3(1, bottomViewportLimit, 0));

          if (Physics.Raycast(ray, out hit, 100f, 1 << 11))
          {
            bottomRightOfScreen = hit.point;
          }

          // Find the top left of the camera
          ray = playerCamera.ViewportPointToRay(new Vector3(0, topViewportLimit, 0));

          if (Physics.Raycast(ray, out hit, 175f, 1 << 11))
          {
            topLeftOfScreen = hit.point;
          }

          // Find the top right of the camera
          ray = playerCamera.ViewportPointToRay(new Vector3(1, topViewportLimit, 0));

          if (Physics.Raycast(ray, out hit, 175f, 1 << 11))
          {
            topRightOfScreen = hit.point;
          }

          bottomLeftOfScreen.y = 0;
          bottomRightOfScreen.y = 0;
          topLeftOfScreen.y = 0;
          topRightOfScreen.y = 0;

          Vector3 intersectionPoint = Vector3.zero;

          Vector2 trackerFramePos = Vector3.zero;

          Debug.DrawLine(bottomLeftOfScreen, bottomRightOfScreen, Color.blue);
          Debug.DrawLine(bottomRightOfScreen, topRightOfScreen, Color.blue);
          Debug.DrawLine(topRightOfScreen, topLeftOfScreen, Color.blue);
          Debug.DrawLine(topLeftOfScreen, bottomLeftOfScreen, Color.blue);
          Debug.DrawLine(centerOfScreen, minerPos, Color.yellow);

          Vector3[] intersectionPoints = new Vector3[(int)INTERSECTION_BORDER.LAST];
          bool[] intersects = new bool[(int)INTERSECTION_BORDER.LAST];

          for (int i = 0; i < (int)INTERSECTION_BORDER.LAST; ++i)
          {
            intersectionPoints[i] = Vector3.zero;
            intersects[i] = false;
          }

          // Check against every border of the screen for collision
          // Check bottom border
          if (LineLineIntersection(out intersectionPoints[(int)INTERSECTION_BORDER.BOTTOM], bottomRightOfScreen, bottomLeftOfScreen - bottomRightOfScreen, centerOfScreen, minerPos - centerOfScreen) &&
            PointLiesOnLine(bottomLeftOfScreen, bottomRightOfScreen, intersectionPoints[(int)INTERSECTION_BORDER.BOTTOM]))
          {
            intersects[(int)INTERSECTION_BORDER.BOTTOM] = true;
          }

          // Check top border
          if (LineLineIntersection(out intersectionPoints[(int)INTERSECTION_BORDER.TOP], topRightOfScreen, topLeftOfScreen - topRightOfScreen, centerOfScreen, minerPos - centerOfScreen) &&
            PointLiesOnLine(topLeftOfScreen, topRightOfScreen, intersectionPoints[(int)INTERSECTION_BORDER.TOP]))
          {
            intersects[(int)INTERSECTION_BORDER.TOP] = true;
          }

          // Check right border
          if (LineLineIntersection(out intersectionPoints[(int)INTERSECTION_BORDER.RIGHT], bottomRightOfScreen, topRightOfScreen - bottomRightOfScreen, centerOfScreen, minerPos - centerOfScreen) &&
            PointLiesOnLine(bottomRightOfScreen, topRightOfScreen, intersectionPoints[(int)INTERSECTION_BORDER.RIGHT]))
          {
            intersects[(int)INTERSECTION_BORDER.RIGHT] = true;
          }

          // Check left border
          if (LineLineIntersection(out intersectionPoints[(int)INTERSECTION_BORDER.LEFT], topLeftOfScreen, bottomLeftOfScreen - topLeftOfScreen, centerOfScreen, minerPos - centerOfScreen) &&
            PointLiesOnLine(bottomLeftOfScreen, topLeftOfScreen, intersectionPoints[(int)INTERSECTION_BORDER.LEFT]))
          {
            intersects[(int)INTERSECTION_BORDER.LEFT] = true;
          }

          float shortestSqrDistance = 999999999f;
          INTERSECTION_BORDER closestBorder = INTERSECTION_BORDER.LAST;

          // Compare the distance from the intersection points to the miner position and grab the closest one
          for (int i = 0; i < (int)INTERSECTION_BORDER.LAST; ++i)
          {
            if (intersects[i])
            {
              float sqrMag = (intersectionPoints[i] - minerPos).sqrMagnitude;

              if (sqrMag < shortestSqrDistance)
              {
                shortestSqrDistance = sqrMag;
                closestBorder = (INTERSECTION_BORDER)i;
                intersectionPoint = intersectionPoints[i];
              }
            }
          }

          float xAxisAway;
          float xAxisLength;
          float zAxisAway;
          float zAxisLength;

          switch (closestBorder)
          {
            case INTERSECTION_BORDER.BOTTOM:
              xAxisAway = intersectionPoint.x - bottomLeftOfScreen.x;
              xAxisLength = bottomRightOfScreen.x - bottomLeftOfScreen.x;
              trackerFramePos.x = (xAxisAway / xAxisLength) * (maxWidthOffset * 2f) - maxWidthOffset;

              trackerFramePos.y = bottomScreenLimit;
              trackerFramePos.y -= (Mathf.Sin(angle * Mathf.Deg2Rad) * spriteHeightOffset);
              Debug.Log("Bottom");
              break;

            case INTERSECTION_BORDER.TOP:
              xAxisAway = intersectionPoint.x - topLeftOfScreen.x;
              xAxisLength = topRightOfScreen.x - topLeftOfScreen.x;
              trackerFramePos.x = (xAxisAway / xAxisLength) * (maxWidthOffset * 2f) - maxWidthOffset;

              trackerFramePos.y = topScreenLimit;
              trackerFramePos.y -= (Mathf.Sin(angle * Mathf.Deg2Rad) * spriteHeightOffset);
              Debug.Log("Top");
              break;

            case INTERSECTION_BORDER.RIGHT:
              trackerFramePos.x = maxWidthOffset + spriteWidthOffset;
              trackerFramePos.x -= Mathf.Cos(angle * Mathf.Deg2Rad) * spriteWidthOffset;

              if (intersectionPoint.z < 0)
              {
                zAxisAway = 0;
              }

              else if (intersectionPoint.z > topRightOfScreen.z)
              {
                zAxisAway = topRightOfScreen.z;
              }

              else
              {
                zAxisAway = intersectionPoint.z - bottomRightOfScreen.z;
              }


              zAxisLength = topRightOfScreen.z - bottomRightOfScreen.z;
              trackerFramePos.y = bottomPanel.sprite.rect.height + spriteHeightOffset + ((zAxisAway / zAxisLength) * maxHeightOffset);
              Debug.Log("Right");
              break;

            case INTERSECTION_BORDER.LEFT:
              trackerFramePos.x = -maxWidthOffset - spriteWidthOffset;
              trackerFramePos.x -= Mathf.Cos(angle * Mathf.Deg2Rad) * spriteWidthOffset;

              if (intersectionPoint.z < 0)
              {
                zAxisAway = 0;
              }

              else if (intersectionPoint.z > topLeftOfScreen.z)
              {
                zAxisAway = topLeftOfScreen.z;
              }

              else
              {
                zAxisAway = intersectionPoint.z - bottomLeftOfScreen.z;
              }

              zAxisLength = topLeftOfScreen.z - bottomLeftOfScreen.z;
              trackerFramePos.y = bottomPanel.sprite.rect.height + spriteHeightOffset + ((zAxisAway / zAxisLength) * maxHeightOffset);
              Debug.Log("Left");
              break;
          }

          Debug.DrawLine(intersectionPoint, centerOfScreen, Color.red);

          minerTrackerFrame.GetComponent<RectTransform>().anchoredPosition = trackerFramePos;    
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

    toPos.y = 10f;
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

  private bool PointLiesOnLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
  {
    float linePointX = point.x - lineStart.x;
    float linePointZ = point.z - lineStart.z;

    float lineX = lineEnd.x - lineStart.x;
    float lineZ = lineEnd.z - lineStart.z;

    if (Mathf.Abs(lineX) >= Mathf.Abs(lineZ))
    {
      return lineX > 0 ?
        lineStart.x <= point.x && point.x <= lineEnd.x :
        lineEnd.x <= point.x && point.x <= lineStart.x;
    }

    else
    {
      return lineZ > 0 ?
        lineStart.z <= point.z && point.z <= lineEnd.z :
        lineEnd.z <= point.z && point.z <= lineStart.z;
    }
  }
}
