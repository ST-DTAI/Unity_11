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
        // �ʱ� �ð� ����
        timeInputField.text = "00:12:45";

        // InputField�� Ŭ�� �̺�Ʈ ������ �߰�
        timeInputField.onEndEdit.AddListener(OnInputFieldClick);
    }

    // Update is called once per frame
    void Update()
    {
        // ȭ��ǥ Ű�� �ð� ���� ����
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
        // Ŀ�� ��ġ ��������
        int cursorPosition = timeInputField.caretPosition;

        // Ŀ�� ��ġ�� ���� �ð� ���� ����
        if (cursorPosition <= 2) // ��
        {
            selectedUnitIndex = 0;
        }
        else if (cursorPosition > 2 && cursorPosition <= 5) // ��
        {
            selectedUnitIndex = 1;
        }
        else if (cursorPosition > 5) // ��
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

        if (selectedUnitIndex == 0) // ��
        {
            value = Mathf.Clamp(value, 0, 23);
        }
        else // ��, ��
        {
            value = Mathf.Clamp(value, 0, 59);
        }

        timeParts[selectedUnitIndex] = value.ToString("D2");
        timeInputField.text = string.Join(":", timeParts);

        // Ŀ�� ��ġ ����
        SetCaretPosition();
    }

    void SetCaretPosition()
    {
        // ���õ� ������ ���� Ŀ�� ��ġ ����
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

        // �ٸ� ĭ���� �̵��� �� ���� �� �ڸ��� ������Ʈ
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
