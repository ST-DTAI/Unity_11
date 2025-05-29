using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class testCell : MonoBehaviour
{
    public TextMeshProUGUI cellText; // 셀의 TextMeshProUGUI 컴포넌트

    void Start()
    {
        // Event Trigger 추가
        AddEventTrigger();
    }

    private void AddEventTrigger()
    {
        EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { OnCellClick(); });
        eventTrigger.triggers.Add(entry);
    }
    public void OnCellClick()
    {
        Debug.Log("Cell clicked: " + cellText.text); // 클릭 로그 확인
        // 클릭 시 툴팁 표시
        TooltipManager.Instance.ShowTooltip(cellText.text, transform.position);
    }
}
