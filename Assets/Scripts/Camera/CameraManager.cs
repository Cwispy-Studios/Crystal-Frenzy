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
  private readonly Vector3 DEFAULT_ROT = new Vector3(55f, 0f, 0f);
  private const float DEFAULT_FOV = 60f;
  private readonly Vector3 BIRDS_EYE_VIEW_ROT = new Vector3(90f, 0f, 0f);
  private const float BIRDS_EYE_VIEW_FOV = 10f;
  private const float BIRDS_EYE_VIEW_Y = 450f;

  private Vector3 lastCamRot;

  public void SetBirdsEyeView(Vector3 setPosition)
  {
    Camera camera = GetComponent<Camera>();

    // Extra safety check
    if (GameManager.CurrentPhase == PHASES.PREPARATION || GameManager.CurrentPhase == PHASES.PREPARATION_DEFENSE)
    {
      GetComponent<CameraControls>().birdsEyeViewMode = true;

      Vector3 toPos = setPosition;
      toPos.y = BIRDS_EYE_VIEW_Y;

      StartLerp(toPos, BIRDS_EYE_VIEW_ROT, BIRDS_EYE_VIEW_FOV, 1.5f);
    }
  }

  public void PointCameraAtAssembly(Vector3 assemblyPos)
  {
    GetComponent<CameraControls>().birdsEyeViewMode = false;

    Vector3 toPos = assemblyPos;
    toPos.z -= CameraControls.MAX_ZOOM / Mathf.Tan(DEFAULT_ROT.x * Mathf.Deg2Rad);
    toPos.y = CameraControls.MAX_ZOOM;

    StartLerp(toPos, DEFAULT_ROT, DEFAULT_FOV, 1.5f);
  }

  private IEnumerator LerpCamera(Vector3 fromPos, Vector3 toPos, Vector3 fromRot, Vector3 toRot, float fromFov, float toFov, float duration)
  {
    float startTime = Time.time;

    while (Time.time - startTime < duration)
    {
      transform.position = Vector3.Lerp(fromPos, toPos, (Time.time - startTime) / duration);
      transform.eulerAngles = Vector3.Lerp(fromRot, toRot, (Time.time - startTime) / duration);
      GetComponent<Camera>().fieldOfView = Mathf.Lerp(fromFov, toFov, (Time.time - startTime) / duration);

      yield return null;
    }

    transform.position = toPos;
    transform.eulerAngles = toRot;
    GetComponent<Camera>().fieldOfView = toFov;
  }

  private Coroutine StartLerp(Vector3 toPos, Vector3 toRot, float toFov, float duration)
  {
    StopAllCoroutines();
    return StartCoroutine(LerpCamera(transform.position, toPos, transform.rotation.eulerAngles, toRot, GetComponent<Camera>().fieldOfView, toFov, duration));
  }
}
