using TMPro;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }
    public GameObject tooltipPrefab; // ���� ������
    private GameObject currentTooltip; // ���� Ȱ��ȭ�� ����
    private TextMeshProUGUI tooltipText; // ������ �ؽ�Ʈ ������Ʈ

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void ShowTooltip(string content, Vector3 position)
    {
        Debug.Log("Showing tooltip with content: " + content); // ���� ���� �α� Ȯ��
        // ���� ������ ������ ����
        if (currentTooltip != null)
        {
            Destroy(currentTooltip);
        }

        // ���ο� ���� ����
        currentTooltip = Instantiate(tooltipPrefab, position, Quaternion.identity);

        Debug.Log("Current Tooltip: " + currentTooltip); // ������ �����Ǿ����� Ȯ��

        tooltipText = currentTooltip.GetComponentInChildren<TextMeshProUGUI>();
        tooltipText.text = content;

        // ���� ũ�� ���� �� ��ġ ����
        UpdateTooltipSize();
        PositionTooltip();
    }

    private void PositionTooltip()
    {
        if (currentTooltip != null)
        {
            Vector3 mousePos = Input.mousePosition; // ���� ���콺 ��ġ
                                                    // ������ RectTransform�� �����ɴϴ�.
            RectTransform tooltipRect = currentTooltip.GetComponent<RectTransform>();

            // ������ ȭ�� ���� ǥ�õǵ��� ����
            float offsetY = 50f; // ���콺 ���� ǥ���ϱ� ���� ������
            float screenHeight = Screen.height;
            float tooltipHeight = tooltipRect.sizeDelta.y;

            // ���콺 ��ġ���� �������� ���� ��ġ
            Vector3 newPosition = new Vector3(mousePos.x, mousePos.y + offsetY, 0);

            // ������ ȭ�� ������ ������ �ʵ��� ����
            if (newPosition.y + tooltipHeight > screenHeight)
            {
                newPosition.y = screenHeight - tooltipHeight; // ȭ�� ����� �ʰ����� �ʵ��� ����
            }

            // ���� ��ġ ����
            currentTooltip.transform.position = newPosition;
        }
    }

    private void UpdateTooltipSize()
    {
        if (currentTooltip != null)
        {
            RectTransform tooltipRect = currentTooltip.GetComponent<RectTransform>();
            float padding = 10f; // �е� �� ����

            // �ؽ�Ʈ�� ũ�⸦ �����ͼ� �г� ũ�⸦ ����
            float width = tooltipText.preferredWidth + padding * 2; // �е� �߰�
            float height = tooltipText.preferredHeight + padding * 2; // �е� �߰�

            tooltipRect.sizeDelta = new Vector2(width, height); // �г� ũ�� ����
        }
    }

    public void HideTooltip()
    {
        if (currentTooltip != null)
        {
            Destroy(currentTooltip);
        }
    }

    void Update()
    {
        // ������ Ȱ��ȭ�Ǿ� ���� �� ���콺 ��ġ�� ���� ���� ��ġ ������Ʈ
        if (currentTooltip != null)
        {
            PositionTooltip();
        }
    }
}
