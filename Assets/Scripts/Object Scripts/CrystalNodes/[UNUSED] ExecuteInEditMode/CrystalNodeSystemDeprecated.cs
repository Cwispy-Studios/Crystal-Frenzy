using UnityEngine;
using BezierSolution;

[ExecuteInEditMode]
public class CrystalNodeSystemDeprecated : MonoBehaviour
{
  private const int DEFAULT_NUM_POINTS = 5;
  [SerializeField]
  private bool initialise = false, reset = false;

  [SerializeField]
  private GameObject[] connectedNodes;
  [SerializeField]
  private GameObject[] crystalNodesSplines;

  public ConnectedNodesScriptableObject[] connectedNodesScriptableObject;
  public CrystalNodesSplinesScriptableObject[] crystalNodesSplinesScriptableObject;

  [SerializeField]
  private GameObject CrystalPathIndicatorPrefab = null;
  private const float SPLINE_Y_AXIS_OFFSET = 0.3f;

  private bool initialised = false;
  private int connectedNodesSize = 0;

  private GameObject[] cachedConnectedNodes = null;

  private void Awake()
  {
    SerialiseData();

    UpdateCache();

    reset = false;
    initialise = false;
  }

  private void SerialiseData()
  {
    if (connectedNodesScriptableObject != null && connectedNodesScriptableObject.Length > 0)
    {
      for (int i = 0; i < connectedNodesScriptableObject.Length; ++i)
      {
        if (connectedNodesScriptableObject[i] != null)
        {
          connectedNodes[i] = connectedNodesScriptableObject[i].connectedNode;
        }

        else
        {
          connectedNodes[i] = null;
        }
      }
    }

    if (crystalNodesSplinesScriptableObject != null && crystalNodesSplinesScriptableObject.Length > 0)
    {
      for (int i = 0; i < crystalNodesSplinesScriptableObject.Length; ++i)
      {
        if (crystalNodesSplinesScriptableObject[i] != null)
        {
          crystalNodesSplines[i] = crystalNodesSplinesScriptableObject[i].crystalNodeSpline;
        }

        else
        {
          crystalNodesSplines[i] = null;
        }
      }
    }
  }

  private void UpdateCache()
  {
    cachedConnectedNodes = new GameObject[connectedNodes.Length];
    connectedNodes.CopyTo(cachedConnectedNodes, 0);
  }

  private void Update()
  {
    if (!Application.isPlaying)
    {
      if (connectedNodes != null)
        connectedNodesSize = connectedNodes.Length;

      if (reset)
      {
        ResetAll();
      }

      // We only do any sort of updating of the nodes if it already exists
      if (connectedNodes != null)
      {
        // We check if the Splines do not already exist, if they do not, this means we are assigning them for the first time
        // All the nodes also have to be already assigned to
        if (initialise && crystalNodesSplines != null && crystalNodesSplines.Length == 0 && AllNodesAreAssigned())
        {
          InitialiseSplines();
        }

        // We check every frame if we have changed anything in connectedNodes in the Editor.
        else if (initialised)
        {
          UpdateNodesReferences();
        }
      }

      SaveSerialiseData();

      initialise = false;
    }
  }

  private void InitialiseSplines()
  {
    crystalNodesSplines = new GameObject[connectedNodes.Length];

    // Create all the splines
    for (int i = 0; i < connectedNodes.Length; ++i)
    {
      InitialiseSpline(i);

      connectedNodes[i].GetComponent<CrystalNodeSystemDeprecated>().AddConnectedNodeReference(gameObject, crystalNodesSplines[i]);
    } // for end

    initialised = true;
    UpdateCache();
  }

