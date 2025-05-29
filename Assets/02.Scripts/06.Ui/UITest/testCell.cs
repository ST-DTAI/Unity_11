using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class testCell : MonoBehaviour
{
    public TextMeshProUGUI cellText; // ���� TextMeshProUGUI ������Ʈ

    void Start()
    {
        // Event Trigger �߰�
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
        Debug.Log("Cell clicked: " + cellText.text); // Ŭ�� �α� Ȯ��
        // Ŭ�� �� ���� ǥ��
        TooltipManager.Instance.ShowTooltip(cellText.text, transform.position);
    }
}
