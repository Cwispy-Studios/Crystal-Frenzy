using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
  [Header("Text")]
  [SerializeField]
  private Text phaseText = null, nodeText = null;

  [Header("")]

  private ResourceManager resourceManager;

  private int currentPhaseCycle = 0;
  private PHASES currentPhase = PHASES.PREPARATION;

  private void Start()
  {
    resourceManager = GetComponent<ResourceManager>();
  }

  private void FixedUpdate()
  {
    switch (currentPhase)
    {
      case PHASES.PREPARATION: phaseText.text = PHASES.PREPARATION.ToString() + " PHASE";
        break;

      case PHASES.ESCORT:
        break;

      case PHASES.FORTIFICATION:
        break;

      case PHASES.DEFENSE:
        break;
    }
  }
}
