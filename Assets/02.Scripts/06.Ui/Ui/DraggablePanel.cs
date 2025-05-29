using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePanel : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RectTransform panelRectTransform;
    private Vector2 offset;

    void Start()
    {
        panelRectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) // 좌클릭 확인
        {
            // 마우스 클릭 시 패널과의 오프셋 계산
            RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, eventData.position, eventData.pressEventCamera, out offset);
            offset = panelRectTransform.anchoredPosition - offset;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) // 좌클릭 드래그 확인
        {
            MovePanel(eventData.position);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 드래그 종료 시 추가 로직이 필요하면 여기에 작성
    }

    private void MovePanel(Vector2 mousePosition)
    {
        // 화면 좌표를 캔버스 좌표로 변환
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform.parent.GetComponent<RectTransform>(), mousePosition, null, out anchoredPosition);

        // 패널 위치 업데이트
        panelRectTransform.anchoredPosition = anchoredPosition + offset;
    }
}
