using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 새로운 게임 오브젝트 생성
        GameObject container = new GameObject("ImageContainer");

        // RectTransform 컴포넌트 추가
        RectTransform rectTransform = container.AddComponent<RectTransform>();

        // Horizontal Layout Group 컴포넌트 추가
        HorizontalLayoutGroup layoutGroup = container.AddComponent<HorizontalLayoutGroup>();

        // Layout Group의 속성 설정 (필요에 따라 수정 가능)
        layoutGroup.childAlignment = TextAnchor.MiddleCenter; // 자식 정렬
        layoutGroup.spacing = 10; // 자식 간의 간격
        layoutGroup.padding = new RectOffset(10, 10, 10, 10); // 패딩 설정

        // Canvas에 자식으로 추가
        Canvas canvas = FindObjectOfType<Canvas>();
        container.transform.SetParent(canvas.transform);

        // RectTransform의 크기 설정 (필요에 따라 수정 가능)
        rectTransform.sizeDelta = new Vector2(600, 100); // 원하는 크기로 설정
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