  private void InitialiseSpline(int index)
  {
    // Create the spline and set it as a child of this object
    crystalNodesSplines[index] = new GameObject();
    crystalNodesSplines[index].transform.parent = transform;
    crystalNodesSplines[index].transform.localPosition = new Vector3();

    BezierSpline spline = crystalNodesSplines[index].AddComponent<BezierSpline>();
    spline.Initialize(DEFAULT_NUM_POINTS);

    // Set the name of the spline according to which objects it is connecting with
    crystalNodesSplines[index].name = name + "To" + connectedNodes[index].name + "Spline";

    // Set the positions of the bezier splines
    Vector3 startPos = transform.position;
    startPos.y = SPLINE_Y_AXIS_OFFSET;
    Vector3 endPos = connectedNodes[index].transform.position;
    endPos.y = SPLINE_Y_AXIS_OFFSET;

    for (int splinePoint = 0; splinePoint < DEFAULT_NUM_POINTS; ++splinePoint)
    {
      spline[splinePoint].position = Vector3.Lerp(startPos, endPos, splinePoint / (DEFAULT_NUM_POINTS - 1f));
    }

    // Set up the particle system for indicating the path
    GameObject crystalPathIndicator = Instantiate(CrystalPathIndicatorPrefab, crystalNodesSplines[index].transform, false);
    crystalPathIndicator.name = name + "To" + connectedNodes[index].name + "CrystalPathIndicator";

    Vector3 indicatorPos = crystalPathIndicator.transform.position;
    indicatorPos.y = 0.4f;
    crystalPathIndicator.transform.position = indicatorPos;

    ParticleSystem.EmissionModule emittor = crystalPathIndicator.GetComponent<ParticleSystem>().emission;
    crystalPathIndicator.GetComponent<ParticlesFollowBezier>().spline = spline;
    emittor.enabled = false;
  }

  // Here we check if anything with the connectedNodes assignment has changed
  // The length could be changed or another object could have been assigned
  private void UpdateNodesReferences()
  {
    //Debug.Log(connectedNodes.Length + name);
    // First we check if connectedNodes has been removed, that is, the length has been set to 0
    if (connectedNodes.Length == 0)
    {
      //Debug.Log("Correct");
      // No more nodes connected, so we have to go through the cache and removed all the Splines and connectedNodes references
      ResetSplines();

      for (int i = 0; i < cachedConnectedNodes.Length; ++i)
      {
        cachedConnectedNodes[i].GetComponent<CrystalNodeSystemDeprecated>().RemoveConnectedNodeReference(gameObject);
      }

      // Update the cache to be empty as well
      cachedConnectedNodes = null;

      initialised = false;
    } // connectedNodes empty

    // The connectedNodes has objects, check if the length is different from the cache
    else if (connectedNodes.Length != cachedConnectedNodes.Length)
    {
      // This means the length of connectedNodes has changed, and we need to reinitialise the values of crystalNodesSplines
      // For this, we need to store the old values somewhere 
      GameObject[] tempSets = new GameObject[crystalNodesSplines.Length];

      // If the set has increased in size, we just move the existing sets into the new one
      // If the set has decreased in size, we simply leave out the excess nodes in the scene and move over what we can
      for (int i = 0; i < crystalNodesSplines.Length; ++i)
      {
        tempSets[i] = crystalNodesSplines[i];
      }

      // Initialise new Splines
      // This leaves the existing crystalNodes in the scene in case we still need them.
      crystalNodesSplines = new GameObject[connectedNodes.Length];

      int iteration = connectedNodes.Length < cachedConnectedNodes.Length ? connectedNodes.Length : cachedConnectedNodes.Length;

      for (int i = 0; i < iteration; ++i)
      {
        crystalNodesSplines[i] = tempSets[i];
      }

      // Now we check if the size has decreased
      // If it decreased, we need to remove references from the connectedNodes
      if (connectedNodes.Length < cachedConnectedNodes.Length)
      {
        for (int i = connectedNodes.Length; i < cachedConnectedNodes.Length; ++i)
        {
          if (cachedConnectedNodes[i] != null)
          {
            cachedConnectedNodes[i].GetComponent<CrystalNodeSystemDeprecated>().RemoveConnectedNodeReference(gameObject);
          }
        }
      }

      // If it increased, we only need to update the cache since the references do not get deleted
      UpdateCache();
    }

    // The length has not changed but the references may have changed, we need to check against the cache for that
    else if (connectedNodes.Length == cachedConnectedNodes.Length)
    {
      for (int i = 0; i < connectedNodes.Length; ++i)
      {
        if (connectedNodes[i] != cachedConnectedNodes[i])
        {
          // Before doing anything, we need to check if the new reference already exists in this node
          // In case we added the same object more than once in the Inspector
          bool nodeAlreadyExists = false;

          for (int checkAgainstIndex = 0; checkAgainstIndex < connectedNodes.Length; ++checkAgainstIndex)
          {
            if (i != checkAgainstIndex && connectedNodes[i] == connectedNodes[checkAgainstIndex])
            {
              nodeAlreadyExists = true;
              break;
            }
          }

          // If there is a duplicate object, we revert back to the cache
          if (nodeAlreadyExists)
          {
            cachedConnectedNodes.CopyTo(connectedNodes, 0);
          }

          // If any references changed, we need to remove the references of the old connected nodes, set a new reference to the newly assigned
          // node, and create a new Spline for this new connection. Again, we leave the old spline in the scene in case we still need it
          else
          {
            // Add new Spline to this object
            InitialiseSpline(i);
            // Remove references
            cachedConnectedNodes[i].GetComponent<CrystalNodeSystemDeprecated>().RemoveConnectedNodeReference(gameObject);
            // Set new reference
            connectedNodes[i].GetComponent<CrystalNodeSystemDeprecated>().AddConnectedNodeReference(gameObject, crystalNodesSplines[i]);
          }
        }
      }
    }
  }

