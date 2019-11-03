using UnityEngine;
using UnityEngine.UI;

public class UINode : MonoBehaviour
{
  [SerializeField]
  private Image nodeConnectingLinePrefab = null;
  private Image[] nodeConnectingLines;

  // Node colour for your fortress
  private Color fortressColour = new Color(185f / 255f, 60f / 255f, 125f / 255f);
  // Node colour of the life crystal
  private Color lifeCrystalColour = new Color(90f / 255f, 60f / 255f, 125f / 255f);
  // Node colour if node is unexplored
  private Color unexploredColour = new Color(155f / 255f, 155f / 255f, 155f / 255f);
  // Node colour if node is the current active one
  private Color activeColour = new Color(0 / 255f, 200f / 255f, 0 / 255f);
  // Node colour if you can select node to targset
  private Color conquerableColour = new Color(255f / 255f, 170f / 255f, 50f / 255f);
  // Node colour if node is selected to be attacked
  private Color targetedColour = new Color(155f / 255f, 0 / 255f, 0 / 255f);
  // Node colour if node is explored but can no longer be chosen
  private Color blockedColour = new Color(125f / 255f, 125f / 255f, 25f / 255f);

  private Camera playerCamera;
  private GameObject node = null;

  private float buttonRadius;

  private void Awake()
  {
    playerCamera = Camera.main;

    GetComponent<Button>().onClick.AddListener(ZoomToNode);

    buttonRadius = GetComponent<RectTransform>().sizeDelta.x / 2f;
  }

  private void ZoomToNode()
  {
    playerCamera.GetComponent<CameraManager>().PointCameraAtPosition(node.transform.position, playerCamera.GetComponent<CameraControls>().birdsEyeViewMode, true, 0, 0.2f);
  }

  public void SetNode(GameObject setNode, float worldScale, float panelScale)
  {
    if (node == null)
    {
      node = setNode;

      if (node.GetComponent<CrystalNode>().IsFortress)
      {
        GetComponent<Image>().color = fortressColour;
      }

      nodeConnectingLines = new Image[node.GetComponent<CrystalNode>().ConnectedNodesData.Length];

      for (int i = 0; i < nodeConnectingLines.Length; ++i)
      {
        nodeConnectingLines[i] = Instantiate(nodeConnectingLinePrefab, transform, false);

        Vector3 worldVectorBetweenNodes = node.GetComponent<CrystalNode>().ConnectedNodesData[i].connectedNode.transform.position - node.transform.position;

        // Convert to panel space
        Vector3 panelVectorBetweenNodes = (worldVectorBetweenNodes / worldScale) * panelScale;

        float panelDistance = panelVectorBetweenNodes.magnitude;

        Vector3 heading = panelVectorBetweenNodes / panelDistance;
        heading.y = heading.z;
        heading.z = 0;

        float connectingLineWidth = panelDistance - (buttonRadius * 2f);

        Vector3 connectingLineSize = new Vector3(connectingLineWidth, nodeConnectingLines[i].GetComponent<RectTransform>().sizeDelta.y);

        nodeConnectingLines[i].GetComponent<RectTransform>().sizeDelta = connectingLineSize;

        float lineAngle = Vector3.Angle(Vector3.right, heading);

        if (heading.y < 0)
        {
          lineAngle = -lineAngle;
        }

        nodeConnectingLines[i].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, lineAngle);
        nodeConnectingLines[i].GetComponent<RectTransform>().anchoredPosition = heading * buttonRadius + (Vector3.right * buttonRadius);
      }
    }
  }

  public void UpdateColour()
  {
    GetComponent<Button>().interactable = node.GetComponent<CrystalNode>().explored;

    // Fortress node never changes colour
    if (node.GetComponent<CrystalNode>().IsFortress)
    {
      GetComponent<Image>().color = fortressColour;
    }
    // Life Crystal node never changes colour
    else if (node.GetComponent<CrystalNode>().IsLifeCrystal)
    {
      GetComponent<Image>().color = lifeCrystalColour;
    }

    // Unexplored nodes are nodes not connected to any nodes we have conquered
    else if (!node.GetComponent<CrystalNode>().explored)
    {
      GetComponent<Image>().color = unexploredColour;
    }

    // Active node is the node you are currently playing on
    else if (node.GetComponent<CrystalNode>().active)
    {
      GetComponent<Image>().color = activeColour;
    }

    // Targetted node is the node we are attacking / defending from
    else if (node.GetComponent<CrystalNode>().targeted)
    {
      GetComponent<Image>().color = targetedColour;
    }

    // Explored and conquerable means the node can be targetted
    else if (node.GetComponent<CrystalNode>().explored && node.GetComponent<CrystalNode>().conquerable)
    {
      GetComponent<Image>().color = conquerableColour;
    }

    // Explored and not conquerable means the node is blocked off because we selected another node
    else if (node.GetComponent<CrystalNode>().explored && !node.GetComponent<CrystalNode>().conquerable)
    {
      GetComponent<Image>().color = blockedColour;
    }
  }
}
