using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlagButtonController : MonoBehaviour
{
    public RawImage rawImage; // RawImage 참조
    public TMP_Text text; // Text 참조
    public Button remoteButton; // 버튼 참조
    public Button autoButton;
    private Color originalColor; // 원래 색상 저장

    private void Start()
    {
        // 버튼 클릭 시 OnButtonClick 메서드 호출
        remoteButton.onClick.AddListener(OnRemoteButtonClick);
        autoButton.onClick.AddListener(OnAutoButtonClick);


    }

    private void Update()
    {
        
        if (Input.GetMouseButton(0) && remoteButton.interactable)
        {
            // 버튼의 색상을 pressedColor로 변경
            ColorBlock colors = remoteButton.colors;
            remoteButton.image.color = colors.pressedColor;
        }
        else
        {
            // 버튼이 눌리지 않을 때 원래 색상으로 복원
            remoteButton.image.color = originalColor;
        }
    }
    public void OnRemoteButtonClick()
    {
        // RawImage 색상 변경
        Color newColor = new Color32(255, 203, 0, 255);
        rawImage.color = newColor;

        // Text 색상 변경
        text.color = Color.black;

        text.text = "리모컨";
    }

    public void OnAutoButtonClick()
    {
        // RawImage 색상 변경
        Color newColor = new Color32(0, 58, 150, 255);
        rawImage.color = newColor;

        // Text 색상 변경
        text.text = "자동";
        text.color = Color.white; 
    }
}
