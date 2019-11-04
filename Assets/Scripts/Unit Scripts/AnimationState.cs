using UnityEngine;

public enum CURRENT_ANIMATION_STATE
{
  IDLE = 0,
  MOVE,
  ATTACK
}

public class AnimationState : MonoBehaviour
{
  [HideInInspector]
  public CURRENT_ANIMATION_STATE currentAnimationState;
}
