using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlagButtonController : MonoBehaviour
{
    public RawImage rawImage; // RawImage ����
    public TMP_Text text; // Text ����
    public Button remoteButton; // ��ư ����
    public Button autoButton;
    private Color originalColor; // ���� ���� ����

    private void Start()
    {
        // ��ư Ŭ�� �� OnButtonClick �޼��� ȣ��
        remoteButton.onClick.AddListener(OnRemoteButtonClick);
        autoButton.onClick.AddListener(OnAutoButtonClick);


    }

    private void Update()
    {
        
        if (Input.GetMouseButton(0) && remoteButton.interactable)
        {
            // ��ư�� ������ pressedColor�� ����
            ColorBlock colors = remoteButton.colors;
            remoteButton.image.color = colors.pressedColor;
        }
        else
        {
            // ��ư�� ������ ���� �� ���� �������� ����
            remoteButton.image.color = originalColor;
        }
    }
    public void OnRemoteButtonClick()
    {
        // RawImage ���� ����
        Color newColor = new Color32(255, 203, 0, 255);
        rawImage.color = newColor;

        // Text ���� ����
        text.color = Color.black;

        text.text = "������";
    }

    public void OnAutoButtonClick()
    {
        // RawImage ���� ����
        Color newColor = new Color32(0, 58, 150, 255);
        rawImage.color = newColor;

        // Text ���� ����
        text.text = "�ڵ�";
        text.color = Color.white; 
    }
}