  public void RemoveConnectedNodeReference(GameObject nodeToRemove)
  {
    GameObject[] tempNodes = new GameObject[connectedNodes.Length - 1];
    GameObject[] tempSplines = new GameObject[connectedNodes.Length - 1];

    int removedIndex = 9999;

    // Find the object reference to be removed in our connectedNodes
    for (int i = 0; i < connectedNodes.Length; ++i)
    {
      if (connectedNodes[i] == nodeToRemove)
      {
        connectedNodes[i] = null;
        crystalNodesSplines[i] = null;

        removedIndex = i;
      }

      else
      {
        if (i < removedIndex)
        {
          tempNodes[i] = connectedNodes[i];
          tempSplines[i] = crystalNodesSplines[i];
        }

        else if (i > removedIndex)
        {
          tempNodes[i - 1] = connectedNodes[i];
          tempSplines[i - 1] = crystalNodesSplines[i];
        }
      }
    }

    connectedNodes = new GameObject[tempNodes.Length];
    crystalNodesSplines = new GameObject[tempSplines.Length];

    tempNodes.CopyTo(connectedNodes, 0);
    tempSplines.CopyTo(crystalNodesSplines, 0);

    UpdateCache();
  }

  public void AddConnectedNodeReference(GameObject nodeToAdd, GameObject splineToSync)
  {
    int indexToSync = 0;
    // In case we need to extend the array to accomadate this new node, we should store all the old values in a temp array.

    GameObject[] tempNodes = null;
    GameObject[] tempSplines = null;
    bool arrayExists = false;

    // Check if the objects have been initialised yet
    if (connectedNodes != null && connectedNodes.Length > 0)
    {
      tempNodes = new GameObject[connectedNodes.Length];
      tempSplines = new GameObject[connectedNodes.Length];
      arrayExists = true;
    }

    else
    {
      arrayExists = false;
    }

    bool increaseArraySize = true;

    // We can simply create a new object without worrying about moving anything
    if (!arrayExists)
    {
      connectedNodes = new GameObject[1];
      crystalNodesSplines = new GameObject[1];
      initialised = true;
    }

    else
    {
      // First check for any null nodes that we can use to sync up 
      for (int i = 0; i < connectedNodes.Length; ++i)
      {
        // If null found, save the index
        if (connectedNodes[i] == null)
        {
          indexToSync = i;
          increaseArraySize = false;

          break;
        }

        // Otherwise populate the temp array
        else
        {
          tempNodes[i] = connectedNodes[i];
          tempSplines[i] = crystalNodesSplines[i];
        }
      }

      // No nulls found, so we need to increase the array size by 1 and set the last node as the index to add to
      if (increaseArraySize)
      {
        connectedNodes = new GameObject[tempNodes.Length + 1];
        crystalNodesSplines = new GameObject[tempNodes.Length + 1];

        // Repopulate array from temp array
        for (int i = 0; i < tempNodes.Length; ++i)
        {
          connectedNodes[i] = tempNodes[i];
          crystalNodesSplines[i] = tempSplines[i];
        }

        indexToSync = connectedNodes.Length - 1;
      }
    }
    connectedNodes[indexToSync] = nodeToAdd;

    // Now we sync up the bezierSpline as well
    crystalNodesSplines[indexToSync] = splineToSync;

    UpdateCache();
  }

