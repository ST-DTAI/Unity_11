using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentTime : MonoBehaviour
{
    public TextMeshProUGUI timeText; // �ð� ǥ�ø� ���� Text ���
    public TextMeshProUGUI dateText;

    // Start is called before the first frame update
    void Start()
    {
        // ���� �ð��� �����ͼ� ����
        string currentTime = System.DateTime.Now.ToString("HH:mm:ss");
        timeText.text = currentTime; // Text ��ҿ� �ð� ������Ʈ

        string currentDay = System.DateTime.Now.ToString("yyyy��MM��dd��"); // ������ ����
        dateText.text = currentDay; // Text ��ҿ� ��¥ ������Ʈ
    }

    // Update is called once per frame
    void Update()
    {
        string currentTime = System.DateTime.Now.ToString("HH:mm:ss");
        timeText.text = currentTime; // Text ��ҿ� �ð� ������Ʈ
    }
}
