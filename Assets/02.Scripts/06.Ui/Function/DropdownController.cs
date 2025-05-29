using UnityEngine;
using TMPro;

public class DropdownController : MonoBehaviour
{
    public TMP_Dropdown dropdown; // TMP_Dropdown 컴포넌트
    public GameObject[] panels;   // 패널 GameObjects 배열

    void Start()
    {
        // 드롭다운 값이 변경될 때 호출될 메서드 등록
        dropdown.onValueChanged.AddListener(DropdownValueChanged);

        // 모든 패널을 비활성화 상태로 시작
        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }
        // 드롭다운의 기본값을 임시로 다른 값으로 설정
        dropdown.value = dropdown.options.Count - 1;

        // 드롭다운 값 변경 이벤트를 수동으로 호출하여 기본값이 변경되었음을 알림
        DropdownValueChanged(dropdown.value);
    }

    void DropdownValueChanged(int value)
    {
        // 모든 패널을 비활성화
        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }

        // 선택된 값에 해당하는 패널을 활성화
        if (value >= 0 && value < panels.Length)
        {
            panels[value].SetActive(true);
        }
    }
}