  private void ResetSplines()
  {
    if (crystalNodesSplines != null)
    {
      for (int i = 0; i < crystalNodesSplines.Length; ++i)
      {
        if (crystalNodesSplines[i] != null)
        {
          crystalNodesSplines[i].name += "Old";
        }
      }

      crystalNodesSplines = null;
    }

    //reset = false;
  }

  private bool AllNodesAreAssigned()
  {
    if (connectedNodes == null || connectedNodes.Length == 0)
    {
      return false;
    }

    for (int i = 0; i < connectedNodes.Length; ++i)
    {
      if (connectedNodes[i] == null)
      {
        return false;
      }
    }

    return true;
  }

  private void ResetAll()
  {
    // Removes the references of all the connected nodes
    for (int i = 0; i < connectedNodes.Length; ++i)
    {
      if (connectedNodes[i] != null)
      {
        connectedNodes[i].GetComponent<CrystalNodeSystemDeprecated>().RemoveConnectedNodeReference(gameObject);
      }
    }

    connectedNodes = null;
    cachedConnectedNodes = null;
    crystalNodesSplines = null;

    reset = false;
    initialised = false;
  }

  private void OnValidate()
  {
    // Unity auto fills in new values, we don't want that so we have to remove all the new values they added
    if (connectedNodesSize < connectedNodes.Length)
    {
      for (int i = connectedNodesSize; i < connectedNodes.Length; ++i)
      {
        connectedNodes[i] = null;
      }
    }

    //if (connectedNodes != null && connectedNodes.Length > 0)
    //{
    //  connectedNodesScriptableObject = new ConnectedNodesScriptableObject[connectedNodes.Length];

    //  for (int i = 0; i < connectedNodes.Length; ++i)
    //  {
    //    if (connectedNodes[i] != null)
    //    {
    //      connectedNodesScriptableObject[i] = new ConnectedNodesScriptableObject();
    //      connectedNodesScriptableObject[i].connectedNode = connectedNodes[i];
    //    }

    //    else
    //    {
    //      connectedNodesScriptableObject[i] = null;
    //    }
    //  }
    //}

    //if (crystalNodesSplines != null && crystalNodesSplines.Length > 0)
    //{
    //  crystalNodesSplinesScriptableObject = new CrystalNodesSplinesScriptableObject[crystalNodesSplines.Length];

    //  for (int i = 0; i < crystalNodesSplines.Length; ++i)
    //  {
    //    if (crystalNodesSplines[i] != null)
    //    {
    //      crystalNodesSplinesScriptableObject[i] = new CrystalNodesSplinesScriptableObject();
    //      crystalNodesSplinesScriptableObject[i].crystalNodeSpline = crystalNodesSplines[i];
    //    }

    //    else
    //    {
    //      crystalNodesSplinesScriptableObject[i] = null;
    //    }
    //  }
    //}
  }

  public bool CheckCrystalIsValid(GameObject checkObject, GameObject crystalPath)
  {
    for (int i = 0; i < connectedNodes.Length; ++i)
    {
      if (connectedNodes[i] == checkObject && checkObject.GetComponent<Faction>().faction != GetComponent<Faction>().faction)
      {
        crystalPath = crystalNodesSplines[i];
        return true;
      }
    }

    return false;
  }

  private void SaveSerialiseData()
  {
    connectedNodesScriptableObject = null;
    crystalNodesSplinesScriptableObject = null;

    if (connectedNodes != null && connectedNodes.Length > 0)
    {
      connectedNodesScriptableObject = new ConnectedNodesScriptableObject[connectedNodes.Length];

      for (int i = 0; i < connectedNodes.Length; ++i)
      {
        if (connectedNodes[i] != null)
        {
          connectedNodesScriptableObject[i] = ScriptableObject.CreateInstance<ConnectedNodesScriptableObject>();
          connectedNodesScriptableObject[i].connectedNode = connectedNodes[i];
        }

        else
        {
          connectedNodesScriptableObject[i] = null;
        }
      }
    }

    if (crystalNodesSplines != null && crystalNodesSplines.Length > 0)
    {
      crystalNodesSplinesScriptableObject = new CrystalNodesSplinesScriptableObject[crystalNodesSplines.Length];

      for (int i = 0; i < crystalNodesSplines.Length; ++i)
      {
        if (crystalNodesSplines[i] != null)
        {
          crystalNodesSplinesScriptableObject[i] = ScriptableObject.CreateInstance<CrystalNodesSplinesScriptableObject>();
          crystalNodesSplinesScriptableObject[i].crystalNodeSpline = crystalNodesSplines[i];
        }

        else
        {
          crystalNodesSplinesScriptableObject[i] = null;
        }
      }

    }
  }

