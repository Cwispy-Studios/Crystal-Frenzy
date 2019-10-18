using UnityEngine;

//[ExecuteAlways]
public class TestCamera : MonoBehaviour
{
  private const float height = 30f;
  private const float camera_rot = 55f;
  private float x_mag = height / Mathf.Tan(camera_rot * Mathf.Deg2Rad);
  public GameObject lookAt;

  private void Update()
  {
    //Vector3 cameraPos = (lookAt.transform.position -lookAt.transform.forward * x_mag);
    //cameraPos.y = height;
    //transform.position = cameraPos;
    //transform.LookAt(lookAt.transform);
  }
}
