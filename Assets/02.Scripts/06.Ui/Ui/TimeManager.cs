using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public TMP_InputField timeInputField; // ��/��/�� ǥ�� TMP_InputField

    void Start()
    {
        // �ʱⰪ ���� (��: 12:00:00)
        timeInputField.text = "12:00:00";
    }

    void Update()
    {
        // �ð� �� ��ȿ�� �˻� �� ����
        ValidateInput();
    }

    private void ValidateInput()
    {
        // �Էµ� �ð� ���ڿ��� ':'�� �и�
        string[] timeParts = timeInputField.text.Split(':');

        // �� �κ� ��ȿ�� �˻�
        if (timeParts.Length == 3)
        {
            // �� �Է� �� �˻�
            if (int.TryParse(timeParts[0], out int hour))
            {
                if (hour < 0 || hour >= 24)
                {
                    timeParts[0] = "23"; // �߸��� �Է��� ��� �⺻������ ����
                }
            }
            else
            {
                timeParts[0] = "00"; // �߸��� �Է��� ��� �⺻������ ����
            }

            // �� �Է� �� �˻�
            if (int.TryParse(timeParts[1], out int minute))
            {
                if (minute < 0 || minute >= 60)
                {
                    timeParts[1] = "59"; // �߸��� �Է��� ��� �⺻������ ����
                }
            }
            else
            {
                timeParts[1] = "00"; // �߸��� �Է��� ��� �⺻������ ����
            }

            // �� �Է� �� �˻�
            if (int.TryParse(timeParts[2], out int second))
            {
                if (second < 0 || second >= 60)
                {
                    timeParts[2] = "59"; // �߸��� �Է��� ��� �⺻������ ����
                }
            }
            else
            {
                timeParts[2] = "00"; // �߸��� �Է��� ��� �⺻������ ����
            }

            // ��ȿ�� ���� �ٽ� �����Ͽ� �Է� �ʵ忡 ����
            timeInputField.text = $"{timeParts[0]}:{timeParts[1]}:{timeParts[2]}";
        }
        else
        {
            // �߸��� ������ ��� �⺻������ ����
            timeInputField.text = "12:00:00";
        }
    }
}