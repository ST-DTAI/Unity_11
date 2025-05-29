using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TestTimeController : MonoBehaviour
{
    public TMP_InputField hourInputField; // �� ǥ�� TMP_InputField
    public TMP_InputField minuteInputField; // �� ǥ�� TMP_InputField
    public TMP_InputField secondInputField; // �� ǥ�� TMP_InputField

    private DateTime currentTime; // ���� �ð�

    void Start()
    {
        // �ʱⰪ ���� (��: 12:00:00)
        hourInputField.text = "12";
        minuteInputField.text = "00";
        secondInputField.text = "00";
    }

    void Update()
    {
        // �ð� �� ��ȿ�� �˻� �� ����
        ValidateInputs();
    }

    private void ValidateInputs()
    {
        // �� �Է� �� �˻�
        if (int.TryParse(hourInputField.text, out int hour))
        {
            if (hour < 0 || hour >= 24)
            {
                hourInputField.text = "23"; // �߸��� �Է��� ��� �⺻������ ����
            }
        }
        else
        {
            hourInputField.text = "00"; // �߸��� �Է��� ��� �⺻������ ����
        }

        // �� �Է� �� �˻�
        if (int.TryParse(minuteInputField.text, out int minute))
        {
            if (minute < 0 || minute >= 60)
            {
                minuteInputField.text = "59"; // �߸��� �Է��� ��� �⺻������ ����
            }
        }
        else
        {
            minuteInputField.text = "00"; // �߸��� �Է��� ��� �⺻������ ����
        }

        // �� �Է� �� �˻�
        if (int.TryParse(secondInputField.text, out int second))
        {
            if (second < 0 || second >= 60)
            {
                secondInputField.text = "59"; // �߸��� �Է��� ��� �⺻������ ����
            }
        }
        else
        {
            secondInputField.text = "00"; // �߸��� �Է��� ��� �⺻������ ����
        }
    }
}
