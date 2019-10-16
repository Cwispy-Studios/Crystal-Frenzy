using UnityEngine;
using System.Collections;

public class Selectable : MonoBehaviour
{
  public enum SELECT_STATUS
  {
    UNSELECTED = 0,
    HOVER,
    SELECTED
  }

  public enum SELECT_FACTION
  {
    NEUTRAL = 0,
    FRIENDLY,
    UNFRIENDLY
  }

  private const float TARGET_CIRCLE_MODIFIER = 1.05f;

  [SerializeField]
  GameObject selectionCirclePrefab = null;
  [SerializeField]
  float scaleModifier = 1f;

  [System.NonSerialized]
  public SELECT_STATUS selectStatus = SELECT_STATUS.UNSELECTED;
  [System.NonSerialized]
  public SELECT_FACTION selectFaction = SELECT_FACTION.NEUTRAL;

  private GameObject selectionCircle;
  private GameObject targetCircle;

  private const float ALPHA_OPAQUE = 1f;
  private const float ALPHA_TRANSLUCENT = 0.3f;
  private const float ALPHA_TRANSPARENT = 0f;

  private int blinkLoops = 0;
  private const int MAX_LOOPS = 4;

  private void Awake()
  {
    selectionCircle = Instantiate(selectionCirclePrefab);
    selectionCircle.transform.SetParent(transform, false);
    selectionCircle.name = "SelectionCircle";
    targetCircle = Instantiate(selectionCirclePrefab);
    targetCircle.transform.SetParent(transform, false);
    targetCircle.name = "TargetCircle";

    // Get size of game object
    Vector3 parentSize = gameObject.GetComponent<Collider>().bounds.size;

    // Get the largest of the 2d coordinates
    float largestAxis = Mathf.Max(parentSize.x, parentSize.z);

    Vector3 circleScale = new Vector3(largestAxis, largestAxis);

    // Set the size of the circles equal to the bounds size and then scale with modifier
    selectionCircle.transform.localScale = circleScale * scaleModifier;
    targetCircle.transform.localScale = circleScale * scaleModifier * TARGET_CIRCLE_MODIFIER;

    // Circles have to hover very slightly off the ground
    Vector3 circlePos = transform.position;
    circlePos.y = 0.01f;
    selectionCircle.transform.localPosition = circlePos;
    targetCircle.transform.localPosition = circlePos;

    // Default invisible
    ChangeTransparency(selectionCircle, ALPHA_TRANSPARENT);
    ChangeTransparency(targetCircle, ALPHA_TRANSPARENT);
  }

  private void Update()
  {
    switch (selectStatus)
    {
      case SELECT_STATUS.UNSELECTED: ChangeTransparency(selectionCircle, ALPHA_TRANSPARENT);
        break;

      case SELECT_STATUS.HOVER: ChangeTransparency(selectionCircle, ALPHA_TRANSLUCENT);
        break;

      case SELECT_STATUS.SELECTED: ChangeTransparency(selectionCircle, ALPHA_OPAQUE);
        break;
    }

    if (blinkLoops >= MAX_LOOPS)
    {
      StopBlinking();
    }
  }

  private void ChangeTransparency(GameObject circle, float alpha)
  {
    MaterialPropertyBlock block = new MaterialPropertyBlock();
    circle.GetComponent<Renderer>().GetPropertyBlock(block);

    Color newColor = block.GetColor("_BaseColor");

    newColor.a = alpha;

    block.SetColor("_BaseColor", newColor);

    circle.GetComponent<Renderer>().SetPropertyBlock(block);
  }

  public void CheckFactionColour(Faction.FACTIONS playerFaction)
  {
    // Get faction of this unit
    Faction.FACTIONS faction = gameObject.GetComponent<Faction>().faction;

    // Neutral units should always be yellow, check if this is the case
    if (faction == Faction.FACTIONS.NEUTRAL)
    {
      // Colour is wrong, change the colour
      if (selectFaction != SELECT_FACTION.NEUTRAL)
      {
        ChangeColour(selectionCircle, SELECT_FACTION.NEUTRAL);
        ChangeColour(targetCircle, SELECT_FACTION.NEUTRAL);
      }
    }

    // The unit is friendly and belongs to the player
    else if (faction == playerFaction)
    {
      // Colour is wrong, change the colour
      if (selectFaction != SELECT_FACTION.FRIENDLY)
      {
        ChangeColour(selectionCircle, SELECT_FACTION.FRIENDLY);
        ChangeColour(targetCircle, SELECT_FACTION.FRIENDLY);
      }
    }

    // The unit is unfriendly and belongs to the enemy
    else if (faction != playerFaction)
    {
      // Colour is wrong, change the colour
      if (selectFaction != SELECT_FACTION.UNFRIENDLY)
      {
        ChangeColour(selectionCircle, SELECT_FACTION.UNFRIENDLY);
        ChangeColour(targetCircle, SELECT_FACTION.UNFRIENDLY);
      }
    }
  }

  private void ChangeColour(GameObject circle, SELECT_FACTION newFactionColour)
  {
    MaterialPropertyBlock block = new MaterialPropertyBlock();
    circle.GetComponent<Renderer>().GetPropertyBlock(block);

    Color newColor = block.GetColor("_BaseColor");

    switch (newFactionColour)
    {
      case SELECT_FACTION.NEUTRAL:
        newColor.r = 255f / 255f;
        newColor.g = 255f / 255f;
        newColor.b = 0;
        selectFaction = SELECT_FACTION.NEUTRAL;
        break;

      case SELECT_FACTION.FRIENDLY:
        newColor.r = 0;
        newColor.g = 255f / 255f;
        newColor.b = 0;
        selectFaction = SELECT_FACTION.FRIENDLY;
        break;

      case SELECT_FACTION.UNFRIENDLY:
        newColor.r = 255f / 255f;
        newColor.g = 0;
        newColor.b = 0;
        selectFaction = SELECT_FACTION.UNFRIENDLY;
        break;
    }

    block.SetColor("_BaseColor", newColor);

    circle.GetComponent<Renderer>().SetPropertyBlock(block);
  }

  

  public void StartBlinking()
  {
    blinkLoops = 0;
    StopBlinking();
    StartCoroutine("TargetBlink");
  }

  private void StopBlinking()
  {
    blinkLoops = 0;
    ChangeTransparency(targetCircle, 0);
    StopAllCoroutines();
  }

  IEnumerator TargetBlink()
  {
    while (true)
    {
      ++blinkLoops;
      MaterialPropertyBlock block = new MaterialPropertyBlock();
      targetCircle.GetComponent<Renderer>().GetPropertyBlock(block);

      switch (block.GetColor("_BaseColor").a.ToString())
      {
        case "0":
          ChangeTransparency(targetCircle, 1);
          
          yield return new WaitForSeconds(0.25f);
          break;
        case "1":
          ChangeTransparency(targetCircle, 0);

          yield return new WaitForSeconds(0.25f);
          break;
      }
    }
  }
}