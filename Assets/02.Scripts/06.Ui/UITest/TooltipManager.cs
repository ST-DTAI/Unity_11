using TMPro;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }
    public GameObject tooltipPrefab; // 툴팁 프리팹
    private GameObject currentTooltip; // 현재 활성화된 툴팁
    private TextMeshProUGUI tooltipText; // 툴팁의 텍스트 컴포넌트

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
        Debug.Log("Showing tooltip with content: " + content); // 툴팁 내용 로그 확인
        // 기존 툴팁이 있으면 삭제
        if (currentTooltip != null)
        {
            Destroy(currentTooltip);
        }

        // 새로운 툴팁 생성
        currentTooltip = Instantiate(tooltipPrefab, position, Quaternion.identity);

        Debug.Log("Current Tooltip: " + currentTooltip); // 툴팁이 생성되었는지 확인

        tooltipText = currentTooltip.GetComponentInChildren<TextMeshProUGUI>();
        tooltipText.text = content;

        // 툴팁 크기 조정 및 위치 조정
        UpdateTooltipSize();
        PositionTooltip();
    }

    private void PositionTooltip()
    {
        if (currentTooltip != null)
        {
            Vector3 mousePos = Input.mousePosition; // 현재 마우스 위치
                                                    // 툴팁의 RectTransform을 가져옵니다.
            RectTransform tooltipRect = currentTooltip.GetComponent<RectTransform>();

            // 툴팁이 화면 내에 표시되도록 조정
            float offsetY = 50f; // 마우스 위에 표시하기 위한 오프셋
            float screenHeight = Screen.height;
            float tooltipHeight = tooltipRect.sizeDelta.y;

            // 마우스 위치에서 오프셋을 더한 위치
            Vector3 newPosition = new Vector3(mousePos.x, mousePos.y + offsetY, 0);

            // 툴팁이 화면 밖으로 나가지 않도록 조정
            if (newPosition.y + tooltipHeight > screenHeight)
            {
                newPosition.y = screenHeight - tooltipHeight; // 화면 상단을 초과하지 않도록 조정
            }

            // 최종 위치 설정
            currentTooltip.transform.position = newPosition;
        }
    }

    private void UpdateTooltipSize()
    {
        if (currentTooltip != null)
        {
            RectTransform tooltipRect = currentTooltip.GetComponent<RectTransform>();
            float padding = 10f; // 패딩 값 설정

            // 텍스트의 크기를 가져와서 패널 크기를 조정
            float width = tooltipText.preferredWidth + padding * 2; // 패딩 추가
            float height = tooltipText.preferredHeight + padding * 2; // 패딩 추가

            tooltipRect.sizeDelta = new Vector2(width, height); // 패널 크기 설정
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
        // 툴팁이 활성화되어 있을 때 마우스 위치에 따라 툴팁 위치 업데이트
        if (currentTooltip != null)
        {
            PositionTooltip();
        }
    }
}
