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
    UNFRIENDLY,
    NONE
  }

  private const float TARGET_CIRCLE_MODIFIER = 1.05f;

  [SerializeField]
  GameObject selectionCirclePrefab = null;
  [SerializeField]
  float scaleModifier = 1f;

  [System.NonSerialized]
  public SELECT_STATUS selectStatus = SELECT_STATUS.UNSELECTED;
  [System.NonSerialized]
  public SELECT_FACTION selectFaction = SELECT_FACTION.NONE;

  private GameObject selectionCircle;
  private GameObject targetCircle;
  private float selectionCircleRadius;

  private const float ALPHA_OPAQUE = 1f;
  private const float ALPHA_TRANSLUCENT = 0.4f;
  private const float ALPHA_TRANSPARENT = 0f;

  private int blinkLoops = 0;
  private const int MAX_LOOPS = 4;

  private Camera playerCamera;

  private void Awake()
  {
    playerCamera = Camera.main;
    selectionCircle = Instantiate(selectionCirclePrefab);
    selectionCircle.transform.SetParent(gameObject.transform, false);
    selectionCircle.name = "SelectionCircle";
    targetCircle = Instantiate(selectionCirclePrefab);
    targetCircle.transform.SetParent(gameObject.transform, false);
    targetCircle.name = "TargetCircle";

    // Get size of game object
    Vector3 parentSize = gameObject.GetComponent<Collider>().bounds.size;
    // Get radius average of x and z and then scale
    selectionCircleRadius = ((parentSize.x + parentSize.z) / 4) * scaleModifier;
    // Set the projector size
    selectionCircle.GetComponent<Projector>().orthographicSize = selectionCircleRadius;
    targetCircle.GetComponent<Projector>().orthographicSize = selectionCircleRadius * TARGET_CIRCLE_MODIFIER;
    // Set how far down the projector projects based on the y position of the object
    selectionCircle.GetComponent<Projector>().farClipPlane += parentSize.y;
    targetCircle.GetComponent<Projector>().farClipPlane += parentSize.y;

    // Assign new material instances to the selection and target circles and destroy the current instances
    Material selectionCircleMaterial = new Material(selectionCircle.GetComponent<Projector>().material);
    Material targetCircleMaterial = new Material(targetCircle.GetComponent<Projector>().material);

    selectionCircle.GetComponent<Projector>().material = selectionCircleMaterial;
    targetCircle.GetComponent<Projector>().material = targetCircleMaterial;

    // Default invisible
    ChangeTransparency(selectionCircle, ALPHA_TRANSPARENT);
    ChangeTransparency(targetCircle, ALPHA_TRANSPARENT);
  }

  private void OnEnable()
  {
    playerCamera.GetComponent<CameraObjectSelection>().AddSelectable(this);
  }

  private void OnDisable()
  {
    if (playerCamera)
      playerCamera.GetComponent<CameraObjectSelection>().RemoveSelectable(this);
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

  public float GetSelectionCircleSize()
  {
    return selectionCircleRadius;
  }

  private void ChangeTransparency(GameObject circle, float alpha)
  {
    //Material circleMaterial = new Material(circle.GetComponent<Projector>().material);

    //Color newColor = circleMaterial.color;
    //newColor.a = alpha;
    //circleMaterial.color = newColor;
    //circle.GetComponent<Projector>().material = circleMaterial;

    Color newColor = circle.GetComponent<Projector>().material.color;
    newColor.a = alpha;
    circle.GetComponent<Projector>().material.color = newColor;
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
    Color newColor = circle.GetComponent<Projector>().material.color;

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

    circle.GetComponent<Projector>().material.color = newColor;

    newColor = circle.GetComponent<Projector>().material.color;
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

      switch (targetCircle.GetComponent<Projector>().material.color.a.ToString())
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

  private void OnDestroy()
  {
    Destroy(selectionCircle.GetComponent<Projector>().material);
    Destroy(targetCircle.GetComponent<Projector>().material);
  }
}