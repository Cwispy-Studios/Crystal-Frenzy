using UnityEngine;

public class UISoundEmitter : MonoBehaviour
{
  [FMODUnity.EventRef]
  public string buttonClickSound = "", constructButtonSound = "", repairButtonSound = "", combatStartSound = "", beginPhaseSound = "";

  public void PlayButtonClick()
  {
    FMODUnity.RuntimeManager.PlayOneShot(buttonClickSound);
  }

  public void PlayConstructSound()
  {
    FMODUnity.RuntimeManager.PlayOneShot(constructButtonSound);
  }

  public void PlayRepairSound()
  {
    FMODUnity.RuntimeManager.PlayOneShot(repairButtonSound);
  }

  public void PlayCombatStartSound()
  {
    FMODUnity.RuntimeManager.PlayOneShot(combatStartSound);
  }

  public void PlayBeginPhaseSound()
  {
    FMODUnity.RuntimeManager.PlayOneShot(beginPhaseSound);
  }
}
