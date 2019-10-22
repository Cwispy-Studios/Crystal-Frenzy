using UnityEngine;

public class CameraManager : MonoBehaviour
{
  private readonly Vector3 BIRDS_EYE_VIEW_ROT = new Vector3(90f, 0f, 0f);
  private const float BIRDS_EYE_VIEW_ORT_SIZE = 50f;
  private const float BIRDS_EYE_VIEW_ORT_ASSEMBLY_SIZE = 30f;

  private Vector3 lastCamRot;

  public void SetBirdsEyeView()
  {
    // Extra safety check
    if (GameManager.CurrentPhase == PHASES.PREPARATION || GameManager.CurrentPhase == PHASES.PREPARATION_DEFENSE)
    {
      lastCamRot = transform.rotation.eulerAngles;

      transform.eulerAngles = BIRDS_EYE_VIEW_ROT;
      GetComponent<Camera>().orthographic = true;

      if (GameManager.CurrentPhase == PHASES.PREPARATION)
      {
        GetComponent<Camera>().orthographicSize = BIRDS_EYE_VIEW_ORT_SIZE;
      }
      
      else
      {
        SetAssemblyOrthographicSize();
      }
    }
  }

  public void SetAssemblyOrthographicSize()
  {
    GetComponent<Camera>().orthographicSize = BIRDS_EYE_VIEW_ORT_ASSEMBLY_SIZE;
  }

  public void SetNodeSelectionOrthographicSize()
  {
    GetComponent<Camera>().orthographicSize = BIRDS_EYE_VIEW_ORT_SIZE;
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
