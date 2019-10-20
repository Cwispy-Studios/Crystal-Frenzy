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
    CameraProperties.mouseOverUI = true;
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    CameraProperties.mouseOverUI = false;
  }

  #endregion
}