using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TestTimeController : MonoBehaviour
{
    public TMP_InputField hourInputField; // 시 표시 TMP_InputField
    public TMP_InputField minuteInputField; // 분 표시 TMP_InputField
    public TMP_InputField secondInputField; // 초 표시 TMP_InputField

    private DateTime currentTime; // 현재 시간

    void Start()
    {
        // 초기값 설정 (예: 12:00:00)
        hourInputField.text = "12";
        minuteInputField.text = "00";
        secondInputField.text = "00";
    }

    void Update()
    {
        // 시간 값 유효성 검사 및 제한
        ValidateInputs();
    }

    private void ValidateInputs()
    {
        // 시 입력 값 검사
        if (int.TryParse(hourInputField.text, out int hour))
        {
            if (hour < 0 || hour >= 24)
            {
                hourInputField.text = "23"; // 잘못된 입력일 경우 기본값으로 설정
            }
        }
        else
        {
            hourInputField.text = "00"; // 잘못된 입력일 경우 기본값으로 설정
        }

        // 분 입력 값 검사
        if (int.TryParse(minuteInputField.text, out int minute))
        {
            if (minute < 0 || minute >= 60)
            {
                minuteInputField.text = "59"; // 잘못된 입력일 경우 기본값으로 설정
            }
        }
        else
        {
            minuteInputField.text = "00"; // 잘못된 입력일 경우 기본값으로 설정
        }

        // 초 입력 값 검사
        if (int.TryParse(secondInputField.text, out int second))
        {
            if (second < 0 || second >= 60)
            {
                secondInputField.text = "59"; // 잘못된 입력일 경우 기본값으로 설정
            }
        }
        else
        {
            secondInputField.text = "00"; // 잘못된 입력일 경우 기본값으로 설정
        }
    }
}
