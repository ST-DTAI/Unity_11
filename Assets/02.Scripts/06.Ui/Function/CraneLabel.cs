using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CraneLabel : MonoBehaviour
{
    public GameObject targetObject; // ���� ���� ������Ʈ
    public GameObject labelPanel; // UI �ؽ�Ʈ

    void Update()
    {
        // Ÿ�� ������Ʈ�� ��ġ�� ���� �ؽ�Ʈ ��ġ ������Ʈ
        if (targetObject != null)
        {
            // Ÿ�� ������Ʈ�� ��ġ�� ���� ��ǥ�� ������
            Vector3 screenPos = Camera.main.WorldToScreenPoint(targetObject.transform.position);

            // �ؽ�Ʈ ��ġ�� ��ũ�� ��ǥ�� �°� ����
            labelPanel.transform.position = screenPos;
        }
    }
}
