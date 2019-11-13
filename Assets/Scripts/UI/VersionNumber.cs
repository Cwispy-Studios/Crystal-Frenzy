using UnityEngine;
using UnityEngine.UI;

public class VersionNumber : MonoBehaviour
{
  private void Awake()
  {
    GetComponent<Text>().text = "Version " + Application.version.ToString();
  }
}
