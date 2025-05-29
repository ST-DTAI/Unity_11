using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentTime : MonoBehaviour
{
    public TextMeshProUGUI timeText; // 시간 표시를 위한 Text 요소
    public TextMeshProUGUI dateText;

    // Start is called before the first frame update
    void Start()
    {
        // 현재 시간을 가져와서 포맷
        string currentTime = System.DateTime.Now.ToString("HH:mm:ss");
        timeText.text = currentTime; // Text 요소에 시간 업데이트

        string currentDay = System.DateTime.Now.ToString("yyyy년MM월dd일"); // 연월일 포맷
        dateText.text = currentDay; // Text 요소에 날짜 업데이트
    }

    // Update is called once per frame
    void Update()
    {
        string currentTime = System.DateTime.Now.ToString("HH:mm:ss");
        timeText.text = currentTime; // Text 요소에 시간 업데이트
    }
}
