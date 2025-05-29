using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class CalendarDateItem : MonoBehaviour 
{
    private CalendarController calendarController;

    void Start()
    {
        // 부모 오브젝트에서 CalendarController 컴포넌트를 가져옵니다.
        calendarController = GetComponentInParent<CalendarController>();
    }

    public void OnDateItemClick()
    {
        // TMP_Text를 사용하여 텍스트를 가져옵니다.
        TMP_Text tmpText = gameObject.GetComponentInChildren<TMP_Text>();
        string dayText = tmpText.text;

        // 각 인스턴스의 calendarController를 통해 OnDateItemClick 호출
        calendarController.OnDateItemClick(dayText);
    }
}
