/******************************************************************************/
/*!
\file   CameraManager.cs
\author Wong Zhihao, tomvds
\par    email: wongzhihao.student.utwente.nl
\date   18 October 2019
\brief

  Brief:
    Lerp implementation from: https://forum.unity.com/threads/smooth-transition-between-perspective-and-orthographic-modes.32765/
*/
/******************************************************************************/

using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
  public bool cameraLerping = false;

  private readonly Vector3 DEFAULT_ROT = new Vector3(55f, 0f, 0f);
  private const float DEFAULT_FOV = 60f;
  private readonly Vector3 BIRDS_EYE_VIEW_ROT = new Vector3(90f, 0f, 0f);
  private const float BIRDS_EYE_VIEW_FOV = 10f;
  private const float BIRDS_EYE_VIEW_Y = 990;

  public bool mouseOverUI = false;
  public bool selectionDisabled = false;

  private Vector3 lastCamRot;

  [SerializeField]
  private FMODUnity.StudioEventEmitter ambienceEmitter = null;

  public void PointCameraAtPosition(Vector3 pointPos, bool birdsEyeView, bool maintainHeightRot, float targetHeight = 0, float duration = 1.2f, bool turnOnCamControls = true, bool cheat = false)
  {
    GetComponent<CameraControls>().birdsEyeViewMode = birdsEyeView;

    if (birdsEyeView)
    {
      ambienceEmitter.SetParameter("BirdsEyeView", 1);
    }

    else
    {
      ambienceEmitter.SetParameter("BirdsEyeView", 0);
    }

    Vector3 toPos = pointPos;
    Vector3 rot;
    float fov;

    if (!birdsEyeView)
    {
      if (maintainHeightRot)
      {
        float correctedY = transform.position.y;

        if (targetHeight != 0)
        {
          correctedY = targetHeight;
        }

        if (transform.position.y > CameraControls.MAX_ZOOM)
        {
          correctedY = CameraControls.MAX_ZOOM;
        }

        Vector3 normalisedDirectionVec = new Vector3(Mathf.Sin(transform.eulerAngles.y * Mathf.Deg2Rad), 0, Mathf.Cos(transform.eulerAngles.y * Mathf.Deg2Rad));
        float distanceAwayFromPos = correctedY / Mathf.Tan(DEFAULT_ROT.x * Mathf.Deg2Rad);

        // Use the current rotation and camera height
        toPos.x -= distanceAwayFromPos * normalisedDirectionVec.x;
        toPos.z -= distanceAwayFromPos * normalisedDirectionVec.z;
        toPos.y = correctedY;

        rot = transform.rotation.eulerAngles;
        rot.x = DEFAULT_ROT.x;
      }

      else
      {
        if (targetHeight != 0)
        {
          toPos.y = targetHeight;
        }

        else
        {
          toPos.y = CameraControls.MAX_ZOOM;
        }

        toPos.z -= toPos.y / Mathf.Tan(DEFAULT_ROT.x * Mathf.Deg2Rad);

        rot = DEFAULT_ROT;
      }

      fov = DEFAULT_FOV;
    }

    else
    {
      toPos.y = BIRDS_EYE_VIEW_Y;
      rot = BIRDS_EYE_VIEW_ROT;
      fov = BIRDS_EYE_VIEW_FOV;

      if (cheat)
      {
        toPos.y = targetHeight;
      }
    }    

    StartLerp(toPos, rot, fov, duration, turnOnCamControls);
  }

  private IEnumerator LerpCamera(Vector3 fromPos, Vector3 toPos, Vector3 fromRot, Vector3 toRot, float fromFov, float toFov, float duration, bool turnOnCamControls)
  {
    float startTime = Time.time;

    while (Time.time - startTime < duration)
    {
      cameraLerping = true;
      GetComponent<CameraControls>().enabled = false;

      transform.position = Vector3.Lerp(fromPos, toPos, (Time.time - startTime) / duration);
      transform.eulerAngles = Vector3.Lerp(fromRot, toRot, (Time.time - startTime) / duration);
      GetComponent<Camera>().fieldOfView = Mathf.Lerp(fromFov, toFov, (Time.time - startTime) / duration);

      yield return 1;
    }

    cameraLerping = false;
    GetComponent<CameraControls>().enabled = turnOnCamControls;

    transform.position = toPos;
    transform.eulerAngles = toRot;
    GetComponent<Camera>().fieldOfView = toFov;
  }

  private Coroutine StartLerp(Vector3 toPos, Vector3 toRot, float toFov, float duration, bool turnOnCamControls)
  {
    StopAllCoroutines();
    return StartCoroutine(LerpCamera(transform.position, toPos, transform.rotation.eulerAngles, toRot, GetComponent<Camera>().fieldOfView, toFov, duration, turnOnCamControls));
  }

  public void RotateAroundObject(GameObject crystal)
  {
    StartRotate(crystal);
  }

  private IEnumerator RotateCamera(GameObject rotateAround)
  {
    while (true)
    {
      while (!cameraLerping)
      {
        transform.RotateAround(rotateAround.transform.position, Vector3.up, 20f * Time.deltaTime);

        yield return 1;
      }

      yield return 1;
    }
  }

  private Coroutine StartRotate(GameObject rotateAround)
  {
    return StartCoroutine(RotateCamera(rotateAround));
  }
}
