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
        if (eventData.button == PointerEventData.InputButton.Left) // ��Ŭ�� Ȯ��
        {
            // ���콺 Ŭ�� �� �гΰ��� ������ ���
            RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, eventData.position, eventData.pressEventCamera, out offset);
            offset = panelRectTransform.anchoredPosition - offset;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) // ��Ŭ�� �巡�� Ȯ��
        {
            MovePanel(eventData.position);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // �巡�� ���� �� �߰� ������ �ʿ��ϸ� ���⿡ �ۼ�
    }

    private void MovePanel(Vector2 mousePosition)
    {
        // ȭ�� ��ǥ�� ĵ���� ��ǥ�� ��ȯ
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform.parent.GetComponent<RectTransform>(), mousePosition, null, out anchoredPosition);

        // �г� ��ġ ������Ʈ
        panelRectTransform.anchoredPosition = anchoredPosition + offset;
    }
}
