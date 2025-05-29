using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // ���ο� ���� ������Ʈ ����
        GameObject container = new GameObject("ImageContainer");

        // RectTransform ������Ʈ �߰�
        RectTransform rectTransform = container.AddComponent<RectTransform>();

        // Horizontal Layout Group ������Ʈ �߰�
        HorizontalLayoutGroup layoutGroup = container.AddComponent<HorizontalLayoutGroup>();

        // Layout Group�� �Ӽ� ���� (�ʿ信 ���� ���� ����)
        layoutGroup.childAlignment = TextAnchor.MiddleCenter; // �ڽ� ����
        layoutGroup.spacing = 10; // �ڽ� ���� ����
        layoutGroup.padding = new RectOffset(10, 10, 10, 10); // �е� ����

        // Canvas�� �ڽ����� �߰�
        Canvas canvas = FindObjectOfType<Canvas>();
        container.transform.SetParent(canvas.transform);

        // RectTransform�� ũ�� ���� (�ʿ信 ���� ���� ����)
        rectTransform.sizeDelta = new Vector2(600, 100); // ���ϴ� ũ��� ����
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
