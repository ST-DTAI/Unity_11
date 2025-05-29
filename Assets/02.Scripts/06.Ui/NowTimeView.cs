using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NowTimeView : MonoBehaviour
{
    public TextMeshProUGUI timeText; // 시간 표시를 위한 Text 요소


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    
    void Update()
    {
        // 현재 시간을 가져와서 포맷
        string currentTime = System.DateTime.Now.ToString("HH:mm:ss");
        timeText.text = currentTime; // Text 요소에 시간 업데이트

    }
}
