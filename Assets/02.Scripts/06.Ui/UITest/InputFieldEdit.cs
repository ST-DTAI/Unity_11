using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputFieldEdit : MonoBehaviour
{
    public TMP_InputField inputField; // InputField�� �����մϴ�.
    public Button toggleButton; // ��ư�� �����մϴ�.

    private bool isEditable = false;

    void Start()
    {
        // �ʱ� ����: InputField�� �б� �������� ����
        inputField.interactable = isEditable;

        // ��ư Ŭ�� �̺�Ʈ �߰�
        toggleButton.onClick.AddListener(OnButtonClick);
    }

    // ��ư Ŭ�� �� ȣ��Ǵ� �޼���
    void OnButtonClick()
    {
        // InputField�� ���� ���� ���θ� ��ȯ
        isEditable = !isEditable;
        inputField.interactable = isEditable;

        // ��ư �ؽ�Ʈ ����
        UpdateButtonText();
    }

    // ��ư �ؽ�Ʈ ������Ʈ �޼���
    void UpdateButtonText()
    {
        if (isEditable)
        {
            toggleButton.GetComponentInChildren<TMP_Text>().text = "���� �Ϸ�"; // ���� ���� ����
        }
        else
        {
            toggleButton.GetComponentInChildren<TMP_Text>().text = "�����ϱ�"; // �б� ���� ����
        }
    }
}
