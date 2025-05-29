using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public TMP_InputField timeInputField; // 시/분/초 표시 TMP_InputField

    void Start()
    {
        // 초기값 설정 (예: 12:00:00)
        timeInputField.text = "12:00:00";
    }

    void Update()
    {
        // 시간 값 유효성 검사 및 제한
        ValidateInput();
    }

    private void ValidateInput()
    {
        // 입력된 시간 문자열을 ':'로 분리
        string[] timeParts = timeInputField.text.Split(':');

        // 각 부분 유효성 검사
        if (timeParts.Length == 3)
        {
            // 시 입력 값 검사
            if (int.TryParse(timeParts[0], out int hour))
            {
                if (hour < 0 || hour >= 24)
                {
                    timeParts[0] = "23"; // 잘못된 입력일 경우 기본값으로 설정
                }
            }
            else
            {
                timeParts[0] = "00"; // 잘못된 입력일 경우 기본값으로 설정
            }

            // 분 입력 값 검사
            if (int.TryParse(timeParts[1], out int minute))
            {
                if (minute < 0 || minute >= 60)
                {
                    timeParts[1] = "59"; // 잘못된 입력일 경우 기본값으로 설정
                }
            }
            else
            {
                timeParts[1] = "00"; // 잘못된 입력일 경우 기본값으로 설정
            }

            // 초 입력 값 검사
            if (int.TryParse(timeParts[2], out int second))
            {
                if (second < 0 || second >= 60)
                {
                    timeParts[2] = "59"; // 잘못된 입력일 경우 기본값으로 설정
                }
            }
            else
            {
                timeParts[2] = "00"; // 잘못된 입력일 경우 기본값으로 설정
            }

            // 유효한 값을 다시 조합하여 입력 필드에 설정
            timeInputField.text = $"{timeParts[0]}:{timeParts[1]}:{timeParts[2]}";
        }
        else
        {
            // 잘못된 형식일 경우 기본값으로 설정
            timeInputField.text = "12:00:00";
        }
    }
}