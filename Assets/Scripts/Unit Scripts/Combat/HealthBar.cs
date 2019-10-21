/******************************************************************************/
/*!
\file   HealthBar.cs
\author Unity3d College, Wong Zhihao
\par    email: wongzhihao.student.utwente.nl
\date   15 October 2019
\brief

  Brief:
    Implementation from: https://www.youtube.com/watch?v=kQqqo_9FfsU&t=396s
*/
/******************************************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
  [SerializeField]
  private Image healthBarImage = null;

  private Camera playerCamera;
  private Health health;

  private Vector3 originalScale;
  private float objectSize = 1f;
  private float heightOffset = 0f;

  private void Awake()
  {
    playerCamera = Camera.main;
  }

  public void SetHealth(Health setHealth)
  {
    health = setHealth;
    health.OnHealthChanged += HandleHealthChanged;

    originalScale = transform.localScale;

    // Get size of game object
    Vector3 objectBounds = health.GetComponent<Collider>().bounds.size;
    // Get size average of x and z
    objectSize = ((objectBounds.x + objectBounds.z) / 2);

    heightOffset = objectBounds.y;
  }

  private void HandleHealthChanged(float pct)
  {
    healthBarImage.fillAmount = pct;
  }

  private void LateUpdate()
  {
    transform.position = playerCamera.WorldToScreenPoint(health.transform.position + Vector3.up * heightOffset);

    Vector3 objectScale = originalScale * playerCamera.GetComponent<CameraControls>().GetZoomPerc();
    objectScale.x *= objectSize;
    //objectScale.y *= objectSize * 0.5f;

    transform.localScale = objectScale;
  }

  private void OnDestroy()
  {
    health.OnHealthChanged -= HandleHealthChanged;
  }
}
