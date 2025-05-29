using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CraneLabel : MonoBehaviour
{
    public GameObject targetObject; // 따라갈 게임 오브젝트
    public GameObject labelPanel; // UI 텍스트

    void Update()
    {
        // 타겟 오브젝트의 위치에 따라 텍스트 위치 업데이트
        if (targetObject != null)
        {
            // 타겟 오브젝트의 위치를 월드 좌표로 가져옴
            Vector3 screenPos = Camera.main.WorldToScreenPoint(targetObject.transform.position);

            // 텍스트 위치를 스크린 좌표에 맞게 설정
            labelPanel.transform.position = screenPos;
        }
    }
}
