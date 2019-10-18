using UnityEngine;

public class CameraManager : MonoBehaviour
{
  private readonly Vector3 BIRDS_EYE_VIEW_ROT = new Vector3(90f, 0f, 0f);
  private const float BIRDS_EYE_VIEW_ORT_SIZE = 40f;

  private Vector3 lastCamRot;

  public void SetBirdsEyeView()
  {
    // Extra safety check
    if (GameManager.currentPhase == PHASES.PREPARATION)
    {
      lastCamRot = transform.rotation.eulerAngles;

      transform.eulerAngles = BIRDS_EYE_VIEW_ROT;
      GetComponent<Camera>().orthographic = true;
      GetComponent<Camera>().orthographicSize = BIRDS_EYE_VIEW_ORT_SIZE;
    }
  }

  public void SetNormalView()
  {
    transform.eulerAngles = lastCamRot;
    GetComponent<Camera>().orthographic = false;
  }

  public void PanToNode()
  {
    // 
  }
}
