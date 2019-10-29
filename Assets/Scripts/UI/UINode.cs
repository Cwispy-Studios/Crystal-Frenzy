using UnityEngine;
using UnityEngine.UI;

public class UINode : MonoBehaviour
{
  [SerializeField]
  private Image nodeConnectingLinePrefab = null;
  private Image[] nodeConnectingLines;

  private Color fortressColour = new Color(225f / 255f, 60f / 255f, 125f / 255f);
  private Color unexploredColour = new Color(155f / 255f, 155f / 255f, 155f / 255f);
  private Color conquerableColour = new Color(155f / 255f, 155f / 255f, 155f / 255f);

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
    playerCamera.GetComponent<CameraManager>().PointCameraAtPosition(node.transform.position, 0.2f);
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

        nodeConnectingLines[i].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, lineAngle);
        nodeConnectingLines[i].GetComponent<RectTransform>().anchoredPosition = heading * buttonRadius + (Vector3.right * buttonRadius);
      }
    }
  }

  public void UpdateColour()
  {
    GetComponent<Button>().interactable = node.GetComponent<CrystalNode>().explored;

    if (!node.GetComponent<CrystalNode>().explored)
    {
      GetComponent<Image>().color = unexploredColour;
    }
  }
}
