using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI;

public class ManualPanelManager : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject panel;
    public Button registerButton;

    private Vector3 dragOffset; // �巡�� ���� ���� ������

    // �г��� �ݴ� �޼���
    public void OnCloseButtonClick()
    {
        panel.SetActive(false);
    }

    public void OnRegisterButtonClick()
    {
        Debug.Log("���۾� ��� �Ǿ����ϴ�");
    }

    // �巡�� ���� �� ȣ��Ǵ� �޼���
    public void OnBeginDrag(PointerEventData eventData)
    {
        dragOffset = transform.position - GetMouseWorldPosition(eventData);
    }

    // �巡�� �� ȣ��Ǵ� �޼���
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = GetMouseWorldPosition(eventData) + dragOffset;
    }

    // �巡�� ���� �� ȣ��Ǵ� �޼���
    public void OnEndDrag(PointerEventData eventData)
    {
        // �巡�� ���� �� �߰� �۾��� �ʿ��ϴٸ� ���⿡ �ۼ�
    }

    // ���콺�� ���� ��ġ�� ����ϴ� �޼���
    private Vector3 GetMouseWorldPosition(PointerEventData eventData)
    {
        Vector3 screenPos = eventData.position;
        screenPos.z = Camera.main.nearClipPlane; // ī�޶��� ����� Ŭ�� ��鿡 ��ġ ����
        return Camera.main.ScreenToWorldPoint(screenPos);
    }
}
