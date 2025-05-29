using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI;

public class ManualPanelManager : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject panel;
    public Button registerButton;

    private Vector3 dragOffset; // 드래그 시작 시의 오프셋

    // 패널을 닫는 메서드
    public void OnCloseButtonClick()
    {
        panel.SetActive(false);
    }

    public void OnRegisterButtonClick()
    {
        Debug.Log("수작업 등록 되었습니다");
    }

    // 드래그 시작 시 호출되는 메서드
    public void OnBeginDrag(PointerEventData eventData)
    {
        dragOffset = transform.position - GetMouseWorldPosition(eventData);
    }

    // 드래그 중 호출되는 메서드
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = GetMouseWorldPosition(eventData) + dragOffset;
    }

    // 드래그 종료 시 호출되는 메서드
    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 종료 후 추가 작업이 필요하다면 여기에 작성
    }

    // 마우스의 월드 위치를 계산하는 메서드
    private Vector3 GetMouseWorldPosition(PointerEventData eventData)
    {
        Vector3 screenPos = eventData.position;
        screenPos.z = Camera.main.nearClipPlane; // 카메라의 가까운 클립 평면에 위치 설정
        return Camera.main.ScreenToWorldPoint(screenPos);
    }
}
