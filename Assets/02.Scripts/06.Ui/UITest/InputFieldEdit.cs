using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputFieldEdit : MonoBehaviour
{
    public TMP_InputField inputField; // InputField를 연결합니다.
    public Button toggleButton; // 버튼을 연결합니다.

    private bool isEditable = false;

    void Start()
    {
        // 초기 설정: InputField를 읽기 전용으로 설정
        inputField.interactable = isEditable;

        // 버튼 클릭 이벤트 추가
        toggleButton.onClick.AddListener(OnButtonClick);
    }

    // 버튼 클릭 시 호출되는 메서드
    void OnButtonClick()
    {
        // InputField의 수정 가능 여부를 전환
        isEditable = !isEditable;
        inputField.interactable = isEditable;

        // 버튼 텍스트 변경
        UpdateButtonText();
    }

    // 버튼 텍스트 업데이트 메서드
    void UpdateButtonText()
    {
        if (isEditable)
        {
            toggleButton.GetComponentInChildren<TMP_Text>().text = "수정 완료"; // 수정 가능 상태
        }
        else
        {
            toggleButton.GetComponentInChildren<TMP_Text>().text = "수정하기"; // 읽기 전용 상태
        }
    }
}
