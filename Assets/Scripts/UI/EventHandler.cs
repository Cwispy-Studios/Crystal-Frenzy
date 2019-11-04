using UnityEngine;
using UnityEngine.EventSystems;

public class EventHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
  private GameObject playerCamera = null;

  private void Awake()
  {
    playerCamera = Camera.main.gameObject;
  }

  #region IPointerEnterHandler implementation

  public void OnPointerEnter(PointerEventData eventData)
  {
    playerCamera.GetComponent<CameraManager>().mouseOverUI = true;
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    playerCamera.GetComponent<CameraManager>().mouseOverUI = false;
  }

  #endregion
}