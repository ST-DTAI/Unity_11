using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{


    public TMP_InputField timeInputField;
    private int selectedUnitIndex = -1;


    // Start is called before the first frame update
    void Start()
    {
        // 초기 시간 설정
        timeInputField.text = "00:12:45";

        // InputField에 클릭 이벤트 리스너 추가
        timeInputField.onEndEdit.AddListener(OnInputFieldClick);
    }

    // Update is called once per frame
    void Update()
    {
        // 화살표 키로 시간 단위 조정
        if (selectedUnitIndex != -1)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                AdjustTimeUnit(1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                AdjustTimeUnit(-1);
            }
        }
    }

    void OnInputFieldClick(string input)
    {
        // 커서 위치 가져오기
        int cursorPosition = timeInputField.caretPosition;

        // 커서 위치에 따라 시간 단위 선택
        if (cursorPosition <= 2) // 시
        {
            selectedUnitIndex = 0;
        }
        else if (cursorPosition > 2 && cursorPosition <= 5) // 분
        {
            selectedUnitIndex = 1;
        }
        else if (cursorPosition > 5) // 초
        {
            selectedUnitIndex = 2;
        }
    }

    void AdjustTimeUnit(int adjustment)
    {
        string[] timeParts = timeInputField.text.Split(':');

        if (timeParts.Length != 3) return;

        int value = int.Parse(timeParts[selectedUnitIndex]);
        value += adjustment;

        if (selectedUnitIndex == 0) // 시
        {
            value = Mathf.Clamp(value, 0, 23);
        }
        else // 분, 초
        {
            value = Mathf.Clamp(value, 0, 59);
        }

        timeParts[selectedUnitIndex] = value.ToString("D2");
        timeInputField.text = string.Join(":", timeParts);

        // 커서 위치 유지
        SetCaretPosition();
    }

    void SetCaretPosition()
    {
        // 선택된 단위에 따라 커서 위치 설정
        switch (selectedUnitIndex)
        {
            case 0:
                timeInputField.stringPosition = 0;
                timeInputField.caretPosition = 0;
                break;
            case 1:
                timeInputField.stringPosition = 3;
                timeInputField.caretPosition = 3;
                break;
            case 2:
                timeInputField.stringPosition = 6;
                timeInputField.caretPosition = 6;
                break;
        }

        // 다른 칸으로 이동할 때 값을 두 자리로 업데이트
        UpdateTimeUnitFormat();
    }

    void UpdateTimeUnitFormat()
    {
        string[] timeParts = timeInputField.text.Split(':');

        if (timeParts.Length != 3) return;

        for (int i = 0; i < 3; i++)
        {
            int value = int.Parse(timeParts[i]);
            timeParts[i] = value.ToString("D2");
        }

        timeInputField.text = string.Join(":", timeParts);
    }
}
