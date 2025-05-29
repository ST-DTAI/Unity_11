using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public GameObject tooltip; // 툴팁 UI 패널
    public TextMeshProUGUI tooltipText; // 툴팁에 표시할 텍스트
    public TextMeshProUGUI contentText; // 셀의 내용

    void Start()
    {
        // 툴팁 비활성화
        tooltip.SetActive(false);
    }

    public void OnMouseEnter()
    {
        // 툴팁 활성화 및 내용 설정
        if (contentText != null)
        {
            tooltipText.text = contentText.text; // 셀의 텍스트를 툴팁에 설정
        }
        tooltip.SetActive(true);

        // 툴팁 위치 조정 (예시로 마우스 위치에 따라 설정)
        PositionTooltip();
        // 툴팁 크기 조정
        UpdateTooltipSize();
    }

    public void OnMouseExit()
    {
        // 툴팁 비활성화
        tooltip.SetActive(false);
    }

    void Update()
    {
        // 마우스 위치에 따라 툴팁 위치 업데이트
        if (tooltip.activeSelf)
        {
            PositionTooltip();
        }
    }

    private void PositionTooltip()
    {
        Vector3 mousePos = Input.mousePosition;
        tooltip.transform.position = new Vector3(mousePos.x, mousePos.y + 50, mousePos.z); // 마우스 위에 위치
    }
    private void UpdateTooltipSize()
    {
        // 텍스트의 크기에 따라 패널 크기 조정
        RectTransform tooltipRect = tooltip.GetComponent<RectTransform>();
        float padding = 10f; // 패딩 값 설정

        // 텍스트의 크기를 가져와서 패널 크기를 조정
        float width = tooltipText.preferredWidth + padding * 2; // 패딩 추가
        float height = tooltipText.preferredHeight + padding * 2; // 패딩 추가

        tooltipRect.sizeDelta = new Vector2(width, height); // 패널 크기 설정
    }
}
