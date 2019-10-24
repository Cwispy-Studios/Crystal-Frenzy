using UnityEngine;

public class MoveToGroundCircle : MonoBehaviour
{
  private const float fadeSpeed = 1.8f, shrinkSpeed = 2f;

  private void Update()
  {
    Color circleColour = GetComponent<Renderer>().material.color;
    Vector3 circleScale = transform.localScale;

    if (circleColour.a > 0 && circleScale.x > 0)
    {
      circleColour.a -= fadeSpeed * Time.deltaTime;
      circleScale.x -= shrinkSpeed * Time.deltaTime;
      circleScale.y -= shrinkSpeed * Time.deltaTime;

      GetComponent<Renderer>().material.color = circleColour;
      transform.localScale = circleScale;
    }

    else
    {
      Destroy(gameObject);
    }
  }
}
