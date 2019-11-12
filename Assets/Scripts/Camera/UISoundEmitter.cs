using UnityEngine;

public class UISoundEmitter : MonoBehaviour
{
  [FMODUnity.EventRef]
  public string buttonClickSound = "", constructButtonSound = "", repairButtonSound = "", combatStartEscortSound = "", combatStartDefenseSound = "", beginPhaseSound = "";

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

  public void PlayCombatStartEscortSound()
  {
    FMODUnity.RuntimeManager.PlayOneShot(combatStartEscortSound);
  }

  public void PlayCombatStartDefenseSound()
  {
    FMODUnity.RuntimeManager.PlayOneShot(combatStartDefenseSound);
  }

  public void PlayBeginPhaseSound()
  {
    FMODUnity.RuntimeManager.PlayOneShot(beginPhaseSound);
  }
}
