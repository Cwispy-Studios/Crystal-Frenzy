using UnityEngine;

public class CameraVisibility : MonoBehaviour
{
  public GameObject prefab;

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.G))
    {
      prefab.GetComponent<Health>().MaxHealth += 10;
    }


  }
}
