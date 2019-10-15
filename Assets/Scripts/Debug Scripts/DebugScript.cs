using UnityEngine;
using UnityEngine.UI;

public class DebugScript : MonoBehaviour
{
  public static Text debugText;

  private void Awake()
  {
    debugText = gameObject.GetComponent<Text>();
    debugText.text = "";
  }

  private void Update()
  {
    debugText.text = Input.mousePosition.x.ToString() + ", " + Input.mousePosition.y.ToString() + ", " + Input.mousePosition.z.ToString();
  }
}