  //public void OnBeforeSerialize()
  //{
  //connectedNodesScriptableObject = null;
  //crystalNodesSplinesScriptableObject = null;

  //if (connectedNodes != null && connectedNodes.Length > 0)
  //{
  //  connectedNodesScriptableObject = new ConnectedNodesScriptableObject[connectedNodes.Length];

  //  for (int i = 0; i < connectedNodes.Length; ++i)
  //  {
  //    if (connectedNodes[i] != null)
  //    {
  //      Debug.Log(connectedNodes[i].name);
  //      connectedNodesScriptableObject[i] = ScriptableObject.CreateInstance<ConnectedNodesScriptableObject>();
  //      connectedNodesScriptableObject[i].connectedNode = connectedNodes[i];
  //    }

  //    else
  //    {
  //      connectedNodesScriptableObject[i] = null;
  //    }
  //  }
  //}

  //if (crystalNodesSplines != null && crystalNodesSplines.Length > 0)
  //{
  //  crystalNodesSplinesScriptableObject = new CrystalNodesSplinesScriptableObject[crystalNodesSplines.Length];

  //  for (int i = 0; i < crystalNodesSplines.Length; ++i)
  //  {
  //    if (crystalNodesSplines[i] != null)
  //    {
  //      crystalNodesSplinesScriptableObject[i] = ScriptableObject.CreateInstance<CrystalNodesSplinesScriptableObject>();
  //      crystalNodesSplinesScriptableObject[i].crystalNodeSpline = crystalNodesSplines[i];
  //    }
  //    crystalNodesSplinesScriptableObject[i] = null;
  //  }
  //}
  //}

  //public void OnAfterDeserialize()
  //{
  //if (connectedNodesScriptableObject.Length > 0)
  //{
  //  for (int i = 0; i < connectedNodesScriptableObject.Length; ++i)
  //  {
  //    if (connectedNodesScriptableObject[i] != null)
  //    {
  //      if (connectedNodesScriptableObject[i].connectedNode != null)
  //      {
  //        Debug.Log(i + " object is found");
  //      }

  //      else
  //      {
  //        Debug.Log(i + " object is null");
  //      }
  //    }

  //    else
  //    {
  //      Debug.Log(i + " is null");
  //    }
  //  }
  //}

  //else Debug.Log("Length is 0");
  //if (connectedNodesScriptableObject != null && connectedNodesScriptableObject.Length > 0)
  //{
  //  connectedNodes = new GameObject[connectedNodesScriptableObject.Length];

  //  for (int i = 0; i < connectedNodesScriptableObject.Length; ++i)
  //  {
  //    if (connectedNodesScriptableObject[i] != null && connectedNodesScriptableObject[i].connectedNode != null)
  //    {
  //      connectedNodes[i] = connectedNodesScriptableObject[i].connectedNode;
  //    }

  //    else
  //    {
  //      connectedNodes[i] = null;
  //    }
  //  }
  //}

  //if (crystalNodesSplinesScriptableObject != null && crystalNodesSplinesScriptableObject.Length > 0)
  //{
  //  crystalNodesSplines = new BezierSpline[crystalNodesSplinesScriptableObject.Length];

  //  for (int i = 0; i < crystalNodesSplinesScriptableObject.Length; ++i)
  //  {
  //    if (crystalNodesSplinesScriptableObject[i] != null && crystalNodesSplinesScriptableObject[i].crystalNodeSpline != null)
  //    {
  //      crystalNodesSplines[i] = crystalNodesSplinesScriptableObject[i].crystalNodeSpline;
  //    }

  //    else
  //    {
  //      crystalNodesSplines[i] = null;
  //    }
  //  }
  //}
  //}
}