using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public GameObject tooltip; // ���� UI �г�
    public TextMeshProUGUI tooltipText; // ������ ǥ���� �ؽ�Ʈ
    public TextMeshProUGUI contentText; // ���� ����

    void Start()
    {
        // ���� ��Ȱ��ȭ
        tooltip.SetActive(false);
    }

    public void OnMouseEnter()
    {
        // ���� Ȱ��ȭ �� ���� ����
        if (contentText != null)
        {
            tooltipText.text = contentText.text; // ���� �ؽ�Ʈ�� ������ ����
        }
        tooltip.SetActive(true);

        // ���� ��ġ ���� (���÷� ���콺 ��ġ�� ���� ����)
        PositionTooltip();
        // ���� ũ�� ����
        UpdateTooltipSize();
    }

    public void OnMouseExit()
    {
        // ���� ��Ȱ��ȭ
        tooltip.SetActive(false);
    }

    void Update()
    {
        // ���콺 ��ġ�� ���� ���� ��ġ ������Ʈ
        if (tooltip.activeSelf)
        {
            PositionTooltip();
        }
    }

    private void PositionTooltip()
    {
        Vector3 mousePos = Input.mousePosition;
        tooltip.transform.position = new Vector3(mousePos.x, mousePos.y + 50, mousePos.z); // ���콺 ���� ��ġ
    }
    private void UpdateTooltipSize()
    {
        // �ؽ�Ʈ�� ũ�⿡ ���� �г� ũ�� ����
        RectTransform tooltipRect = tooltip.GetComponent<RectTransform>();
        float padding = 10f; // �е� �� ����

        // �ؽ�Ʈ�� ũ�⸦ �����ͼ� �г� ũ�⸦ ����
        float width = tooltipText.preferredWidth + padding * 2; // �е� �߰�
        float height = tooltipText.preferredHeight + padding * 2; // �е� �߰�

        tooltipRect.sizeDelta = new Vector2(width, height); // �г� ũ�� ����
    }
}
