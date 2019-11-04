using UnityEngine;

public class UISoundEmitter : MonoBehaviour
{
  [FMODUnity.EventRef]
  public string buttonClickSound = "", constructButtonSound = "", repairButtonSound = "";

  public void PlayButtonClick()
  {
    FMODUnity.RuntimeManager.PlayOneShotAttached(buttonClickSound, gameObject);
  }

  public void PlayConstructSound()
  {
    FMODUnity.RuntimeManager.PlayOneShotAttached(constructButtonSound, gameObject);
  }

  public void PlayRepairSound()
  {
    FMODUnity.RuntimeManager.PlayOneShotAttached(repairButtonSound, gameObject);
  }
}
