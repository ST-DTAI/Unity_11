using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NowTimeView : MonoBehaviour
{
    public TextMeshProUGUI timeText; // �ð� ǥ�ø� ���� Text ���


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    
    void Update()
    {
        // ���� �ð��� �����ͼ� ����
        string currentTime = System.DateTime.Now.ToString("HH:mm:ss");
        timeText.text = currentTime; // Text ��ҿ� �ð� ������Ʈ

    }
}
