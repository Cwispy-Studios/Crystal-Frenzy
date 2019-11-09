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

  private float centerHeight;

  private float topAxisChangeAngle;
  private float bottomAxisChangeAngle;

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

    centerHeight = (Screen.height / 2f) + (bottomPanel.sprite.rect.height / 2f) - (topPanel.sprite.rect.height / 2f);

    // Find the angle where the tracker should change which side of the screen it locks on to
    topAxisChangeAngle = Vector2.Angle(Vector2.right, new Vector2(screenWidth, (maxHeightOffset - topPanel.sprite.rect.height) - centerHeight));
    bottomAxisChangeAngle = Vector2.Angle(Vector2.right, new Vector2(screenWidth, (maxHeightOffset - bottomPanel.sprite.rect.height) - centerHeight));
  }

  private void Update()
  {
    // In Escort Phase and miner still alive
    if (miner != null)
    {
      // Point the camera at the miner
      PointMinerTrackerAtMiner();

      // Check if the miner is off the screen
      Vector3 minerViewportPoint = playerCamera.WorldToScreenPoint(miner.transform.position);

      if (minerViewportPoint.x < 0 || minerViewportPoint.x > 1 || minerViewportPoint.y < 0 || minerViewportPoint.y > 1)
      {
        // Show the tracker frame
        minerTrackerFrame.gameObject.SetActive(true);

        // Calculate the angle from the middle of the screen to the miner
        Vector2 minerViewportPoint2DCentered = minerViewportPoint;
        minerViewportPoint2DCentered.x -= 0.5f;
        minerViewportPoint2DCentered.y -= 0.5f;

        float angle = Vector2.SignedAngle(Vector2.right, minerViewportPoint2DCentered);
        float angleRad = angle * Mathf.Deg2Rad;

        // Set the frame's angle
        minerTrackerFrame.transform.eulerAngles = new Vector3(0, 0, angle);

        // Set the frame's offset, identify which part of the screen it should lock to
        // Lock to right side, so x-axis is locked
        if ((angle >= 0 && angle < topAxisChangeAngle) || (angle >= -bottomAxisChangeAngle && angle < 0))
        {
          minerTrackerFrame.GetComponent<RectTransform>().anchoredPosition = new Vector2(maxWidthOffset, centerHeight + (Mathf.Sin(angleRad) * maxHeightOffset));
        }

        // Lock to top side, so y-axis is locked
        else if (angle >= topAxisChangeAngle && angle < 180f - topAxisChangeAngle)
        {
          minerTrackerFrame.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Cos(angleRad) * maxWidthOffset, centerHeight + maxHeightOffset);
        }

        // Lock to left side, so x-axis is locked
        else if ((angle >= 180f - topAxisChangeAngle && angle < 180f) || (angle >= -180f && angle < -180f + bottomAxisChangeAngle))
        {
          minerTrackerFrame.GetComponent<RectTransform>().anchoredPosition = new Vector2(-maxWidthOffset, centerHeight + (Mathf.Sin(angleRad) * maxHeightOffset));
        }

        else if (angle >= -180f + bottomAxisChangeAngle && angle < -bottomAxisChangeAngle)
        {
          minerTrackerFrame.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Cos(angleRad) * maxWidthOffset, centerHeight - maxHeightOffset);
        }

        minerTrackerImage.transform.eulerAngles = Vector2.zero;
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
}
