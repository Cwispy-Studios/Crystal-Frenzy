using UnityEngine;
using UnityEngine.UI;

public class UINode : MonoBehaviour
{
  [SerializeField]
  private GameObject node = null;

  private void Start()
  {
    //GetComponent<Button>().onClick.AddListener(Camera.main.GetComponent<CameraManager>().PanToNode);
  }
}
